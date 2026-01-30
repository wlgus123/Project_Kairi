using UnityEngine;
using tagName = Globals.TagName;

public class EnemyRock : MonoBehaviour
{
    [Header("생존 시간")]
    public float lifeTime = 3f;
    [Header("목표까지 도달 시간 (작을수록 빠르고 직선에 가까움)")]
    public float flyTime = 0.8f;

    private Vector2 velocity;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        GameObject player = GameObject.FindGameObjectWithTag(tagName.player);

        if (player != null)
        {
            Vector2 start = transform.position;
            Vector2 target = player.transform.position;

            velocity = CalculateParabolicVelocity(start, target, flyTime);
            rb.linearVelocity = velocity;
        }
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    Vector2 CalculateParabolicVelocity(Vector2 start, Vector2 target, float time)
    {
        float gravity = Physics2D.gravity.y;
        float vx = (target.x - start.x) / time;
        float vy = (target.y - start.y - 0.5f * gravity * time * time) / time;
        return new Vector2(vx, vy);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagName.player))
        {
            Debug.Log("플레이어 Rock 피격");
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
        CancelInvoke();
        rb.linearVelocity = Vector2.zero;
    }
}
