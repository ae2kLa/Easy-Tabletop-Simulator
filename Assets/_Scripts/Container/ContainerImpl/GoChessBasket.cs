using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoChessBasket : ContainerObj
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
            return piece.VirtualColor == ContainGoChessColor;
        }
    }

    [Server]
    protected override void AfterGenerate(DragObject dragObj)
    {
        if (dragObj is GoChessPiece)
        {
            GoChessPiece piece = dragObj as GoChessPiece;
            piece.VirtualColor = ContainGoChessColor;
            piece.RpcColorChange(ContainGoChessColor);
        }
    }


    [Server]
    protected override bool CheckHandleAddition(uint playerNid)
    {
        bool res = false;
        PlayManager.Instance.ForEach((player) =>
        {
            if (player.netId == playerNid && player.CurrentColor == ContainGoChessColor)
                res = true;
        });
        return res;
    }
}
