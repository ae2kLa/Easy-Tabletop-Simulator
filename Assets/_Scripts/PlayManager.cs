using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 仅服务器管理
/// </summary>
public class PlayManager : NetworkBehaviour
{
    public static PlayManager Instance => m_instance;
    public static PlayManager m_instance = null;

    private List<Player> m_players;

    public override void OnStartServer()
    {
        if(m_instance == null)
        {
            m_instance = this;
            m_players = new List<Player>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Add(Player player)
    {
        m_players.Add(player);
        Debug.Log($"检测到玩家连接，Nid:{player.netId}");
    }

    public void Remove(Player player)
    {
        m_players.Remove(player);
        Debug.Log($"检测到玩家退出，Nid:{player.netId}");
    }

    public void ForEach(UnityAction<Player> callback)
    {
        foreach(Player player in m_players)
        {
            callback(player);
        }
    }

    public void SendMsg(uint playerNid, string msg)
    {
        ForEach((player) =>
        {
            if (player.netId == playerNid)
            {
                player.TargetSendMsg(player.connectionToClient, msg);
            }
        });
    }

    public void SendAllMsg(string msg)
    {
        ForEach((player) =>
        {
            player.TargetSendMsg(player.connectionToClient, msg);
        });
    }

    public NetworkConnectionToClient GetConn(uint playerNid)
    {
        NetworkConnectionToClient res = null;
        ForEach((player) =>
        {
            if (player.netId == playerNid)
                res = player.connectionToClient;
        });
        return res;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        MapObject[] maps = GameObject.FindObjectsOfType<MapObject>();

        foreach(var map in maps)
        {
            map.RestartGame(NetworkClient.localPlayer.netId);
        }
    }


}
