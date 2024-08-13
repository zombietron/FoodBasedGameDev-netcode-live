using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkPlayerIdentifier : NetworkBehaviour
{

    [SerializeField] TextMeshProUGUI playerID;
    
    //this network variable is updated OnNetworkSpawn with the OwnerClientID from the network behaviour
    //that throws OnNetworkSpawn
    NetworkVariable<ulong> playerIdNetworkV = new NetworkVariable<ulong>();
    //flag set to make sure this only hits on first update and then stops
    bool isPlayerIdSet = false;
    
    //fires when the object is spawned on the network. 
    //utilizing to set the field when client joins
    public override void OnNetworkSpawn()
    {
        
        if (IsServer)
        {
            playerIdNetworkV.Value = OwnerClientId +1;
        }   

        base.OnNetworkSpawn();


    }



    private void Update()
    {
        if (!isPlayerIdSet)
        {
            SetPlayerIdText();
            isPlayerIdSet = true;
        }
    }

    //set the playerId UGUI text to the updated Network Variable
    public void SetPlayerIdText()
    {
        playerID.text = $"P{playerIdNetworkV.Value}";
    }


}
