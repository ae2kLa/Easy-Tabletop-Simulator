using Mirror;

namespace Tabletop.Online
{
    public class OnlineGoChessBasket : OnlineContainerObj
    {
        public GoChessColor ContainGoChessColor = GoChessColor.Unknown;

        protected override void AddContainTypes()
        {
            ContainTypes.Add(typeof(OnlineGoChessPiece));
        }

        protected override bool AddCondition(OnlineDragObject dragObj)
        {
            if (dragObj is not OnlineGoChessPiece)
            {
                return false;
            }
            else
            {
                OnlineGoChessPiece piece = dragObj as OnlineGoChessPiece;
                return piece.VirtualColor == ContainGoChessColor;
            }
        }

        [Server]
        protected override void AfterGenerate(OnlineDragObject dragObj)
        {
            if (dragObj is OnlineGoChessPiece)
            {
                OnlineGoChessPiece piece = dragObj as OnlineGoChessPiece;
                piece.VirtualColor = ContainGoChessColor;
            }
        }

        [Server]
        protected override bool CheckHandleAddition(uint playerNid)
        {
            bool res = false;
            OnlinePlayerManager.Instance.ForEach((player) =>
            {
                if (player.netId == playerNid && player.CurrentColor == ContainGoChessColor)
                    res = true;
            });
            return res;
        }
    }

}
