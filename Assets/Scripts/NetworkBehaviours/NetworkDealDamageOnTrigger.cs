using UnityEngine;
using Unity.Netcode;

[DisallowMultipleComponent]
public class NetworkDealDamageOnTrigger : NetworkBehaviour
{
    [SerializeField]
    private int m_damageAmount = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if(other.TryGetComponent<HP>(out var hpNetworked))
        {
            hpNetworked.ReduceHP(m_damageAmount);
        }
    }
}