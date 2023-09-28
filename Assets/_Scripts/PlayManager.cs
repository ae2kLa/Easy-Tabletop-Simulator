using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LowLevel;

/// <summary>
/// 仅服务器管理
/// </summary>
public class PlayManager : NetworkBehaviour
{
    public static PlayManager Instance => m_instance;
    public static PlayManager m_instance = null;

    private List<Player> m_players;

    private int maxPlayerCnt = 2;

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
            return;
        }
    }

    public void Add(Player player)
    {
        m_players.Add(player);
        Debug.Log($"检测到玩家连接，Nid:{player.netId}");

        if (PlayManager.Instance.Count() == maxPlayerCnt)
        {
            PlayManager.Instance.ForEachWithIndex((i, player) =>
            {
                //TODO:后期暴露给玩家选择
                if (i == 0)
                    player.CurrentColor = GoChessColor.Black;
                else if (i == 1)
                    player.CurrentColor = GoChessColor.White;
            });
        }
    }

    public void Remove(Player player)
    {
        m_players.Remove(player);
        Debug.Log($"检测到玩家退出，Nid:{player.netId}");
    }

    public int Count()
    {
        return m_players.Count;
    }

    public void ForEachWithIndex(UnityAction<int, Player> callback)
    {
        for(int i = 0; i < m_players.Count; i++)
        {
            callback(i, m_players[i]);
        }
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

    public void RestartGame()
    {
        MapObject[] maps = GameObject.FindObjectsOfType<MapObject>();

        foreach(var map in maps)
        {
            map.RestartGame(NetworkClient.localPlayer.netId);
        }
    }


}
