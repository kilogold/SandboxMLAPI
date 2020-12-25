using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileCommandUi : MonoBehaviour
{
    public float rowHeight;
    public float colWidth;
    private UnetTransport transport;
    private bool isConnected = false;

    private void Start()
    {
        transport = NetworkingManager.Singleton.GetComponent<UnetTransport>();
        NetworkingManager.Singleton.OnServerStarted += ConnectionStarted;
        NetworkingManager.Singleton.OnClientConnectedCallback += ConnectionStarted;
    }

    private void ConnectionStarted(ulong obj)
    {
        ConnectionStarted();
    }

    private void ConnectionStarted()
    {
        isConnected = true;
    }

    private void OnGUI()
    {
        float rHeight = rowHeight * Screen.dpi;
        float cWidth = colWidth * Screen.dpi;
        Rect bounds = new Rect(Screen.width / 2 - cWidth / 2, 0, cWidth, rHeight);

        if (isConnected)
        {
            if (GUI.Button(bounds, "End Connection"))
            {
                var net = NetworkingManager.Singleton;
                if (net.IsHost) net.StopHost();
                else if (net.IsClient) net.StopClient();
                else if (net.IsServer) net.StopServer();

                isConnected = false;
            }
                
            return;
        }

        bounds.height *= 2;
        transport.ConnectAddress = GUI.TextField(bounds, transport.ConnectAddress);

        bounds.y += bounds.height;
        bounds.height /= 2;
        if (GUI.Button(bounds, "Connect Host"))
        {
            NetworkingManager.Singleton.StartHost();
        }
        bounds.y += bounds.height;
        bounds.y += bounds.height;
        if (GUI.Button(bounds, "Connect Server"))
        {
            NetworkingManager.Singleton.StartServer();
        }
        bounds.y += bounds.height;
        bounds.y += bounds.height;
        if (GUI.Button(bounds, "Connect Client"))
        {
            NetworkingManager.Singleton.StartClient();
        }
    }
}
