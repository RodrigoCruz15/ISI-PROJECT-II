using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.Entities
{
    public class SensorReading
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public decimal Value { get; set; }
        public DateTime Timestamp  { get; set; }
        public bool TriggeredAlert { get; set; } = false;

    }
}
