using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class StaminaPotion : MonoBehaviour
{
    private TextMeshProUGUI numOfStaminaPotions;
    private int potionCount;

    private void Awake()
    {
        numOfStaminaPotions = GetComponent<TextMeshProUGUI>();

        potionCount = SceneManagerScript.instance.numOfStaminaPotions;
        numOfStaminaPotions.SetText(potionCount.ToString());
    }

    private void Update()
    {
        if (potionCount != SceneManagerScript.instance.numOfStaminaPotions)
        {
            potionCount = SceneManagerScript.instance.numOfStaminaPotions;
            numOfStaminaPotions.SetText(potionCount.ToString());
        }
    }
}
