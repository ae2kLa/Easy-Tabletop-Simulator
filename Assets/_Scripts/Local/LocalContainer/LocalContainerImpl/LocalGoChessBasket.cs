using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGoChessBasket : LocalContainerObj
    {
        public GoChessColor ContainGoChessColor = GoChessColor.Unknown;
        [HideInInspector] public GoChessColor PlayerGoChessColor = GoChessColor.Unknown;

        protected override void AddContainTypes()
        {
            ContainTypes.Add(typeof(LocalGoChessPiece));
        }

        protected override bool AddCondition(LocalDragObj dragObj)
        {
            if (dragObj is not LocalGoChessPiece)
            {
                return false;
            }
            else
            {
                LocalGoChessPiece piece = dragObj as LocalGoChessPiece;
                return piece.VirtualColor == ContainGoChessColor;
            }
        }

        protected override void AfterGenerate(LocalDragObj dragObj)
        {
            if (dragObj is LocalGoChessPiece)
            {
                LocalGoChessPiece piece = dragObj as LocalGoChessPiece;
                piece.VirtualColor = ContainGoChessColor;
            }
        }

        protected override bool CheckHandleAddition(GoChessColor color)
        {
            return color == ContainGoChessColor;
        }
    }

}
