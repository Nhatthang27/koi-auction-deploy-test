using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuctionService.Dto.ScheduledTask;

namespace AuctionService.IServices
{
    public interface ITaskSchedulerService
    {
        Guid ScheduleTask(ScheduledTask task);
        void CancelScheduledTask(Guid taskId);
    }
}