using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Dnugbb.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var options = new string[]
            {
                "Anstehende Treffen",
                "Anmelden zu einem Treffen",
                "Organisator(en) kontaktieren"
            };
    
            var descriptions = new string[]
            {
                "Anstehende Treffen",
                "Anmelden zu einem Treffen",
                "Organisator(en) kontaktieren"
            };

            var message = context.MakeMessage();
            message.Text = "Bei folgenden Aktivitäten kann ich Dir helfen:";
            message.Speak = "Bei folgenden Aktivitäten kann ich Dir helfen:";
            await context.PostAsync(message);

            var possibleActivitiesMessage = context.MakeMessage();
            possibleActivitiesMessage.AddHeroCard(string.Empty, options, descriptions);
            await context.PostAsync(possibleActivitiesMessage);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (string.IsNullOrEmpty(activity.Text))
            {
                context.Done(string.Empty);
            }
            else if (activity.Text.ToLower().Contains("anstehende treffen"))
            {
                await context.Forward(new NextMeetupsDialog(), ResumeAfterNextMeetupDialog, activity);
            }
            else if (activity.Text.ToLower().Contains("anmelden zu einem treffen"))
            {
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text.ToLower().Contains("organisator(en) kontaktieren"))
            {
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.Done(string.Empty);
            }
        }

        private async Task ResumeAfterNextMeetupDialog(IDialogContext context, IAwaitable<object> result)
        {
            var userActivity = await result as Activity;

            context.Done(string.Empty);
        }
    }
}