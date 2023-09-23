using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private void Update()
    {
    }

    public void GetAuthority(GameObject targetGo)
    {
        if(targetGo == null)
        {
            Debug.LogWarning("目标对象为空");
            return;
        }

        var id = targetGo.GetComponent<NetworkIdentity>();

        if(id == null)
        {
            Debug.LogWarning("目标脚本为空");
            return;
        }

        CmdAuthority(id, connectionToClient);
    }

    [Command]
    public void CmdAuthority(NetworkIdentity id, NetworkConnectionToClient connClient)
    {
        id.RemoveClientAuthority();
        id.AssignClientAuthority(connClient);
    }

}
