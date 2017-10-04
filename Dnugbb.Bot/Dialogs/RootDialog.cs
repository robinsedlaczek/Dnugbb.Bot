using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Dnugbb.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            var options = new string[]
            {
                "Anstehende Treffen",
                "Anmelden zu einem Treffen"
            };

            var descriptions = new string[]
            {
                "Anstehende Treffen",
                "Anmelden zu einem Treffen"
            };

            var message = context.MakeMessage();
            message.AddHeroCard("Bei folgenden Aktivitäten kann ich Dir helfen:", options, descriptions);
                
            context.PostAsync(message);

            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply(activity.Text);

            //await context.PostAsync(reply);
            context.Done(activity.Text);
            
            //context.Wait(MessageReceivedAsync);
        }
    }
}