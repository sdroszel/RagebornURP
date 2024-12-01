using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoors : MonoBehaviour
{
    [SerializeField] private Animator animator1;
    [SerializeField] private Animator animator2;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Close());

    }

    private IEnumerator Close()
    {
        yield return new WaitForSeconds(1);

        animator1.SetTrigger("close");
        animator2.SetTrigger("close");
    }
}
