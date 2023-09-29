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
    public GoChessColor m_virtualColor;
    public GoChessColor VirtualColor
    {
        set
        {
            m_virtualColor = value;
            RpcColorChange(m_virtualColor);
        }
        get { return m_virtualColor; }
    }

    public EasyEvent<GoChessColor> ColorChange;

    public Material WhiteMaterial;
    public Material BlackMaterial;

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSyncState();
    }

    [Command(requiresAuthority = false)]
    public void CmdSyncState()
    {
        RpcColorChange(m_virtualColor);
    }

    [Server]
    protected override void Init()
    {
        base.Init();

        VirtualColor = GoChessColor.Unknown;
        ColorChange = new EasyEvent<GoChessColor>();
    }

    [ClientRpc]
    private void RpcColorChange(GoChessColor virtualColor)
    {
        //修改为对应材质
        if (virtualColor == GoChessColor.White)
        {
            transform.Find("model").GetComponent<MeshRenderer>().material = WhiteMaterial;
        }
        else if (virtualColor == GoChessColor.Black)
        {
            transform.Find("model").GetComponent<MeshRenderer>().material = BlackMaterial;
        }
        else
        {
            print("棋子颜色未知");
        }
    }

    [Server]
    protected override bool CheckHandleAddition(uint playerNid)
    {
        bool res = false;
        PlayerManager.Instance.ForEach((player) =>
        {
            if (player.netId == playerNid && player.CurrentColor == VirtualColor)
                res = true;
        });
        return res;
    }


}
