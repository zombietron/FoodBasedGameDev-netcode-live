using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Wave))]
public class WaveManager : MonoBehaviour
{
  public enum WaveState
    {
        preWave,
        running,
    }

    public WaveState waveState = WaveState.preWave;

    public Wave wave;

    public SpawnManager_Networked spawnController;

    public delegate void ChangeWaveState(WaveState state);

    public static ChangeWaveState changeWaveState;

    [SerializeField] TimerUiNetworked timer;

    private void Awake()
    {
        changeWaveState += UpdateWaveState;
    }

    public void UpdateWaveState(WaveState newState)
    {
        waveState = newState;
        Debug.Log("WaveState changed to " + waveState);
        Debug.Log($"StartingWave {wave.waveNum}");
        switch (waveState) 
        {
            case WaveState.preWave:
                spawnController.StopSpawning();
                if (wave.waveNum == 0)
                {
                    wave.InitFirstWave();
                    changeWaveState(WaveState.running);
                } 
                else
                     wave.ProgressWave();
                changeWaveState(WaveState.running);
                //timer.gameObject.SetActive(true);
                //timer.StartTimer(3);
                break;

            case WaveState.running:
                //timer.gameObject.SetActive(false);
                spawnController.StartMonsterWithGapCoRoutine(spawnController.SpawnGap);
                break;

            default: break;
        }

    }

    private void OnDestroy()
    {
        changeWaveState -= UpdateWaveState;
    }
}
