using UnityEngine;
using tagName = Globals.TagName;

public class EnemyBullet : MonoBehaviour
{
    [Header("속도")]
    public float speed = 8f;
    [Header("생존 시간")]
    public float lifeTime = 2f;
    private Vector2 moveDir;

    void OnEnable()
    {
        GameObject player = GameObject.FindGameObjectWithTag(tagName.player);       // 발사 순간 플레이어 방향 고정

        if (player != null)
            moveDir = (player.transform.position - transform.position).normalized;
        else
            moveDir = transform.right;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Invoke(nameof(ReturnToPool), lifeTime);     // 생존 시간 후 풀로 반환
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagName.player))
        {
            Debug.Log("플레이어 Bullet 피격");
            GameManager.Instance.playerController.TakeDamage(1);
            ReturnToPool();
        }

        if (!other.isTrigger && !other.CompareTag(tagName.player) && !other.CompareTag(tagName.enemy) && !other.CompareTag(tagName.bullet))
            ReturnToPool();
    }

    void ReturnToPool()
    {
        GameManager.Instance.poolManager.ReturnToPool(gameObject);
    }

    void OnDisable()
    {
        CancelInvoke();     // 풀에서 다시 꺼낼 때 중복 Invoke 방지
    }
}
