using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 仅服务器管理
/// </summary>
public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance => m_instance;
    public static PlayerManager m_instance = null;
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
    }

    public void Remove(Player player)
    {
        m_players.Remove(player);
    }

    public void ForEach(UnityAction<Player> callback)
    {
        foreach(Player player in m_players)
        {
            callback(player);
        }
    }

}
