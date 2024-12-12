using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Method that sends the comment to the Hub
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SendComment(Create.Command command)
        {
            // Send the request to mediator to add the comment
            var comment = await _mediator.Send(command);

            // Send the Comment tp all the connected people in the hub including the person that created the comment
            // The Method will be named as RecieveComment that the client will use
            // Client should pass Value of the comment as a parameter
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("RecieveComment", comment.Value);
        }
            
        public override async Task OnConnectedAsync()
        {
            // Get the HttpContext from SignalR Attributes
            var httpContext = Context.GetHttpContext();

            // Get the activity Id from the query String of the request
            var activityId = httpContext.Request.Query["activityId"];

            // Add the Client (Connect) to the group with the ActivityId given in the request
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);

            // Send back the list of comments from that belongs to the activity
            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });

            // Method that client will call is: LoadComments passing a value
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}
