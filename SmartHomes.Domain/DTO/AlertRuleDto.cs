using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.DTO
{
    public class AlertRuleDto
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public AlertConditionEnum Condition { get; set; }
        public decimal Threshold { get; set; }
        public AlertSeverityEnum Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
