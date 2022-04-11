using AutoMapper;
using Xunit;

namespace Monq.Sm.Service.Sla.Tests
{
    public class AutoMapperTests
    {
        [Fact(DisplayName = "[AutoMapper] Проверка привязки профилей.")]
        public void ShouldProperlyMapProfiles()
        {
            new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Program));
            }).AssertConfigurationIsValid();
        }
    }
}