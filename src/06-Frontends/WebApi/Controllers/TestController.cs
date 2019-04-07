using GrainInterfaces.TestStates;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private IClusterClient _orleansClient;

        public TestsController(IClusterClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync()
        {
            var test = _orleansClient.GetGrain<ITestState>(Guid.Empty);
            var prev = await test.GetState();
            await test.Create(new TestState()
            {
                Id = Guid.NewGuid().ToString(),
                AGuid = Guid.NewGuid(),
                ADecimal = 123.45M,
                ADateTimeOffset = DateTime.Now,
                ADictionary = new Dictionary<Guid, TestState>()
                {
                    {
                        Guid.NewGuid(),
                        new TestState()
                        {
                            Id = Guid.NewGuid().ToString(),
                            AGuid = Guid.NewGuid(),
                            ADecimal = 123.45M,
                            ADateTimeOffset = DateTime.Now,
                        }
                    }
                }
            });

            return Ok();
        }

    }

}
