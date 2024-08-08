using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkedPlayerController : NetworkBehaviour
{
    private FoodBasedGameDevPlayerActions pActions;

    [SerializeField] float moveSpeed;

    Vector3 finalMove;
    
    //OWNERSHIP means WHO IS SIMULATING THIS OBJECT (moving, rotating, etc). 


    // Start is called before the first frame update
    void Start()
    {
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

    public void MoveMe(Vector3 move)
    {
       transform.Translate(move,Space.World);
       transform.rotation =  Quaternion.LookRotation(move);
    }

}
