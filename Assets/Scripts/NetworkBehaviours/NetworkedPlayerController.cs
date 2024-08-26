using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(AttackData))]
public class NetworkedPlayerController : NetworkBehaviour
{
    private FoodBasedGameDevPlayerActions pActions;

    [SerializeField] float moveSpeed;
    [SerializeField] Transform spawnTransform;
    
    Vector3 finalMove = Vector3.zero;

    PlayerInput playerInput;

    Animator anim;
    //OWNERSHIP means WHO IS SIMULATING THIS OBJECT (moving, rotating, etc). 

    //Attack information
    List<ProjectileInfo> projectiles;
    int projectileIndex;
    bool isDead = false;

    public bool IsDead
    {
        set { isDead = value; }
        get { return isDead; }
    }
    public int ProjectileIndex
    {
        set { projectileIndex = value; }
        get { return projectileIndex; }
    }

    //ammo stall reload information
    bool isInteractable;
    StallBehavior stallBehavior;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        pActions = new FoodBasedGameDevPlayerActions();
        pActions.Enable();
        projectiles = GetComponent<AttackData>().PROJECTILES;
        //TODO: Initialize UI elements (projectile Icon, ammo ammount display)
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner || isDead)
            return;

        var move = pActions.Player.Move.ReadValue<Vector2>();
        float moveSpeedxDeltaTime = Time.deltaTime * moveSpeed;

        finalMove.x = -move.x * moveSpeedxDeltaTime;
        finalMove.z = -move.y * moveSpeedxDeltaTime;

        MoveMe(finalMove);

    }

    public override void OnNetworkSpawn()
    {
        //This is to prevent the PlayerInput component from fighting with other instances
        //check if the object has a player component, if so, disable it unless its on the owner

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            if (IsOwner)
            {
                playerInput.enabled = true;
                SetProjectile(0);
                IsDead = false;
            }
            else
                playerInput.enabled = false;
        } 

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if(playerInput != null)
        {
            playerInput.enabled = false;
        }

        base.OnNetworkDespawn();
    }

    public void MoveMe(Vector3 move)
    {
       transform.Translate(move,Space.World);
       transform.rotation =  Quaternion.LookRotation(move);

        if (move != Vector3.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
            anim.SetBool("isRunning", false);
    }

    //when the throw action is activated (spacebar pressed)
    public void OnThrow()
    {
        
        if (IsOwner && IsClient &&  projectiles[projectileIndex].ammoAmount>0)
        {
            projectiles[projectileIndex].ammoAmount--;
            HUDController.Instance.UpdateFoodAmt(projectiles[projectileIndex].ammoAmount);
            ThrowProjectileRpc(projectileIndex, spawnTransform.position, transform.rotation);
        }
    }

    public void OnToggle()
    {
        if(IsOwner && IsClient)
        {
            projectileIndex = projectileIndex < projectiles.Count - 1 ? projectileIndex + 1 : 0;
            SetProjectile(projectileIndex);

        }
    }

    public void SetProjectile(int pIndex)
    {
        projectileIndex = pIndex;
        HUDController.Instance.UpdateFoodAmt(projectiles[projectileIndex].ammoAmount);
        HUDController.Instance.UpdateFoodIcon(projectiles[projectileIndex].icon);

    }

    public void OnInteract(InputValue value)
    {
        if (!isInteractable) return;
        stallBehavior.StartAmmoPickupCycle(value.isPressed);
        // trigger pick up animation?
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("foodStall") || !IsOwner) return;
        stallBehavior = other.gameObject.GetComponent<StallBehavior>();
        stallBehavior.DisplayUseInstructions(true);
        isInteractable = true;
        stallBehavior.Player = this;
    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("foodStall") || !IsOwner) return;

        isInteractable = false;
        stallBehavior.DisplayUseInstructions(false);
    }
    //send request to server to instantiate the hotdog and spawn at the correct position
    [Rpc(SendTo.Server)]
    public void ThrowProjectileRpc(int key, Vector3 pos,Quaternion rot)
    {
        var spawnedProjectile = NetworkObjectPool.Singleton.GetNetworkObject(projectiles[key].projectilePrefab,
            pos,
            rot);

        spawnedProjectile.GetComponent<Food>().foodOriginalPrefab = projectiles[projectileIndex].projectilePrefab;
        spawnedProjectile.Spawn();

        Debug.Log($"I THREW A {spawnedProjectile.name}");
    }
    public void ReloadCurrentProjectile()
    {
        projectiles[ProjectileIndex].RefillAmmo();
        HUDController.Instance.UpdateFoodAmt(projectiles[ProjectileIndex].ammoAmount);
    }
    public Sprite GetActiveFoodTypeToThrow()
    {
        return projectiles[projectileIndex].icon;
    }

    public int GetCurrentAmmoCount()
    {
        return projectiles[projectileIndex].ammoAmount;
    }
}
