using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GoChessColor m_currentColor;

    /// <summary>
    /// 当前玩家对应哪一黑子还是白子
    /// </summary>
    public GoChessColor CurrentColor => m_currentColor;

    public override void OnStartClient()
    {
        if (isClient)
        {
            CmdPlayerAdd();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayerAdd()
    {
        PlayManager.Instance.Add(this);
        this.m_currentColor = GoChessColor.Black;
    }

    public override void OnStopClient()
    {
        if (isClient)
        {
            CmdPlayerRemove();
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdPlayerRemove()
    {
        PlayManager.Instance.Remove(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isLocalPlayer)
        {
            print("LocalPlayerNid:" + netId);
        }
    }

    [TargetRpc]
    public void TargetSendMsg(NetworkConnectionToClient targetConn, string msg)
    {
        Debug.Log(msg);
    }
}
