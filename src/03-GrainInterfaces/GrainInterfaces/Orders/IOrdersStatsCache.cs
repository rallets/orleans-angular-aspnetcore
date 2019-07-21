using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces.Orders
{
    public interface IOrdersStatsCache : IGrainWithGuidKey
    {
        Task<OrdersStats> GetAsync();
    }
}
