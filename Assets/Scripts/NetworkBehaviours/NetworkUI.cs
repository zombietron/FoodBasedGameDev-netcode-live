using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class NetworkUI : MonoBehaviour
{
    //OWNERSHIP means WHO IS SIMULATING THIS OBJECT (moving, rotating, etc). 


    public void StartServerOnClick()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Server Started");
    }    
    
    public void StartHostOnClick()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Started");
    }

    public void StartClientOnClick()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Started");
    }
}
