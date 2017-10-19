using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Dnugbb.Bot.iCal;
using System.Linq;
using System.Collections.Generic;

namespace Dnugbb.Bot.Dialogs
{
    [Serializable]
    public class NextMeetupDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();


            var client = new HttpClient();
            var iCal = await client.GetStringAsync("https://dnugbb.azurewebsites.net/events.ics");
            var calendar = new vCalendar(iCal);

            foreach (var meetup in calendar.vEvents)
            {
                DateTime start;
                string startTime = string.Empty;

                if (DateTime.TryParse(meetup.ContentLines.Where(line => line.Key.ToLower() == "dtstart").FirstOrDefault().Value.Value, out start))
                    startTime = start.ToString("dd.MM.yyyy hh:mm");

                var heroCard = new HeroCard()
                {
                    Title = meetup.ContentLines.Where(line => line.Key.ToLower() == "summary").FirstOrDefault().Value.Value,
                    Subtitle = startTime
                                + " | "
                                + meetup.ContentLines.Where(line => line.Key.ToLower() == "location").FirstOrDefault().Value.Value,
                    Text = meetup.ContentLines.Where(line => line.Key.ToLower() == "description").FirstOrDefault().Value.Value,

                    //Images = new List<CardImage>
                    //{
                    //    new CardImage(nextEvent.SpeakerImage)
                    //},

                    Buttons = new List<CardAction>
                    {
                        new CardAction(type: ActionTypes.OpenUrl, title: "Jetzt anmelden", value: meetup.ContentLines.Where(line => line.Key.ToLower() == "url").FirstOrDefault().Value.Value)
                    }
                };

                message.Attachments.Add(heroCard.ToAttachment());
            }



            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Contains("exit"))
            {
                context.Done(string.Empty);
                return;
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}