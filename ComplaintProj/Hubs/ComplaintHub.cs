using Microsoft.AspNetCore.SignalR;

namespace ComplaintProj.Hubs
{
    public class ComplaintHub :Hub
    {
        public async Task JoinServicesGroup()
        {
            if (Context.User != null && Context.User.IsInRole("PatientServices"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "PatientServicesGroup");

            }
            else if  (Context.User != null && Context.User.IsInRole("HealthcareProvider"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "HealthcareProviderGroup");

            }
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveComplaintToast", message);
        }
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }
}
