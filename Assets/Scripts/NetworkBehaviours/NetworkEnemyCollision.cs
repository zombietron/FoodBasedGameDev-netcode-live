using UnityEngine;
using Unity.Netcode;

public class NetworkEnemyCollision : NetworkBehaviour
{
    [HideInInspector]
    public GameObject originalPrefabKey;

    private void OnTriggerEnter(Collider other)
    {
        NetworkObjectPool.Singleton.ReturnNetworkObject(
            NetworkObject,
            originalPrefabKey);
    }
}
