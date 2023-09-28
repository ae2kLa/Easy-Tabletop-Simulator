using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManagerLobby : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        #region base.OnServerAddPlayer();
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        #endregion
        if(player.TryGetComponent(out Player playerComp))
        {
            PlayManager.Instance.Add(playerComp);
        }
        else
        {
            Debug.LogError($"玩家上无Player组件");
        }
    }






}
