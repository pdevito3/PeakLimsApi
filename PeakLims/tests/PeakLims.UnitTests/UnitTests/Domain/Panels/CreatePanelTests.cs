namespace PeakLims.UnitTests.UnitTests.Domain.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

[Parallelizable]
public class CreatePanelTests
{
    private readonly Faker _faker;

    public CreatePanelTests()
    {
        _faker = new Faker();
    }
    
    [Test]
    public void can_create_valid_panel()
    {
        // Arrange + Act
        var panelToCreate = new FakePanelForCreationDto().Generate();
        var fakePanel = FakePanel.Generate(panelToCreate);

        // Assert
        fakePanel.PanelNumber.Should().Be(panelToCreate.PanelNumber);
        fakePanel.PanelCode.Should().Be(panelToCreate.PanelCode);
        fakePanel.PanelName.Should().Be(panelToCreate.PanelName);
        fakePanel.TurnAroundTime.Should().Be(panelToCreate.TurnAroundTime);
        fakePanel.Type.Should().Be(panelToCreate.Type);
        fakePanel.Version.Should().Be(panelToCreate.Version);
    }

    [Test]
    public void queue_domain_event_on_create()
    {
        // Arrange + Act
        var fakePanel = FakePanel.Generate();

        // Assert
        fakePanel.DomainEvents.Count.Should().Be(1);
        fakePanel.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PanelCreated));
    }
}