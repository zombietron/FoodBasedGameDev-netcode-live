using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.Events;
using System;

public class GameManager_Networked : NetworkBehaviour
{
    //setwaves into action
    //score

    public static GameManager_Networked Instance { get; private set; }

    public WaveManager waveMgr;

    public bool firstRun = true;
    public enum GameState
    {
        menu,
        timer,
        gameRunning,
        gameEnding,
        pause
    }

    public GameState gameState;

    [SerializeField]
    TimerUiNetworked timer;

    [SerializeField]
    int playersInGame;

    public int PlayersInGame
    {
        set 
        { 
            playersInGame = value;
            
            if (firstConnect)
                firstConnect = false;
            
        }
        get { return playersInGame; }
    }

    public bool firstConnect = true;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;


        DontDestroyOnLoad(this);
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.OnServerStarted += StartWave;
        //for testing while we wait to build our menu scene.
        // OnSceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        //StartWave();
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.OnServerStarted -= StartWave;
        base.OnNetworkDespawn();
    }
    public void ChangeGameState(GameState newState)
    {
        gameState = newState;

        switch (gameState) 
        {
            case GameState.menu:
                //do menu stuff
                break;

            case GameState.timer:
                if (!firstRun)
                    EnableCountDownTimerRpc();

                timer.StartCountdown(3);
                firstRun = false;
                break;

            case GameState.gameRunning:
                WaveManager.changeWaveState(WaveManager.WaveState.preWave);
                break;

            case GameState.gameEnding:
                EndGame();
                break;
            
            case GameState.pause:
                PauseGame();                
                break;
            
            default: break;


        }
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if(!firstConnect && playersInGame ==  0)
        {
            ChangeGameState(GameState.gameEnding);
        }
    }

    public void EndGame()
    {
        //StartCoroutine("RestartGame");
        Debug.Log("I'm the end of the game");
        EnableCountDownTimerRpc();
        //return player to lobby?
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void PauseGame()
    {
        Debug.Log("Paused");
    }
    

/*    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (!IsServer)
            return;

        Debug.Log($"{SceneManager.GetActiveScene()} has loaded");
        waveMgr = FindObjectOfType<WaveManager>();
        waveMgr.wave.ResetWaveCount();
        if(scene.name == "Main")
        {
            ChangeGameState(GameState.gameRunning);
        }
    }*/

    public void StartWave()
    {
        ChangeGameState(GameState.timer);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EnableCountDownTimerRpc()
    {
        timer.gameObject.SetActive(true);
       
    }
}
