using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain;
using MediatR;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;
using Application.Core;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : BaseApiController
    {

        [HttpGet] // api/activities
        public async Task<IActionResult> GetActivities([FromQuery] ActivityParams param)
        {
            var result =  await Mediator.Send(new List.Query { Params = param });

            return HandlePagedResult(result);
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

        [Authorize(Policy = "IsActivityHost")] // Adding the Policy to the Edit Controller where we want to use it
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            // Add the Id to the activity object
            activity.Id = id;

            // Send a request to the mediator to edit the activity
            await Mediator.Send(new EditActivity.Command { Activity = activity });

            return Ok();
        }

        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            // Send a request to the Mediator to handle the delete of the activty
            await Mediator.Send(new DeleteActivity.Command { Id = id });

            return Ok();
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendace.Command { Id = id }));
        }
    }
}
 