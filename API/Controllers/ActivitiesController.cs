using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain;
using MediatR;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : BaseApiController
    {

        [HttpGet] // api/activities
        public async Task<IActionResult> GetActivities()
        {
            var result =  await Mediator.Send(new List.Query());

            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("{Id:Guid}")]
        public async Task<IActionResult> GetActivity(Guid Id)
        {
            var result = await Mediator.Send(new Details.Query { Id = Id });

            return HandleResult(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            var result = await Mediator.Send(new CreateActivity.Command { Activity = activity });

            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            await Mediator.Send(new EditActivity.Command { Activity = activity });

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            await Mediator.Send(new DeleteActivity.Command { Id = id });

            return Ok();
        }
    }
}
 