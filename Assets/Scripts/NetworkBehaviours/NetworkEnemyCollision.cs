using UnityEngine;
using Unity.Netcode;

[DisallowMultipleComponent]
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