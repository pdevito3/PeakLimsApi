namespace PeakLims.Domain.Samples.Mappings;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples;
using Mapster;

public sealed class SampleMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Sample, SampleDto>();
        config.NewConfig<ContainerlessSampleForCreationDto, Sample>()
            .TwoWays();
        config.NewConfig<ContainerlessSampleForUpdateDto, Sample>()
            .TwoWays();
        config.NewConfig<ContainerlessSampleForCreationDto, SampleForCreationDto>()
            .TwoWays();
        config.NewConfig<ContainerlessSampleForUpdateDto, SampleForUpdateDto>()
            .TwoWays();
    }
}