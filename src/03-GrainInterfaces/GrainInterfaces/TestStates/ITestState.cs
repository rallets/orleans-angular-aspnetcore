using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.TestStates
{
    public interface ITestState : Orleans.IGrainWithGuidKey
    {
        Task<TestState> Create(TestState testState);
        Task<TestState> GetState();
    }
}
