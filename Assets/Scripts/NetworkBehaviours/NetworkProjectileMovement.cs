using UnityEngine;
using Unity.Netcode;

public class NetworkProjectileMovement : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner)
            return;

        MoveProjectileForwardAtSpeed();
    }

    public void MoveProjectileForwardAtSpeed()
    {
        transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
    }
}
