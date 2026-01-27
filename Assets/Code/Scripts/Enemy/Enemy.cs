using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
	EnemyStatsRuntime currStat;			// 현재 스탯
    EnemySpawner ownerSpawner;
    public EnumType.EnemyState state;   // 현재 상태

    public void Init(EnemySpawner spawner)
    {
        ownerSpawner = spawner;
    }

    void OnEnable()
    {
        // 풀에서 다시 나올 때마다 초기화
        state = EnumType.EnemyState.Idle;
        currStat = new EnemyStatsRuntime(GameManager.Instance.enemyStats);
    }

    void Start()
	{
		state = EnumType.EnemyState.Idle;   // 상태 초기화
		currStat = new EnemyStatsRuntime(GameManager.Instance.enemyStats);  // 각 스탯 초기화
	}

    // 데미지 입히기
    public void TakeDamage(int attack)
    {
        currStat.currentHP -= attack;

        if (currStat.currentHP <= 0)
            Die();
    }

    void Die()
    {
        // 풀로 반환
        ownerSpawner?.OnEnemyDead(this);
        GameManager.Instance.poolManager.ReturnToPool(gameObject);
    }
}
