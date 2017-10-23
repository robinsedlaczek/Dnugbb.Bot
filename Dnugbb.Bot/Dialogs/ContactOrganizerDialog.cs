using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Dnugbb.Bot.Dialogs
{
    [Serializable]
    public class ContactOrganizerDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            
            context.PostAsync(message);

            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //var reply = activity.CreateReply("Was kann ich noch für Dich tun?");

            //await context.PostAsync(reply);

            // [RS] Nothing to do currently.
            //context.Done(string.Empty);

            context.Wait(MessageReceivedAsync);
        }
    }
}