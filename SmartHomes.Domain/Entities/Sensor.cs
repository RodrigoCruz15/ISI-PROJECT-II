using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.Entities
{
    /// <summary>
    /// Representa um sensor IoT instalado numa casa
    /// </summary>
    public class Sensor
    {

        public Guid Id { get; set; }
        public Guid HomeId { get; set; }
        public SensorTypeEnum Type { get; set; } = SensorTypeEnum.Unknown; // inicializa em estado desativado
        public  UnitEnum Unit { get; set; } = UnitEnum.Unknown; // inicializa em estado desativado
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastReadingAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
