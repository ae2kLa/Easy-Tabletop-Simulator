using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tabletop.Online
{
    /// <summary>
    /// 仅服务器管理
    /// </summary>
    public class OnlinePlayerManager : NetworkBehaviour
    {
        public static OnlinePlayerManager Instance => m_instance;
        public static OnlinePlayerManager m_instance = null;

        private List<OnlinePlayer> m_players;

        private int maxPlayerCnt = 2;

        public override void OnStartServer()
        {
            if(m_instance == null)
            {
                m_instance = this;
                m_players = new List<OnlinePlayer>();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void Add(OnlinePlayer player)
        {
            m_players.Add(player);
            Debug.Log($"玩家Nid:{player.netId}加入游戏");

            if (OnlinePlayerManager.Instance.Count() == maxPlayerCnt)
            {
                OnlinePlayerManager.Instance.ForEachWithIndex((i, player) =>
                {
                    //TODO:后期暴露给玩家选择
                    if (i == 0)
                        player.CurrentColor = GoChessColor.Black;
                    else if (i == 1)
                        player.CurrentColor = GoChessColor.White;
                });
            }
        }

        public void Remove(OnlinePlayer player)
        {
            m_players.Remove(player);
        }

        public int Count()
        {
            return m_players.Count;
        }

        public void ForEachWithIndex(UnityAction<int, OnlinePlayer> callback)
        {
            for(int i = 0; i < m_players.Count; i++)
            {
                callback(i, m_players[i]);
            }
        }

        public void ForEach(UnityAction<OnlinePlayer> callback)
        {
            foreach(OnlinePlayer player in m_players)
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
            OnlineMapObj[] maps = GameObject.FindObjectsOfType<OnlineMapObj>();

            foreach(var map in maps)
            {
                map.RestartGame(NetworkClient.localPlayer.netId);
            }
        }


    }
}