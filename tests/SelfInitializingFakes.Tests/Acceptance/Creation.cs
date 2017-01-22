﻿namespace SelfInitializingFakes.Tests.Acceptance
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class Creation
    {
        public interface IService
        {
        }

        [Scenario]
        public static void CreateFromNullServiceFactory(
            ICallDataRepository repository,
            Func<IService> serviceFactory,
            Exception exception)
        {
            "Given a null service factory"
                .x(() => serviceFactory = null);

            "And a saved call repository"
                .x(() => repository = A.Fake<ICallDataRepository>());

            "When I create a self-initializing fake"
                .x(() => exception = Record.Exception(() =>
                        SelfInitializingFake.For(serviceFactory, repository)));

            "Then the constructor throws an exception"
                .x(() => exception.Should()
                    .BeOfType<ArgumentNullException>()
                    .Which.ParamName.Should().Be("serviceFactory"));
        }

        [Scenario]
        public static void CreateFromNullCallRepository(
            ICallDataRepository repository,
            Func<IService> serviceFactory,
            Exception exception)
        {
            "Given a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "And a null saved call repository"
                .x(() => repository = null);

            "When I create a self-initializing fake"
                .x(() => exception = Record.Exception(() =>
                        SelfInitializingFake.For(serviceFactory, repository)));

            "Then the constructor throws an exception"
                .x(() => exception.Should()
                    .BeOfType<ArgumentNullException>()
                    .Which.ParamName.Should().Be("repository"));
        }

        [Scenario]
        public static void CreateFromRepositoryAndServiceFactory(
            ICallDataRepository repository,
            Func<IService> serviceFactory,
            SelfInitializingFake<IService> fake)
        {
            "Given a saved call repository"
                .x(() => repository = A.Fake<ICallDataRepository>());

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For(serviceFactory, repository));

            "Then the self-initializing fake is created"
                .x(() => fake.Should().NotBeNull());

            "And its Fake property is not null"
                .x(() => fake.Fake.Should().NotBeNull());
        }

        [Scenario]
        public static void CreateFromInitializedRepository(
            ICallDataRepository repository,
            Func<IService> serviceFactory,
            SelfInitializingFake<IService> fake)
        {
            "Given a saved call repository"
                .x(() => repository = A.Fake<ICallDataRepository>());

            "And the repository has been initialized"
                .x(() => A.CallTo(() => repository.Load()).Returns(new CallData[0]));

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For(serviceFactory, repository));

            "Then the factory is not invoked"
                .x(() => A.CallTo(serviceFactory).MustNotHaveHappened());
        }

        [Scenario]
        public static void CreateFromUninitializedRepository(
            ICallDataRepository repository,
            Func<IService> serviceFactory,
            SelfInitializingFake<IService> fake)
        {
            "Given a saved call repository"
                .x(() => repository = A.Fake<ICallDataRepository>());

            "And the repository has not been initialized"
                .x(() => A.CallTo(() => repository.Load()).Returns(null));

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For(serviceFactory, repository));

            "Then the factory is invoked to create the service"
                .x(() => A.CallTo(serviceFactory).MustHaveHappened());
        }

        [Scenario]
        public static void CreateFromDerivedFactoryType(
            ICallDataRepository repository,
            Func<Service> serviceFactory,
            SelfInitializingFake<IService> fake)
        {
            "Given a saved call repository"
                .x(() => repository = A.Fake<ICallDataRepository>());

            "And the repository has not been initialized"
                .x(() => A.CallTo(() => repository.Load()).Returns(null));

            "And a service factory that creates a derived type"
                .x(() => serviceFactory = A.Fake<Func<Service>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For<IService>(serviceFactory, repository));

            "Then the factory is invoked to create the service"
                .x(() => A.CallTo(serviceFactory).MustHaveHappened());
        }

        public class Service : IService
        {
        }
    }
}
