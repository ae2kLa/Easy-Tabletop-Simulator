using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStart : MonoBehaviour
{
    public bool IsAppServer;

    private NetworkManager m_networkManager;

    private void Start()
    {
        m_networkManager = GetComponent<NetworkManager>();

        if(IsAppServer)
            m_networkManager.StartServer();
        else
            m_networkManager.StartClient();
    }



}
