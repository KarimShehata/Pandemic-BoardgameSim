using PandemicConsoleApp.Actions;

namespace PandemicConsoleApp
{
    internal class ShareKnowledgeAction : Action
    {
        #region Public Constructors

        public ShareKnowledgeAction(Player giving, Player receiving, int card)
        {
            ActionType = ActionType.ShareKnowledge;
            GivingPlayer = giving;
            ReceivingPlayer = receiving;
            LocationCard = card;
        }

        public int LocationCard { get; set; }

        public Player ReceivingPlayer { get; set; }

        public Player GivingPlayer { get; set; }

        #endregion Public Constructors
    }
}