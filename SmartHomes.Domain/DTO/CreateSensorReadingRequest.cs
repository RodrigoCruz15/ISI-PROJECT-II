using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.DTO
{
    public class CreateSensorReadingRequest
    {
        public Guid SensorId { get; set; }
        public decimal Value { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
