using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextTransition : MonoBehaviour
{
    [SerializeField] private float scrollTime;
        private void Awake() {
        StartCoroutine(WaitForScrollEnd());
    }

    private IEnumerator WaitForScrollEnd()
    {
        yield return new WaitForSeconds(scrollTime);

        SceneManagerScript.instance.LoadNextScene();
    }
}
