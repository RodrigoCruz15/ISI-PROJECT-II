using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHomes.Domain.Enums;

namespace SmartHomes.Domain.DTO
{
    public class HomeDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Area { get; set; }
        public HomeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
