using System.Collections.Generic;

namespace PandemicConsoleApp
{
    internal class Action : IAction
    {
        #region Public Fields

        public ActionType ActionType;
        public List<int> Cost = new List<int>();

        #endregion Public Fields
    }
}