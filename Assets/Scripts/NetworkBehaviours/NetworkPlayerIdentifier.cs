using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkPlayerIdentifier : NetworkBehaviour
{

    [SerializeField] TextMeshProUGUI playerID;

    //fires when the object is spawned on the network. 
    //utilizing to set the field when client joins
    public override void OnNetworkSpawn()
    {
        // the client if not connected is not getting th information set, therefore it is not updating. 
        // fix for next week. 
        base.OnNetworkSpawn();
        
        if (!IsOwner) return;

        var id = $"P{NetworkManager.LocalClientId + 1}";
        SetPlayerIdentifierTextRpc(id);

    }

    //This notifies all clients and hosts of the updated text field value
    //without this the data will not sync
    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerIdentifierTextRpc(string val) 
    {
        playerID.text = val;
    }
}
