using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TimerUiNetworked : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int timerLength=3;
    [SerializeField] WaveManager wMgr;
    [SerializeField] GameObject waveCanvas;
    NetworkVariable<int> networkTimerVariable = new NetworkVariable<int>();
    NetworkVariable<bool> networkTimerComplete = new NetworkVariable<bool>();

    // Start is called before the first frame update
   public override void OnNetworkSpawn()
    {
        if(networkTimerComplete.Value)
        {
            SetTimerVisibilityRpc(false);
            return;
        }

        if(IsServer && !networkTimerComplete.Value)
        {
            StartCoroutine(RunTimer(timerLength));
        }
        base.OnNetworkSpawn();

        
    }



    public IEnumerator RunTimer(int length)
    {
        networkTimerVariable.Value = length;

        while (networkTimerVariable.Value >= 0)
        {
            SetTimerTextRpc(networkTimerVariable.Value);
            networkTimerVariable.Value--;
            yield return new WaitForSeconds(1);
        }

        // commenting for now, until we update wavemanager
        //wMgr.UpdateWaveState(WaveManager.WaveState.running);
        SetTimerVisibilityRpc(false);
        networkTimerComplete.Value = true;
        yield break;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetTimerTextRpc(int value)
    {
        timerText.text = value.ToString();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetTimerVisibilityRpc(bool val)
    {
        waveCanvas.SetActive(val);
    }
}