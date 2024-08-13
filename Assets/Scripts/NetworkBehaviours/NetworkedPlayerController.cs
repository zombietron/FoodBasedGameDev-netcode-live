using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkedPlayerController : NetworkBehaviour
{
    private FoodBasedGameDevPlayerActions pActions;

    
    [SerializeField] float moveSpeed;
    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject hotDogPrefab;

    Vector3 finalMove;

    PlayerInput playerInput;

    Animator anim;
    //OWNERSHIP means WHO IS SIMULATING THIS OBJECT (moving, rotating, etc). 


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        pActions = new FoodBasedGameDevPlayerActions();
        pActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        var move = pActions.Player.Move.ReadValue<Vector2>();
        finalMove = new Vector3(-move.x, 0, -move.y) * Time.deltaTime * moveSpeed;
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
            }
            else
                playerInput.enabled = false;
        } 

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if(playerInput !=null)
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
        if (IsOwner && IsClient)
        {
            ThrowHotDogRpc();
        }
    }


    //send request to server to instantiate the hotdog and spawn at the correct position
    [Rpc(SendTo.Server)]
    public void ThrowHotDogRpc()
    {
        //NetworkObjectSpawner.SpawnNewNetworkObject(hotDogPrefab, spawnPos.position);
        var spawnedHotDog = NetworkObjectPool.Singleton.GetNetworkObject(hotDogPrefab, spawnPos.position, spawnPos.rotation);
        spawnedHotDog.Spawn();
        Debug.Log("I THREW A HOTDOG!");
    }
}
