using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseBehaviour : MonoBehaviour
{
    public int maxHp = 50;
    public int currentHp = 0;

    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0) {
            KillMe();
        }
    }

    private void KillMe()
    {
        Destroy(transform.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        var bulletBeha = other.GetComponent<BulletBehaviour>();

        if (bulletBeha != null) {
            currentHp -= bulletBeha.damage;
            Destroy(bulletBeha.gameObject);
        }
        
    }

}
