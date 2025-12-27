using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.DTO
{
    public class SensorDto
    {
        public Guid Id { get; set; }
        public Guid HomeId { get; set; }
        public SensorTypeEnum Type { get; set; }
        public UnitEnum Unit { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastReadingAt { get; set; }
    }
}
