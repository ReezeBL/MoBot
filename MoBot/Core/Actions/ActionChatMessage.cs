namespace MoBot.Core.Actions
{
    public class ActionChatMessage : SysAction
    {
        public string JsonMessage;

        internal override void HandleAction(IActionHandler handler)
        {
            handler.HandleChatMessage(this);
        }
    }
}
