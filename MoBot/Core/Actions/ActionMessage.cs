namespace MoBot.Core.Actions
{
    public class ActionMessage : SysAction
    {
        public string Message;
        internal override void HandleAction(IActionHandler handler)
        {
            handler.HandleMessage(this);
        }
    }
}
