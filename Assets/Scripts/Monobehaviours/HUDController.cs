using System.Collections;
using System.Collections.Generic;
using Monobehaviours.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : SingletonPersistent<HUDController>
{
    [SerializeField] private TextMeshProUGUI currentPlayerHP;
    [SerializeField] private Sprite activeFoodType;
    [SerializeField] private TextMeshProUGUI activeAmmoCount;
    [SerializeField] private TextMeshProUGUI activeWaveCount;
    [SerializeField] private TextMeshProUGUI currentScore;
    
    [SerializeField] private GameObject player;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject spawnManager;
    private HP playerHealth;
    private NetworkedPlayerController playerController;
    private Wave wave;
    private SpawnManager_Networked spawnController;
    void Awake()
    {
        wave = waveManager.GetComponent<Wave>();
        spawnController = spawnManager.GetComponent<SpawnManager_Networked>();

    }

    // Update is called once per frame
    void Update()
    {
        //currentPlayerHP.text = playerHealth.GetCurrentHP();
        //activeWaveCount.text = wave.GetWaveCount();
        //currentScore.text = spawnController.GetScore();
    }

    public void UpdateFoodIcon(Sprite ico)
    {
        activeFoodType = ico;
    }
    public void UpdateFoodAmt(int amt)
    {
        activeAmmoCount.text = amt.ToString();
    }
}
