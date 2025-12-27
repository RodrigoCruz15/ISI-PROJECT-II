using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.DTO
{
    public class AlertDto
    {

        public Guid Id { get; set; }
        public Guid AlertRuleId { get; set; }
        public Guid SensorReadingId { get; set; }
        public Guid SensorId { get; set; }
        public decimal Value { get; set; }
        public decimal Threshold { get; set; }
        public AlertSeverityEnum Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
    }
}
