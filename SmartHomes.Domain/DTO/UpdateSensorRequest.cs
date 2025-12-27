using SmartHomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.DTO
{
    public class UpdateSensorRequest
    {

        public string? Name { get; set; } // Novo nome/localização do sensor

        public SensorTypeEnum? Type { get; set; }

        public UnitEnum? Unit { get; set; }

        public bool? IsActive { get; set; }
    }
}
