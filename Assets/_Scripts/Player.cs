using Mirror;
using Tabletop;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private GoChessColor m_currentColor = GoChessColor.Unknown;

    /// <summary>
    /// 当前玩家对应哪一黑子还是白子
    /// </summary>
    public GoChessColor CurrentColor
    {
        set { m_currentColor = value; }
        get { return m_currentColor; }
    }

    public override void OnStartServer()
    {
        PlayerManager.Instance.Add(this);
    }

    [Client]
    public override void OnStartClient()
    {
        NetworkClient.ReplaceHandler<OppositeExitMessage>(OnOppositeExit, false);
    }

    [ClientCallback]
    private void OnOppositeExit(OppositeExitMessage msg)
    {
        print(msg.MsgContent + "按B键StopClient");
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("LocalPlayerNid:" + netId);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerManager.Instance.RestartGame();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            NetworkManager.singleton.StopClient();
        }
    }

    [TargetRpc]
    public void TargetSendMsg(NetworkConnectionToClient targetConn, string msg)
    {
        Debug.Log(msg);
    }
}
