using PandemicConsoleApp.Actions;

namespace PandemicConsoleApp
{
    internal abstract class MovementAction : Action
    {
        #region Public Fields

        public int Destiantion;

        #endregion Public Fields

        public abstract override void PrintAction(int i);
    }
}