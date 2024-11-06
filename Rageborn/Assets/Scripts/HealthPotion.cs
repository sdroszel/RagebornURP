using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    private TextMeshProUGUI numOfHealthPotions;
    private int potionCount;

    private void Awake()
    {
        numOfHealthPotions = GetComponent<TextMeshProUGUI>();
        
        potionCount = SceneManagerScript.instance.numOfHealthPotions;
        numOfHealthPotions.SetText(potionCount.ToString());
    }

    private void Update()
    {
        if (potionCount != SceneManagerScript.instance.numOfHealthPotions)
        {
            potionCount = SceneManagerScript.instance.numOfHealthPotions;
            numOfHealthPotions.SetText(potionCount.ToString());
        }
    }
}
