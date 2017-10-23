using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Dnugbb.Bot.iCal;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Dnugbb.Bot.Dialogs
{
    [Serializable]
    public class NextMeetupsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();


            var client = new HttpClient();
            var iCal = await client.GetStringAsync("https://dnugbb.azurewebsites.net/events.ics");
            var calendar = new vCalendar(iCal);

            foreach (var meetup in calendar.vEvents)
            {
                DateTime startTime;
                var startTimestamp = meetup.ContentLines.Where(line => line.Key.ToLower() == "dtstart").FirstOrDefault().Value.Value;

                if (!DateTime.TryParseExact(startTimestamp, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out startTime))
                    continue;

                var meetupUrl = meetup.ContentLines.Where(line => line.Key.ToLower() == "url").FirstOrDefault().Value.Value;

                var title = meetup.ContentLines.Where(line => line.Key.ToLower() == "summary").FirstOrDefault().Value.Value;

                var description = meetup.ContentLines.Where(line => line.Key.ToLower() == "description").FirstOrDefault().Value.Value;

                var metadata = description.Split('\n');

                var excerpt = string.Join("\r\n \r\n", metadata.TakeWhile(entry => !entry.StartsWith("Referent:") && !entry.StartsWith("Image:")));

                var referent = metadata.Where(entry => entry.StartsWith("Referent:")).FirstOrDefault();

                if (!string.IsNullOrEmpty(referent))
                    referent = referent.Substring("Referent:".Length).Trim();

                var imageUrl = metadata.Where(entry => entry.StartsWith("Image:")).FirstOrDefault();

                if (!string.IsNullOrEmpty(imageUrl))
                    imageUrl = imageUrl.Substring("Image:".Length).Trim();

                var subTitle =
                    referent
                    + " | " + startTime.ToString("dd.MM.yyyy hh:mm")
                    + " | " + meetup.ContentLines.Where(line => line.Key.ToLower() == "location").FirstOrDefault().Value.Value;

                var heroCard = new HeroCard()
                {
                    Title = title,
                    Subtitle = subTitle,
                    Text = excerpt,
                    
                    Images = new List<CardImage>
                    {
                        new CardImage(imageUrl)
                    },

                    Buttons = new List<CardAction>
                    {
                        new CardAction(type: ActionTypes.OpenUrl, title: "Zur Event-Website", value: meetupUrl)
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
            var reply = activity.CreateReply("Was kann ich noch für Dich tun?");

            await context.PostAsync(reply);

            // [RS] Nothing to do currently.
            context.Done(string.Empty);

            //context.Wait(MessageReceivedAsync);
        }
    }
}