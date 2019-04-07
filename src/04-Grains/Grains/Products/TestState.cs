using GrainInterfaces.TestStates;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using System.Threading.Tasks;

namespace OrleansSilo.TestStates
{
    [StorageProvider(ProviderName = "TableStore")]
    public class TestStateGrain : Grain<TestState>, ITestState
    {
        private readonly ILogger _logger;

        public TestStateGrain(ILogger<TestStateGrain> logger)
        {
            _logger = logger;
        }

        async Task<TestState> ITestState.Create(TestState TestState)
        {
            State = TestState;
            await base.WriteStateAsync();
            return this.State;
        }

        Task<TestState> ITestState.GetState()
        {
            return Task.FromResult(this.State);
        }
    }
}
