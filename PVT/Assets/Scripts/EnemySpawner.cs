using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<Transform> waypoints;

    [SerializeField]
    private EnemyBaseBehaviour enemyPrefab;

    [SerializeField]
    private int numberOfEnemies;

    [SerializeField]
    [Range(0.1f, 3f)]
    private float secondsBetweenEenemies = 0.5f;
    
    void Start()
    {
        waypoints = transform.GetComponentsInChildren<Transform>().ToList();
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var newEnemy = Instantiate<EnemyBaseBehaviour>(enemyPrefab, transform);
            newEnemy.Path = waypoints;
            yield return new WaitForSeconds(secondsBetweenEenemies);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        if (player is null)
        {
            return;
        }

        StartCoroutine(Spawn());
    }
}
