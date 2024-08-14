using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Unity.Netcode;


[RequireComponent(typeof(LookAtConstraint))]
public class SetPlayerCameraConstraint : NetworkBehaviour
{
    //declare a constraint to be assigned at runtime
    LookAtConstraint constraint;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        constraint = GetComponent<LookAtConstraint>();
        if(constraint!=null)
        {
            InitializeConstraint();
        }

        base.OnNetworkSpawn();
    }

    /**Create new constraint and assign it to spot 0 in the constraints array. 
     * adjust for our camera rotation and set constraint value to 180 on y axis offset, and rest value to same
     * 
     * 
     */
    public void InitializeConstraint()
    {
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = Camera.main.transform;
        src.weight = 1;
        constraint.AddSource(src);
        constraint.rotationOffset = new Vector3(0, 180, 0);
        constraint.rotationAtRest = new Vector3(0, 180, 180);
    }

}
