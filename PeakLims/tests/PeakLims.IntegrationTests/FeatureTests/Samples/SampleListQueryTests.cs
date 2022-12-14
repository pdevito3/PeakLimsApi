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
using Services;

public class SampleListQueryTests : TestBase
{
    
    [Test]
    public async Task can_get_sample_list()
    {
        // Arrange
        var fakePatientOne = FakePatient.Generate(GetService<IDateTimeProvider>());
        var fakePatientTwo = FakePatient.Generate(GetService<IDateTimeProvider>());
        await InsertAsync(fakePatientOne, fakePatientTwo);

        var container = FakeContainer.Generate();

        var fakeContainerOne = FakeContainer.Generate(new FakeContainerForCreationDto().Generate());
        var fakeContainerTwo = FakeContainer.Generate(new FakeContainerForCreationDto().Generate());
        await InsertAsync(fakeContainerOne, fakeContainerTwo);

        var fakeSampleOne = FakeSample.Generate(new FakeContainerlessSampleForCreationDto()
            .RuleFor(s => s.PatientId, _ => fakePatientOne.Id)
            .Generate(), container);
        var fakeSampleTwo = FakeSample.Generate(new FakeContainerlessSampleForCreationDto()
            .RuleFor(s => s.PatientId, _ => fakePatientTwo.Id)
            .Generate(), container);
        var queryParameters = new SampleParametersDto();

        await InsertAsync(fakeSampleOne, fakeSampleTwo);

        // Act
        var query = new GetSampleList.Query(queryParameters);
        var samples = await SendAsync(query);

        // Assert
        samples.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}