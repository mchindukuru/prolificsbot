
namespace ProlificsBot.Bots
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using ProlificsBot.Models;
    using ProlificsBot.Services;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class ActivityBot : ActivityHandler
    {
        private readonly ConcurrentDictionary<string, ReleaseManagementTask> taskDetails;
        private readonly ICardFactory cardFactory;

        public ActivityBot (ConcurrentDictionary<string, ReleaseManagementTask> taskDetails, ICardFactory cardFactory)
        {
            this.taskDetails = taskDetails;
            this.cardFactory = cardFactory;
        }

        /// <summary>
        /// Invoked when members are added/removed from the conversation.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var isPresent = taskDetails.TryGetValue(Constant.TaskDetails, out ReleaseManagementTask releaseManagementTask);
            var welcomeText = releaseManagementTask.TaskTitle + " and Assigned to " + releaseManagementTask.AssignedToName;
            if (isPresent && turnContext.Activity.MembersAdded.Count > 0)
            {
                var adaptiveAttachment = this.cardFactory.CreateAdaptiveCardAttachement(Path.Combine(".", "Resources", "WorkitemCardTemplate.json"), releaseManagementTask);
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
            } else {
                var welcomeText1 = "Hello, Please use valid question";
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText1, welcomeText1), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
