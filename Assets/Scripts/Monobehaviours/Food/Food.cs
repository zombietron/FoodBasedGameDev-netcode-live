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

    [SerializeField]
    private GameObject foodOriginalPrefab;

    [SerializeField]
    [Range(0f, 15f)]
    private float foodLifetime = 5f;

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        foodLifetime -= Time.deltaTime;

        if (foodLifetime <= 0f)
            ReturnFoodToPool();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer)
            return;

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

        // TODO: fix this line, because returning the object like this does not work
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, foodOriginalPrefab);
    }
}