using PandemicConsoleApp.Actions;

namespace PandemicConsoleApp
{
    internal class PassAction : Action
    {
        public PassAction()
        {
            ActionType = ActionType.Pass;
        }
    }
}