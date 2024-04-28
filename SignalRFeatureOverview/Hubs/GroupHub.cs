using Microsoft.AspNetCore.SignalR;

namespace SignalRFeatureOverview.Hub
{
    public class GroupHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public Task Join() => Groups.AddToGroupAsync(Context.ConnectionId, "group_name");

        public Task Leave() => Groups.RemoveFromGroupAsync(Context.ConnectionId, "group_name");

        public Task Message() => Clients
            .Groups("group_name")
            .SendAsync("group_message", new Data(69, "secret group message"));

        //public Task MessageWithGroupName(string groupName) => Clients
        //                                     .Groups(groupName)
        //                                     .SendAsync("group_message", new Data(69, "secret group message"));
        public async Task MessageWithGroupName(string groupName, string message)
        {
            await Clients.Groups(groupName).SendAsync("group_message", message);
        }

        public Task JoinRole1() => Groups.AddToGroupAsync(Context.ConnectionId, "Role1");
        public Task JoinRole2() => Groups.AddToGroupAsync(Context.ConnectionId, "Role2");
        public Task JoinRole3() => Groups.AddToGroupAsync(Context.ConnectionId, "Role3");

        public async Task JoinGroup(string rolename) {
            if (rolename == "Role1") {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Role1");
            }
            if (rolename == "Role2")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Role2");
            }
            if (rolename == "Role3") {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Role3");
            }
        }
    }
}
