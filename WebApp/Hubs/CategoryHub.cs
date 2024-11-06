using COCOApp.Helpers;
using COCOApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace COCOApp.Hubs
{
    public class CategoryHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Session.GetCustomObjectFromSession<User>("user");
            if (user != null && user.Role == 1)//Admin
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Session.GetCustomObjectFromSession<User>("user");
            if (user != null && user.Role == 1)//Admin
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
