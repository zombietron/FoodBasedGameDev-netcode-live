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

        AudioSource.PlayClipAtPoint(impactSound.clip,transform.position);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            collision.gameObject.GetComponent<HP>().ReduceHP(hungerReductionAmount);

            ReturnFoodToPool();
        }
    }

    private void ReturnFoodToPool()
    {
        NetworkObject.Despawn(false);

        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, foodOriginalPrefab);
    }
}