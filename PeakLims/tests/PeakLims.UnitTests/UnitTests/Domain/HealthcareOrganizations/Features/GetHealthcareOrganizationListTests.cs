namespace PeakLims.UnitTests.UnitTests.Domain.HealthcareOrganizations.Features;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Mappings;
using PeakLims.Domain.HealthcareOrganizations.Features;
using PeakLims.Domain.HealthcareOrganizations.Services;
using MapsterMapper;
using FluentAssertions;
using HeimGuard;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using Sieve.Models;
using Sieve.Services;
using TestHelpers;
using NUnit.Framework;

public class GetHealthcareOrganizationListTests
{
    
    private readonly SieveProcessor _sieveProcessor;
    private readonly Mapper _mapper = UnitTestUtils.GetApiMapper();
    private readonly Mock<IHealthcareOrganizationRepository> _healthcareOrganizationRepository;
    private readonly Mock<IHeimGuardClient> _heimGuard;

    public GetHealthcareOrganizationListTests()
    {
        _healthcareOrganizationRepository = new Mock<IHealthcareOrganizationRepository>();
        var sieveOptions = Options.Create(new SieveOptions());
        _sieveProcessor = new SieveProcessor(sieveOptions);
        _heimGuard = new Mock<IHeimGuardClient>();
    }
    
    [Test]
    public async Task can_get_paged_list_of_healthcareOrganization()
    {
        //Arrange
        var fakeHealthcareOrganizationOne = FakeHealthcareOrganization.Generate();
        var fakeHealthcareOrganizationTwo = FakeHealthcareOrganization.Generate();
        var fakeHealthcareOrganizationThree = FakeHealthcareOrganization.Generate();
        var healthcareOrganization = new List<HealthcareOrganization>();
        healthcareOrganization.Add(fakeHealthcareOrganizationOne);
        healthcareOrganization.Add(fakeHealthcareOrganizationTwo);
        healthcareOrganization.Add(fakeHealthcareOrganizationThree);
        var mockDbData = healthcareOrganization.AsQueryable().BuildMock();
        
        var queryParameters = new HealthcareOrganizationParametersDto() { PageSize = 1, PageNumber = 2 };

        _healthcareOrganizationRepository
            .Setup(x => x.Query())
            .Returns(mockDbData);
        
        //Act
        var query = new GetHealthcareOrganizationList.Query(queryParameters);
        var handler = new GetHealthcareOrganizationList.Handler(_healthcareOrganizationRepository.Object, _mapper, _sieveProcessor, _heimGuard.Object);
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().HaveCount(1);
    }
}