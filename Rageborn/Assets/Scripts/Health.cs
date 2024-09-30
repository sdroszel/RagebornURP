using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 100;
    // Update is called once per frame
    void Update()
    {
    }

    public void Damage(int amount)
    {
        this.health -= amount;
        if (this.health < 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        this.health += amount;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
