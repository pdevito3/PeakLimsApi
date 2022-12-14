namespace PeakLims.IntegrationTests.FeatureTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Domain.ContainerStatuses;
using PeakLims.Domain.Containers.Features;
using static TestFixture;
using SharedKernel.Exceptions;

public class AddContainerCommandTests : TestBase
{
    [Test]
    public async Task can_add_new_container_to_db()
    {
        // Arrange
        var fakeContainerOne = new FakeContainerForCreationDto().Generate();

        // Act
        var command = new AddContainer.Command(fakeContainerOne);
        var containerReturned = await SendAsync(command);
        var containerCreated = await ExecuteDbContextAsync(db => db.Containers
            .FirstOrDefaultAsync(c => c.Id == containerReturned.Id));

        // Assert
        containerReturned.Status.Should().Be(ContainerStatus.Active().Value);
        containerReturned.Type.Should().Be(fakeContainerOne.Type);

        containerCreated.Status.Should().Be(ContainerStatus.Active());
        containerCreated.Type.Should().Be(fakeContainerOne.Type);
    }
}