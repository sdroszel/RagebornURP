using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private PlayerHealth playerHealth;

    private float remainingTime;

    private void Awake()
    {
        remainingTime = SceneManagerScript.instance.remainingTime;
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 30)
            {
                timerText.color = Color.red;
            }

            int displayMinutes = Mathf.FloorToInt(remainingTime / 60);
            int displaySeconds = Mathf.FloorToInt(remainingTime % 60);

            SceneManagerScript.instance.remainingTime = (displayMinutes * 60) + displaySeconds;

            timerText.text = string.Format("{0:00}:{1:00}", displayMinutes, displaySeconds);
        }
        else
        {
            timerText.text = "00:00";

            playerHealth.HandleDeath();
        }
    }
}
