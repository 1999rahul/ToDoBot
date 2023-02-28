using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoBot.Models;

namespace ToDoBot.Dialogs.Operations
{
    public class DeleteTaskDialog: ComponentDialog
    {
        public DeleteTaskDialog() : base(nameof(DeleteTaskDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                TasksStepAsync,
                ActStepAsync,
                //DeleteMoreTasksStepAsync,
                SummaryStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            //AddDialog(new DeleteTaskDialog());

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> TasksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter the task to Delete.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            var userDetails = (User)stepContext.Options;
            var itemToRemove = userDetails.TasksList.SingleOrDefault(r => r == (string)stepContext.Result);
            if (itemToRemove != null)
            {
                userDetails.TasksList.Remove(itemToRemove);

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Task Removed")
                }, cancellationToken);
            }
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Task Not Found")
            }, cancellationToken);

            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
           
            if(userDetails.TasksList.Count> 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Here are your Remaning Tasks"));
                foreach (var item in userDetails.TasksList)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(item), cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Task List is Empthy"));
            }
            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }

        /*private async Task<DialogTurnResult> DeleteMoreTasksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            if ((bool)stepContext.Result)
            {
                return await stepContext.BeginDialogAsync(nameof(DeleteTaskDialog), userDetails, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userDetails, cancellationToken);
            }
        }*/

        

        
    }
}
