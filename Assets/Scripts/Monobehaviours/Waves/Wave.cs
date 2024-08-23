using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(WaveManager))]
public class Wave : NetworkBehaviour
{

    [Tooltip("Use these to initialize your first wave")]

    public int enemyCount { private set; get; }
    public float waveTime { private set; get; }
    public NetworkVariable<int> waveNum = new NetworkVariable<int>();
    public int maxEnemiesSpawnedDuringWave { private set; get; }
    public int enemiesSpawnedDuringWave { private set; get; }
    public bool waveComplete { private set; get; }



    

    public void InitFirstWave()
    {
        Debug.Log("First Wave Initialization Called");
        enemyCount = 10;
        waveTime = 60;
        waveNum.Value = 1;
        maxEnemiesSpawnedDuringWave = 4;
        waveComplete = false;
    }

    public void ProgressWave()
    {
        enemyCount += 5;
        waveNum.Value++;
        maxEnemiesSpawnedDuringWave += 4;
        enemiesSpawnedDuringWave = 0;
        waveComplete = false;
        if(waveNum.Value%3 == 0)
        {
            waveTime += 30; 
        }
    }
    
    public void AddEnemyToWaveCount()
    {
        enemiesSpawnedDuringWave++;
        if(enemiesSpawnedDuringWave >= enemyCount)  
        {
            waveComplete = true;
            Debug.Log($"Wave {waveNum} is complete");
        }
    }

    public string GetWaveCount()
    {
        return waveNum.Value.ToString();
    }

    public void ResetWaveCount()
    {
        waveNum.Value = 0;
    }

    
}
