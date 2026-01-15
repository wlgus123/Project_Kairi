using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public float distance;
    public LayerMask isLayer;

    private Vector2 moveDir; // 총알 이동 방향

    void Start()
    {
        // 발사 시점에 플레이어 방향 계산
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            moveDir = (player.transform.position - transform.position).normalized;
        }
        else
        {
            // 플레이어 없으면 기존 방향
            moveDir = -transform.right;
        }

        Invoke("DestroyBullet", 2f);
    }

    void Update()
    {
        // 이동
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

        // 충돌 체크
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir, distance, isLayer);
        if (raycast.collider != null)
        {
            if (raycast.collider.CompareTag("Player"))
            {
                Debug.Log("당했다!");
                // TODO: 플레이어 피해 처리
            }
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
