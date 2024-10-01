using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 1;

    private void OnTriggerStay(Collider collider)
    {
        Debug.Log("on triggerEnter");
        if (collider.GetComponent<Health>() != null)
        {
            Health health = collider.GetComponent<Health>();
            health.Damage(damage);
        }
    }

    void Update()
    {

    }

}