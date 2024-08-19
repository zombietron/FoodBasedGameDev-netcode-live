using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] WaveManager waveMgr;
    [SerializeField] List<Transform> enemiesInScene;
    public List<Transform> EnemiesInScene
    {
        get { return enemiesInScene; }
    }


    //bool waveComplete = false;
    GameObject spawnedEnemies;
    int spawnLocationIndex = 0;



    /*
     * WaveManager now connected to GameManager. The game manager tells 
     * the wave manager when its time to spawn a wave which then
     * tells the SpawnController how many and where
    */

    private void Awake()
    {
        enemiesInScene = new List<Transform>();
    }


    IEnumerator SpawnMonstersWithGap(int gap)
    {
        Debug.Log("SpawnMonstersWithGap Coroutine Started at: " + Time.time);

        while (GameManager.Instance.gameState == GameManager.GameState.gameRunning)
        {

            if (enemiesInScene.Count < waveMgr.wave.maxEnemiesSpawnedDuringWave && !waveMgr.wave.waveComplete)
            {
                NetworkObject enemy = SelectEnemyType();

                var spawnedEnemy = NetworkObjectPool.Singleton.GetNetworkObject(enemy.gameObject, spawnPoints[spawnLocationIndex].position, Quaternion.identity);
                enemiesInScene.Add(spawnedEnemy.transform);
                waveMgr.wave.AddEnemyToWaveCount();
                spawnLocationIndex = spawnLocationIndex >= spawnPoints.Count - 1 ? 0 : spawnLocationIndex + 1;

                //spawnedEnemy.transform.parent = spawnedEnemies.transform;
                yield return new WaitForSeconds(gap);
            }
            else
                yield return new WaitUntil(() => enemiesInScene.Count < waveMgr.wave.maxEnemiesSpawnedDuringWave);


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
