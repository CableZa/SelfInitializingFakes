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
        public static void CreatingFromNullServiceFactory(
            ISavedCallRepository repository,
            Func<IService> serviceFactory,
            Exception exception)
        {
            "Given a null service factory"
                .x(() => serviceFactory = null);

            "And a saved call repository"
                .x(() => repository = A.Fake<ISavedCallRepository>());

            "When I create a self-initializing fake"
                .x(() => exception = Record.Exception(() =>
                        SelfInitializingFake.For<IService>(serviceFactory, repository)));

            "Then the constructor throws an exception"
                .x(() => exception.Should()
                    .BeOfType<ArgumentNullException>()
                    .Which.ParamName.Should().Be("serviceFactory"));
        }

        [Scenario]
        public static void CreatingFromNullCallRepository(
            ISavedCallRepository repository,
            Func<IService> serviceFactory,
            Exception exception)
        {
            "Given a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "And a null saved call repository"
                .x(() => repository = null);

            "When I create a self-initializing fake"
                .x(() => exception = Record.Exception(() =>
                        SelfInitializingFake.For<IService>(serviceFactory, repository)));

            "Then the constructor throws an exception"
                .x(() => exception.Should()
                    .BeOfType<ArgumentNullException>()
                    .Which.ParamName.Should().Be("repository"));
        }

        [Scenario]
        public static void CreatingFromRepositoryAndServiceFactory(
            ISavedCallRepository repository,
            Func<IService> serviceFactory,
            IService fake)
        {
            "Given a saved call repository"
                 .x(() => repository = A.Fake<ISavedCallRepository>());

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For<IService>(serviceFactory, repository));

            "Then the fake is created"
                .x(() => fake.Should().NotBeNull());
        }

        [Scenario]
        public static void CreatingFromInitializedRepository(
            ISavedCallRepository repository,
            Func<IService> serviceFactory,
            IService fake)
        {
            "Given a saved call repository"
                 .x(() => repository = A.Fake<ISavedCallRepository>());

            "And the repository has been initialized"
                .x(() => A.CallTo(() => repository.LoadCalls()).Returns(new ISavedCall[0]));

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For<IService>(serviceFactory, repository));

            "Then the factory is not invoked"
                .x(() => A.CallTo(serviceFactory).MustNotHaveHappened());
        }

        [Scenario]
        public static void CreatingFromUninitializedRepository(
            ISavedCallRepository repository,
            Func<IService> serviceFactory,
            IService fake)
        {
            "Given a saved call repository"
                 .x(() => repository = A.Fake<ISavedCallRepository>());

            "And the repository has not been initialized"
                .x(() => A.CallTo(() => repository.LoadCalls()).Returns(null));

            "And a service factory"
                .x(() => serviceFactory = A.Fake<Func<IService>>());

            "When I create a self-initializing fake"
                .x(() => fake = SelfInitializingFake.For<IService>(serviceFactory, repository));

            "Then the factory is invoked to create the service"
                .x(() => A.CallTo(serviceFactory).MustHaveHappened());
        }
    }
}
