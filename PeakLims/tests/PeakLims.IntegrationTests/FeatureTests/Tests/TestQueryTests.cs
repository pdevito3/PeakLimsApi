namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharedKernel.Exceptions;
using System.Threading.Tasks;
using Domain.Tests.Services;
using static TestFixture;

public class TestQueryTests : TestBase
{
    [Test]
    public async Task can_get_existing_test_with_accurate_props()
    {
        // Arrange
        var fakeTestOne = new FakeTestBuilder()
            .WithRepository(GetService<ITestRepository>())
            .Build();
        await InsertAsync(fakeTestOne);

        // Act
        var query = new GetTest.Query(fakeTestOne.Id);
        var test = await SendAsync(query);

        // Assert
        test.TestCode.Should().Be(fakeTestOne.TestCode);
        test.TestName.Should().Be(fakeTestOne.TestName);
        test.Methodology.Should().Be(fakeTestOne.Methodology);
        test.Platform.Should().Be(fakeTestOne.Platform);
        test.Version.Should().Be(fakeTestOne.Version);
        test.Status.Should().Be(fakeTestOne.Status);
        test.TurnAroundTime.Should().Be(fakeTestOne.TurnAroundTime);
    }

    [Test]
    public async Task get_test_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var badId = Guid.NewGuid();

        // Act
        var query = new GetTest.Query(badId);
        Func<Task> act = () => SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}