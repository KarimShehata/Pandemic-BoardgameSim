using System;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class PassAction : Action
    {
        public PassAction()
        {
            ActionType = ActionType.Pass;
        }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Pass.");
        }
    }
}