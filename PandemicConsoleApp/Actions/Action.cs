using System.Collections.Generic;

namespace PandemicConsoleApp.Actions
{
    internal abstract class Action : IAction
    {
        #region Public Fields

        public ActionType ActionType { get; set; }
        public List<int> Cost = new List<int>();

        #endregion Public Fields

        public abstract void PrintAction();
    }
}