namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Sample;
using SharedKernel.Exceptions;
using PeakLims.Domain.Samples.Features;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.SharedTestHelpers.Fakes.Container;

public class SampleListQueryTests : TestBase
{
    
    [Test]
    public async Task can_get_sample_list()
    {
        // Arrange
        var fakePatientOne = FakePatient.Generate(new FakePatientForCreationDto().Generate());
        var fakePatientTwo = FakePatient.Generate(new FakePatientForCreationDto().Generate());
        await InsertAsync(fakePatientOne, fakePatientTwo);

        var fakeSampleParentOne = FakeSample.Generate(new FakeSampleForCreationDto().Generate());
        var fakeSampleParentTwo = FakeSample.Generate(new FakeSampleForCreationDto().Generate());
        await InsertAsync(fakeSampleParentOne, fakeSampleParentTwo);

        var fakeContainerOne = FakeContainer.Generate(new FakeContainerForCreationDto().Generate());
        var fakeContainerTwo = FakeContainer.Generate(new FakeContainerForCreationDto().Generate());
        await InsertAsync(fakeContainerOne, fakeContainerTwo);

        var fakeSampleOne = FakeSample.Generate(new FakeSampleForCreationDto()
            .RuleFor(s => s.PatientId, _ => fakePatientOne.Id)
            .RuleFor(s => s.ParentSampleId, _ => fakeSampleParentOne.Id)
            .RuleFor(s => s.ContainerId, _ => fakeContainerOne.Id).Generate());
        var fakeSampleTwo = FakeSample.Generate(new FakeSampleForCreationDto()
            .RuleFor(s => s.PatientId, _ => fakePatientTwo.Id)
            .RuleFor(s => s.ParentSampleId, _ => fakeSampleParentTwo.Id)
            .RuleFor(s => s.ContainerId, _ => fakeContainerTwo.Id).Generate());
        var queryParameters = new SampleParametersDto();

        await InsertAsync(fakeSampleOne, fakeSampleTwo);

        // Act
        var query = new GetSampleList.Query(queryParameters);
        var samples = await SendAsync(query);

        // Assert
        samples.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}