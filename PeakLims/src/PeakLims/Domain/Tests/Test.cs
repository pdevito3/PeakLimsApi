namespace PeakLims.Domain.Tests;

using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Validators;
using PeakLims.Domain.Tests.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using Sieve.Attributes;
using PeakLims.Domain.Panels;
using Services;
using TestStatuses;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class Test : BaseEntity
{
    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string TestCode { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string TestName { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Methodology { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual string Platform { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual int Version { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public virtual int TurnAroundTime { get; private set; }

    public virtual TestStatus Status { get; private set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<Panel> Panels { get; private set; }


    public static Test Create(TestForCreationDto testForCreationDto, ITestRepository testRepository)
    {
        new TestForCreationDtoValidator().ValidateAndThrow(testForCreationDto);
        GuardWhenExists(testForCreationDto.TestCode, testForCreationDto.Version, testRepository);

        var newTest = new Test();
        
        newTest.TestCode = testForCreationDto.TestCode;
        newTest.TestName = testForCreationDto.TestName;
        newTest.TurnAroundTime = testForCreationDto.TurnAroundTime;
        newTest.Methodology = testForCreationDto.Methodology;
        newTest.Platform = testForCreationDto.Platform;
        newTest.Version = testForCreationDto.Version;
        newTest.Status = TestStatus.Draft();

        newTest.QueueDomainEvent(new TestCreated(){ Test = newTest });
        
        return newTest;
    }

    public static void GuardWhenExists(string testCode, int version, ITestRepository testRepository)
    {
        if (Exists(testCode, version, testRepository))
            throw new ValidationException(nameof(Test),
                $"A test with the given test code ('{testCode}') and version ('{version}') already exists.");
    }

    public static bool Exists(string testCode, int version, ITestRepository testRepository) => testRepository.Exists(testCode, version);

    public void Update(TestForUpdateDto testForUpdateDto, ITestRepository testRepository)
    {
        new TestForUpdateDtoValidator().ValidateAndThrow(testForUpdateDto);
        GuardWhenExists(TestCode, testForUpdateDto.Version, testRepository);
        
        TestName = testForUpdateDto.TestName;
        TurnAroundTime = testForUpdateDto.TurnAroundTime;
        Methodology = testForUpdateDto.Methodology;
        Platform = testForUpdateDto.Platform;
        Version = testForUpdateDto.Version;

        QueueDomainEvent(new TestUpdated(){ Id = Id });
    }

    public Test Activate()
    {
        if (Status == TestStatus.Active())
            return this;
        
        Status = TestStatus.Active();
        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }

    public Test Deactivate()
    {
        if (Status == TestStatus.Inactive())
            return this;
        
        Status = TestStatus.Inactive();
        QueueDomainEvent(new TestUpdated(){ Id = Id });
        return this;
    }
    
    protected Test() { } // For EF + Mocking
}