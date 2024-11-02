using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipShow : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake() {
        StartCoroutine(WaitForSkip());
    }

    private IEnumerator WaitForSkip()
    {
        yield return new WaitForSeconds(3);

        button.gameObject.SetActive(true);
    }
}
