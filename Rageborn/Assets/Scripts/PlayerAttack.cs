using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea = default;//transform.Find("AttackArea");


    // Start is called before the first frame update
    void Start()
    {
        attackArea = this.transform.Find("AttackArea").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            attackArea.SetActive(true);

        }
        else
        {
            attackArea.SetActive(false);
        }
    }
}