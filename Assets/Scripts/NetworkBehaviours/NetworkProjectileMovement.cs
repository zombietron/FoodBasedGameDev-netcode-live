using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkProjectileMovement : NetworkBehaviour
{
    [SerializeField]
    float moveSpeed=10f;

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            MoveProjectileForwardAtSpeed();
        }
    }

    public void MoveProjectileForwardAtSpeed()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
