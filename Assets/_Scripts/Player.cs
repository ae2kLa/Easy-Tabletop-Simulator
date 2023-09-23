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
        if (!isLocalPlayer) return;

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
    protected void CmdAuthority(NetworkIdentity id, NetworkConnectionToClient connClient)
    {
        id.RemoveClientAuthority();
        id.AssignClientAuthority(connClient);
    }

    //[Command]
    //public void CmdGet(ContainerObj containerObj)
    //{
    //    if (!isLocalPlayer) return;

    //    if (containerObj.Contents.Count == 0)
    //    {
    //        if (containerObj.CountUnlimitedToggle)
    //        {
                
    //        }
    //    }

    //    print("List count:" + containerObj.Contents.Count);

    //    containerObj.currentDragObj = containerObj.Contents[containerObj.Contents.Count - 1];
    //    containerObj.currentDragObj.OnMouseDown();
    //    containerObj.currentDragObj.gameObject.SetActive(true);

    //    containerObj.Contents.RemoveAt(containerObj.Contents.Count - 1);
    //}


}
