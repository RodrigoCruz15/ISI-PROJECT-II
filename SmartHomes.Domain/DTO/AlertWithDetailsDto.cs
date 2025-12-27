using SmartHomes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomes.Domain.DTO
{
    public class AlertWithDetailsDto
    {
        // Dados do alerta
        public Guid AlertId { get; set; }
        public decimal Value { get; set; }
        public decimal Threshold { get; set; }
        public AlertSeverityEnum Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public bool IsAcknowledged { get; set; }

        // Detalhes do sensor
        public Guid SensorId { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public SensorTypeEnum SensorType { get; set; }
        public UnitEnum Unit { get; set; }

        // Detalhes da casa
        public Guid HomeId { get; set; }
        public string HomeName { get; set; } = string.Empty;

        // Detalhes da regra
        public string RuleName { get; set; } = string.Empty;

    }
}
