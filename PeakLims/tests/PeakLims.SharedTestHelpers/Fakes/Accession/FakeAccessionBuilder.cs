namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using Domain.HealthcareOrganizationContacts;
using Domain.HealthcareOrganizations;
using Domain.Panels;
using Domain.Patients;
using Domain.TestOrders;
using Domain.Tests;
using Domain.Tests.Services;
using HealthcareOrganizationContact;
using Moq;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;
using Test;
using TestOrder;

public class FakeAccessionBuilder :
    ITestRepositorySetterStage
{
    private readonly List<HealthcareOrganizationContact> _contacts = new List<HealthcareOrganizationContact>();
    private readonly List<Panel> _panels = new List<Panel>();
    private readonly List<Test> _tests = new List<Test>();
    private bool _includeATestOrder = true;
    private bool _includeAContact = true;
    private Patient _patient = null;
    private HealthcareOrganization _org = null;
    private ITestRepository _testRepository = null;

    private FakeAccessionBuilder() { }
    
    public static ITestRepositorySetterStage Initialize() => new FakeAccessionBuilder();

    public FakeAccessionBuilder WithPatient(Patient patient)
    {
        _patient = patient;
        return this;
    }
    
    public FakeAccessionBuilder WithHealthcareOrganization(HealthcareOrganization org)
    {
        _org = org;
        return this;
    }
    
    public FakeAccessionBuilder WithContact(HealthcareOrganizationContact contact)
    {
        _contacts.Add(contact);
        return this;
    }
    
    public FakeAccessionBuilder WithPanel(Panel panel)
    {
        _panels.Add(panel);
        return this;
    }
    
    public FakeAccessionBuilder WithTest(Test test)
    {
        _tests.Add(test);
        return this;
    }
    
    public FakeAccessionBuilder ExcludeTestOrders()
    {
        _includeATestOrder = false;
        return this;
    }
    
    public FakeAccessionBuilder ExcludeContacts()
    {
        _includeAContact = false;
        return this;
    }
    
    public FakeAccessionBuilder WithTestRepository(ITestRepository testRepository)
    {
        _testRepository = testRepository;
        return this;
    }
    
    public FakeAccessionBuilder WithMockTestRepository(bool testExists = false)
    {
        var mockTestRepository = new Mock<ITestRepository>();
        mockTestRepository
            .Setup(x => x.Exists(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(testExists);
        
        _testRepository = mockTestRepository.Object;
        return this;
    }
    
    public Accession Build()
    {
        var accession = Accession.Create();
        if (_patient != null)
            accession.SetPatient(_patient);
        if (_org != null)
        {
            accession.SetHealthcareOrganization(_org);
        }
        else
        {
            ExcludeContacts();
        }
        
        if(_contacts.Count <= 0 && _includeAContact)
            _contacts.Add(FakeHealthcareOrganizationContact.Generate(new FakeHealthcareOrganizationContactForCreationDto()
                .RuleFor(x => x.HealthcareOrganizationId, _org?.Id)
                .Generate()));

        if (_tests.Count <= 0 && _panels.Count <= 0 && _includeATestOrder)
        {
            var test = new FakeTestBuilder()
                .WithRepository(_testRepository)
                .Activate()
                .Build();
            accession.AddTest(test);
        }
        
        foreach (var contact in _contacts)
        {
            accession.AddContact(contact);
        }
        foreach (var panel in _panels)
        {
            accession.AddPanel(panel);
        }
        foreach (var testOrder in _tests)
        {
            accession.AddTest(testOrder);
        }

        return accession;
    }
}

public interface ITestRepositorySetterStage
{
    public FakeAccessionBuilder WithTestRepository(ITestRepository testRepository);
    public FakeAccessionBuilder WithMockTestRepository(bool testExists = false);
}