using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Netcode;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MeleAttack))]
public class NavmeshMovement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private float agentSpeed;

    MeleAttack atk;
    
    [SerializeField] 
    private GameObject selectedTarget;

    public UnityEvent<bool> inAttackRange;

    List<GameObject> targets;
    public override void OnNetworkSpawn()
    {
        agent.enabled = true;
        agent.speed = agentSpeed;
        atk = GetComponent<MeleAttack>();
        
        targets = new List<GameObject>();

        foreach (KeyValuePair<ulong, NetworkClient> item in NetworkManager.Singleton.ConnectedClients)
        {
            targets.Add(item.Value.PlayerObject.gameObject);
        }

        selectedTarget = SelectTargetPlayer();
        atk.Target = selectedTarget;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        agent.enabled = false;
        base.OnNetworkDespawn();
    }

    /*  removed due to network rewrite, handling in OnNetworkSpawn()
     *  private void OnEnable()
        {
            selectedTarget = SelectTargetPlayer();
            atk.Target = selectedTarget;
        }*/

    private GameObject SelectTargetPlayer()
    {


        return targets[Random.Range(0, targets.Count-1)];
    }

    #region highlyImportant
    /*                                                   
                 ...::::::.                   
             .:^~!7777??777!^.                
           :^!7?YYYYJ???????7!:.              
         .^!7J55J7!!!!!!777???777!~^.         
        :~!?5PJ!!!!!!!!!77777???JJJ?!^:       
       ^!!?55?7!!!!!!!!77?JJJJJJJJJYJ7!^      
      ^7?YPG5P55J7!77??JYPPPPPPPPPP55J?7:     
    .^?P55Y??7?JY?BPPGYG5YJJJJJJY5G&&#PJ!.    
    :!G5557?7!!!?JP?JYY5J?7?JJYYYYYBBPGPJ~    
    ^7PPGPYYYY5555?7YG55Y5YPGGGPYJYB55BPPJ^   
   .~7YG5J7???JP577?Y5GBP5JJJJJJJYPY55GG55!   
   .~!?55JJYY55J!!7?Y55PP55YJJY555JJYYJY55J:  
   .^!?Y?!777?????J5P555Y??JJJJJ?JJYYYYYY5J~  
    ^!7J??77JJ?JJJ5PPP55YJ?777??JJYYY555YYY!. 
    ^~!????JJ?7!!!77??JJJJJJJJJYY55555GPYYJ!. 
   .^~!!77?J????????JJYYYYYYY55555Y555G5YJ?^. 
   :^~~77777?YGYYY5555GGBG55YYYYYYY55555YJ?^. 
  .^^~~7??77?JB&&&#####&&&5YJJJJYY5PBB5YYJ?^  
  .^^~~!?YJ777Y##BYJYG##GYJJJJYYY5PBBPYYYY?:  
..::^^~~^^~!?5P?77?JY5J?JY5YYJJJJY555PGBG5YYYJ?:  
!?J7!!~~~^~!7YG57!77???JJJYYJJJJJ555GBBGPYYJYJ?:  
PBBBP7~~~^~~!JPGY!77777???JJJJYY55PB#BBG5YYJJY5J: 
GBGGPY!~~~~~!?PPG?777777??JJJYPPPGB#&##G555YYPB##J
#&BG#B!!~~~~??PPGGJ??????JJYPGGGGB##&##GPGG5G&##@@
#BB#@B!!!!~!J?PBPGBPYYY55PGGGGGGB#&@@&#B##BB#&&&@@
GGB##J!7!!!7JJ5BBGGBG55P5PPGGGGB#&@@@@&@@&&&&&&&@&
BB&&G7?7!7!7JJ5PBBGBBGGGGGGGGGB#@@@@@@@@@&&&&@&@@&
 */
    #endregion


    void Update()
    {
        if (!IsServer)
            return;

        if (!selectedTarget.GetComponent<HP>().Interactible && GameManager_Networked.Instance.PlayersInGame > 0)
            selectedTarget = SelectTargetPlayer();
        else
            agent.ResetPath();

        agent.SetDestination(selectedTarget.transform.position);
        
        if(agent.destination != null) {
            if (Vector3.Distance(agent.gameObject.transform.position,selectedTarget.transform.position) <=1f)
            {
                Debug.Log(Vector3.Distance(agent.gameObject.transform.position, selectedTarget.transform.position));
                inAttackRange.Invoke(true);
            }
        } 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, 1.0f);

    }
}
