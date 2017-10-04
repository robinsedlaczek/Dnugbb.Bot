using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;

namespace Dnugbb.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded.Count == 0)
                    return null;

                var message = $@"
Hi {activity.MembersAdded[0].Name}!{Environment.NewLine}
Ich bin der Bot der .NET User Group Berlin-Brandenburg. Was kann ich für Dich tun?{Environment.NewLine}";

                var reply = activity.CreateReply(message);
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.SendToConversationAsync(reply);
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}