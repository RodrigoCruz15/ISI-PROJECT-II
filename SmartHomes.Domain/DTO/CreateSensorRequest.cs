using SmartHomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.DTO
{
    public class CreateSensorRequest
    {
        public Guid HomeId { get; set; }
        public SensorTypeEnum Type { get; set; } // Tipo do sensor
        public UnitEnum Unit { get; set; } // Unidade de medida do sensor
        public string Name { get; set; } = string.Empty; // Nome/Localização do sensor
    }
}
