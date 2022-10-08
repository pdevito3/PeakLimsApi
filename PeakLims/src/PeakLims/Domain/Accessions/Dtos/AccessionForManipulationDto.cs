namespace PeakLims.Domain.Accessions.Dtos;

public abstract class AccessionForManipulationDto 
{
        public string AccessionNumber { get; set; }
        public string State { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? HealthcareOrganizationId { get; set; }

}