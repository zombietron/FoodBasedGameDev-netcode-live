using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateWaveInfo : MonoBehaviour
{
    [SerializeField] WaveManager wave;
    [SerializeField] TextMeshProUGUI waveTitleText;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (GameManager_Networked.Instance.gameState != GameManager_Networked.GameState.gameEnding)
            waveTitleText.text = "Wave " + wave.wave.waveNum.Value.ToString();
        else
            waveTitleText.text = "Game Over";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
