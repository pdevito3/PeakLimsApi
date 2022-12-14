namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Domain.AccessionStatuses;
using PeakLims.Domain.Accessions.Features;
using static TestFixture;
using SharedKernel.Exceptions;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Services;

public class AddAccessionCommandTests : TestBase
{
    [Test]
    public async Task can_add_new_accession_to_db()
    {
        // Arrange
        var patient = FakePatient.Generate(new FakePatientForCreationDto().Generate(), GetService<IDateTimeProvider>());
        await InsertAsync(patient);

        var org = FakeHealthcareOrganization.Generate(new FakeHealthcareOrganizationForCreationDto().Generate());
        await InsertAsync(org);

        // Act
        var command = new AddAccession.Command(patient.Id, org.Id);
        var accessionReturned = await SendAsync(command);
        var accessionCreated = await ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(a => a.Id == accessionReturned.Id));

        // Assert
        accessionReturned.Status.Should().Be(AccessionStatus.Draft().Value);
        accessionReturned.PatientId.Should().Be(patient.Id);
        accessionReturned.HealthcareOrganizationId.Should().Be(org.Id);
        
        accessionCreated.Status.Should().Be(AccessionStatus.Draft());
        accessionCreated.PatientId.Should().Be(patient.Id);
        accessionCreated.HealthcareOrganizationId.Should().Be(org.Id);
    }
}