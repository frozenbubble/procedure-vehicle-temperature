using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseBehaviour : MonoBehaviour
{
    public int maxHp = 50;
    public int currentHp = 0;

    private int targetWaypoint = 0;

    [SerializeField]
    private float speed = 3f;

    public List<Transform> Path { get; set; }

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

        if (Path != null)
        {
            MoveAlongPath();
        }
    }

    private void KillMe()
    {
        Destroy(transform.gameObject);
    }

    private void MoveAlongPath()
    {
        if (targetWaypoint > Path.Count - 1)
        {
            KillMe();
        }
        else
        {
            var target = Path[targetWaypoint].position;
            float delta = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, delta);

            if (transform.position == target)
            {
                targetWaypoint++;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var bulletBeha = other.GetComponent<BulletBehaviour>();

        if (bulletBeha != null) {
            currentHp -= bulletBeha.damage;
            Destroy(bulletBeha.gameObject);
        }

        var playerBeha = other.GetComponent<PlayerController>();

        if (playerBeha != null) {
            playerBeha.DamageMe();
        }
        
    }

}
