using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.Scheduled
{
    public interface IInventoryAutoSupplying : Orleans.IGrainWithGuidKey
    {
        Task Start();
        Task Stop();
    }
}
