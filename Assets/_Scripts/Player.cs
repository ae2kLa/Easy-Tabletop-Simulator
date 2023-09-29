using Mirror;
using Tabletop;
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
        NetworkClient.RegisterHandler<OppositeExitMessage>(OnOppositeExit);
    }

    [ClientCallback]
    private void OnOppositeExit(OppositeExitMessage msg)
    {
        print(msg.MsgContent);
        //NetworkClient.Disconnect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isLocalPlayer)
        {
            print("LocalPlayerNid:" + netId);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerManager.Instance.RestartGame();
        }
    }

    [TargetRpc]
    public void TargetSendMsg(NetworkConnectionToClient targetConn, string msg)
    {
        Debug.Log(msg);
    }
}
