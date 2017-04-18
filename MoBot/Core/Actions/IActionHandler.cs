namespace MoBot.Core.Actions
{
    internal interface IActionHandler
    {
        void HandleChatMessage(ActionChatMessage action);
        void HandleConnect(ActionConnect action);
        void HandleMessage(ActionMessage action);
    }
}