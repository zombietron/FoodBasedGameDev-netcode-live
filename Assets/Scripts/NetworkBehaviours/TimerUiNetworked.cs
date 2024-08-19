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

   public override void OnNetworkSpawn()
    {
        if(networkTimerComplete.Value)
        {
            SetTimerVisibilityRpc(false);
            return;
        }

        if(IsServer && !networkTimerComplete.Value)
        {
            StartCoroutine(StartTimer(timerLength));
        }
        base.OnNetworkSpawn();

        
    }



    public IEnumerator StartTimer(int length)
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
        GameManager_Networked.Instance.ChangeGameState(GameManager_Networked.GameState.gameRunning);
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
