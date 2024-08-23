using UnityEngine;
using Unity.Netcode;

[DisallowMultipleComponent]
public class NetworkReturnToPoolOnTrigger : NetworkBehaviour
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