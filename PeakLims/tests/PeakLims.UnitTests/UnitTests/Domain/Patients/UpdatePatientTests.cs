namespace PeakLims.UnitTests.UnitTests.Domain.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

[Parallelizable]
public class UpdatePatientTests
{
    private readonly Faker _faker;

    public UpdatePatientTests()
    {
        _faker = new Faker();
    }
    
    [Test]
    public void can_update_patient()
    {
        // Arrange
        var fakePatient = FakePatient.Generate();
        var updatedPatient = new FakePatientForUpdateDto().Generate();
        
        // Act
        fakePatient.Update(updatedPatient);

        // Assert
        fakePatient.FirstName.Should().Be(updatedPatient.FirstName);
        fakePatient.LastName.Should().Be(updatedPatient.LastName);
        fakePatient.DateOfBirth.Should().Be(updatedPatient.DateOfBirth);
        fakePatient.Age.Should().Be(updatedPatient.Age);
        fakePatient.Sex.Should().Be(updatedPatient.Sex);
        fakePatient.Race.Should().Be(updatedPatient.Race);
        fakePatient.Ethnicity.Should().Be(updatedPatient.Ethnicity);
        fakePatient.InternalId.Should().Be(updatedPatient.InternalId);
    }
    
    [Test]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakePatient = FakePatient.Generate();
        var updatedPatient = new FakePatientForUpdateDto().Generate();
        fakePatient.DomainEvents.Clear();
        
        // Act
        fakePatient.Update(updatedPatient);

        // Assert
        fakePatient.DomainEvents.Count.Should().Be(1);
        fakePatient.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PatientUpdated));
    }
}