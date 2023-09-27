using Mirror;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoChessColor
{
    White = 0,
    Black,
    Unknown
}

public class GoChessPiece : DragObject
{
    //黑白两方
    [SyncVar]
    public GoChessColor VirtualColor;

    public EasyEvent<GoChessColor> ColorChange;

    public Material WhiteMaterial;
    public Material BlackMaterial;

    protected override void Init()
    {
        base.Init();

        VirtualColor = GoChessColor.Unknown;
        ColorChange = new EasyEvent<GoChessColor>();
    }

    [ClientRpc]
    public void RpcColorChange(GoChessColor virtualColor)
    {
        //修改为对应材质
        if (virtualColor == GoChessColor.White)
        {
            transform.Find("model").GetComponent<MeshRenderer>().material = WhiteMaterial;
        }
        else
        {
            transform.Find("model").GetComponent<MeshRenderer>().material = BlackMaterial;
        }
    }

    protected override bool CheckHandleAddition(uint playerNid)
    {
        bool res = false;
        PlayManager.Instance.ForEach((player) =>
        {
            if (player.netId == playerNid && player.CurrentColor == VirtualColor)
                res = true;
        });
        return res;
    }


}
