using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    [SerializeField]
    private int hungerReductionAmount = 50;

    [SerializeField]
    private PizzaBombProjectiles pizzaParent;

    [SerializeField]
    private AudioSource impactSound;

    [HideInInspector]
    public GameObject foodOriginalPrefab;

    // What is the real shelf life of a hot dog?
    [SerializeField]
    [Range(0f, 15f)]
    private float foodLifetime = 5f;

    private float currentFoodLifetime = 0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        currentFoodLifetime = foodLifetime;
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        currentFoodLifetime -= Time.deltaTime;

        if (currentFoodLifetime <= 0f)
            ReturnFoodToPool();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // making sure to handle trigger collisions on server
        if (!IsServer)
            return;

        AudioSource.PlayClipAtPoint(impactSound.clip,transform.position);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(collision.gameObject.TryGetComponent<HP>(out var hpComponent))
            {
                hpComponent.ReduceHP(hungerReductionAmount);
            }
            else
            {
                Debug.Log("This collision's HP is null yo!");
            }
            
            ReturnFoodToPool();
        }
    }

    private void ReturnFoodToPool()
    {
        NetworkObject.Despawn(false);

        NetworkObjectPool.Singleton.ReturnNetworkObject(
            NetworkObject,
            foodOriginalPrefab);
    }
}