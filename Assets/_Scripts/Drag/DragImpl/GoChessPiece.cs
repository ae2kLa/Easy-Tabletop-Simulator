using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoChessColor
{
    White = 0,
    Black
}

public class GoChessPiece : DragObject
{
    //黑白两方
    public BindableProperty<GoChessColor> VirtualColor;

    public Material WhiteMaterial;
    public Material BlackMaterial;

    protected override void Init()
    {
        print("Piece Init");
        base.Init();

        VirtualColor = new BindableProperty<GoChessColor>();
        VirtualColor.RegisterWithInitValue((virtualColor) =>
        {
            //修改为对应材质
            if(virtualColor == GoChessColor.White)
            {
                transform.Find("model").GetComponent<MeshRenderer>().material = WhiteMaterial;
            }
            else
            {
                transform.Find("model").GetComponent<MeshRenderer>().material = BlackMaterial;
            }
        });
    }

    protected override bool CheckHandleAddition(uint playerNid)
    {
        bool res = false;
        PlayerManager.Instance.ForEach((player) =>
        {
            if (player.netId == playerNid && player.CurrentColor == VirtualColor.Value)
                res = true;
        });
        return res;
    }


}
