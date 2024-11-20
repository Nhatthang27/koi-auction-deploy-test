using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.ScheduledTask
{
    public class ScheduledTask
    {
        public DateTime ExecuteAt { get; set; }
        public Func<Task>? Task { get; set; }
        public Action? Action { get; set; }
    }
}