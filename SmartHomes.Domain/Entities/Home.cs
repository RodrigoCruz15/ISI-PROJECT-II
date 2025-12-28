using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.Entities
{
    public class Home
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Area { get; set; } // em m2
        public HomeStatus Status { get; set; } = HomeStatus.Active; //Active, Inactive, Maintenance
        public DateTime CreatedAt { get; set; }
    }
}
