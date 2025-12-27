using SmartHomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.DTO
{
    public class UpdateAlertRuleRequest
    {
        public string? Name { get; set; }
        public AlertConditionEnum? Condition { get; set; }
        public decimal? Threshold { get; set; }
        public AlertSeverityEnum? Severity { get; set; }
        public string? Message { get; set; }
        public bool? IsActive { get; set; }

    }
}
