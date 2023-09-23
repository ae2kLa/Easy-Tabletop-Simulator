using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoChessBasket : ContainerObj<GoChessPiece>
{
    public GoChessColor ContainGoChessColor = GoChessColor.White;

    protected override void AddContainTypes()
    {
        ContainTypes.Add(typeof(GoChessPiece));
    }

    protected override bool AddCondition(DragObject dragObj)
    {
        if(dragObj is not GoChessPiece)
        {
            return false;
        }
        else
        {
            GoChessPiece piece = dragObj as GoChessPiece;
            return piece.VirtualColor.Value == ContainGoChessColor;
        }
    }

    protected override void AfterGenerateHandler(DragObject dragObj)
    {
        if (dragObj is GoChessPiece)
        {
            GoChessPiece piece = dragObj as GoChessPiece;
            piece.VirtualColor.Value = ContainGoChessColor;
        }
    }
}
