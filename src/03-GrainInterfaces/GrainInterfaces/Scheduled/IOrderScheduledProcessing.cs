using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Scheduled
{
    public interface IOrderScheduledProcessing : Orleans.IGrainWithGuidKey
    {
        Task Start();
        Task Stop();
    }
}
