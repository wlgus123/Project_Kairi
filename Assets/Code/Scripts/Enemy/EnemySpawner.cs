using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 정보")]
    public string enemyPoolName;
    public float respawnDelay = 3f;

    Enemy currentEnemy;
    bool isSpawning = false;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        if (isSpawning) return;

        GameObject obj = GameManager.Instance.poolManager
            .SpawnFromPool(enemyPoolName, transform.position, Quaternion.identity);

        if (obj == null) return;

        currentEnemy = obj.GetComponent<Enemy>();
        currentEnemy.Init(this);

        isSpawning = true;
    }

    public void OnEnemyDead(Enemy enemy)
    {
        if (enemy != currentEnemy) return;

        currentEnemy = null;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        isSpawning = false;
        Spawn();
    }
}
