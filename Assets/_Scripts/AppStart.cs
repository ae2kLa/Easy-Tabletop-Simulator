using Mirror;
using Mirror.Examples.NetworkRoom;
using System;
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
        {
            m_networkManager.StartServer();
            //InitForServerBuild();
        }
        else
        {
            m_networkManager.StartClient();

        }
    }

    public void InitForServerBuild()
    {
        Debug.Log("InitForServerBuild");
        string CommandLine = Environment.CommandLine;
        //Debug.Log(CommandLine);
        string[] CommandLineArgs = Environment.GetCommandLineArgs();
        for (int i = 0; i < CommandLineArgs.Length; i++)
        {
            //Debug.Log(CommandLineArgs[i]);
            if (CommandLineArgs[i] == "--port" && i + 1 < CommandLineArgs.Length)
            {
                string port = CommandLineArgs[i + 1];
                NetworkRoomManagerExt.singleton.gameObject.SendMessage("SetPort", port);
                NetworkRoomManagerExt.singleton.StartServer();
            }
        }
    }

}
