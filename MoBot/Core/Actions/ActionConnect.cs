namespace MoBot.Core.Actions
{
    internal class ActionConnect : SysAction
    {
        public bool Connected;
        internal override void HandleAction(IActionHandler handler)
        {
            handler.HandleConnect(this);
        }
    }
}
