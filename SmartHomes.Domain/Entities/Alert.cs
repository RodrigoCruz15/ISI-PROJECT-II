using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.Entities
{
    public class Alert
    {
        public Guid Id { get; set; }
        public Guid AlertRuleId { get; set; }
        public Guid SensorReadingId { get; set; }
        public Guid SensorId { get; set; }
        public decimal Value { get; set; } /// Valor da leitura que disparou o alerta
        public decimal Threshold { get; set; }
        public AlertSeverityEnum Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public bool IsAcknowledged { get; set; } = false;
        public DateTime? AcknowledgedAt { get; set; }
    }
}
