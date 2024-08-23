using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager_Networked : SingletonNetwork<SpawnManager_Networked>
{
    [SerializeField] int spawnGap = 3;
    private int score = 0;

    public int SpawnGap
    {
        set { spawnGap = value; }
        get { return spawnGap; }
    }

    public List<NetworkObject> enemyPools;

    [SerializeField] List<Transform> spawnPoints = new();
    [SerializeField] WaveManager waveMgr;
    [SerializeField] List<Transform> enemiesInScene = new();
    public List<Transform> EnemiesInScene => enemiesInScene;


    //bool waveComplete = false;
    GameObject spawnedEnemies;
    int spawnLocationIndex = 0;



    /*
     * WaveManager now connected to GameManager. The game manager tells 
     * the wave manager when its time to spawn a wave which then
     * tells the SpawnController how many and where
    */

    IEnumerator SpawnMonstersWithGap(int gap)
    {
        Debug.Log("SpawnMonstersWithGap Coroutine Started at: " + Time.time);
        Debug.Log("Enemies in Scene: " + enemiesInScene.Count);

        while (GameManager_Networked.Instance.gameState == GameManager_Networked.GameState.gameRunning)
        {
            if (enemiesInScene.Count < waveMgr.wave.maxEnemiesSpawnedDuringWave && 
                !waveMgr.wave.waveComplete)
            {
                NetworkObject enemy = SelectEnemyType();

                var spawnedEnemy = NetworkObjectPool.Singleton.GetNetworkObject(
                    enemy.gameObject,
                    spawnPoints[spawnLocationIndex].position,
                    Quaternion.identity);

                spawnedEnemy.Spawn();

                enemiesInScene.Add(spawnedEnemy.transform);
                waveMgr.wave.AddEnemyToWaveCount();

                spawnLocationIndex = 
                    spawnLocationIndex >= spawnPoints.Count - 1 ?
                        0 : spawnLocationIndex + 1;

                //spawnedEnemy.transform.parent = spawnedEnemies.transform;
                yield return new WaitForSeconds(gap);
            }
            else
                yield return new WaitUntil(
                    () => enemiesInScene.Count < waveMgr.wave.maxEnemiesSpawnedDuringWave);


        }

        yield break;

    }

    public NetworkObject SelectEnemyType()
    {
        NetworkObject selectedEnemyType = null;
        int randomNumber = Random.Range(0, 100);

        switch (randomNumber)
        {

            //case int n when n == 0:
            //Debug.Log("I'd spawn a ghost here: {n}");
            //break;


            case int n when n >= 1 && n <= 5:
                selectedEnemyType = enemyPools[2]; //skele
                break;


            case int n when n > 5 && n <= 14:
                selectedEnemyType = enemyPools[1]; //vamp
                break;

            //If the number is 0 to 80 
            case int n when n > 14:
                selectedEnemyType = enemyPools[0]; //zombo
                break;

            default:
                selectedEnemyType = enemyPools[0]; //zombo
                break;

        }

        return selectedEnemyType;
    }

    public void StartMonsterWithGapCoRoutine(int gap)
    {
        StartCoroutine(SpawnMonstersWithGap(gap));
    }

    public void StopSpawning()
    {
        StopCoroutine(SpawnMonstersWithGap(spawnGap));
    }

    public void RemoveDestroyedEnemy(Transform enemyTrans)
    {
        enemiesInScene.Remove(enemyTrans);
        score++;
        if (enemiesInScene.Count == 0)
        {
            WaveManager.changeWaveState(WaveManager.WaveState.preWave);
        }
    }

    public string GetScore()
    {
        return score.ToString();
    }
}
