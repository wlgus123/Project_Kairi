using UnityEngine;
using System.Collections;
using tagName = Globals.TagName;    // 태그
using static Globals;

public class ObjectController : MonoBehaviour
{
	GrabbableObject obj;
    Rigidbody2D rigid;
    public bool explosion;
    public GameObject explosionEffectPrefab;
    public bool isGrounded;
    public bool hasCollided = false;

    private void Awake()
	{
        rigid = GetComponent<Rigidbody2D>();
        obj = GetComponent<GrabbableObject>();
	}

    void Start()
    {
        isGrounded = true;
    }
    void Update()
    {
        if (isGrounded && rigid.linearVelocity == Vector2.zero)
            gameObject.tag = "Object";
    }

    public void CheckGround(Collision2D collision)
    {
        // 바닥 체크
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f &&
                contact.point.y < transform.position.y)
            {
                isGrounded = true;
                break;
            }
        }

        // 충돌 체크
        hasCollided = true;

        // y값 보정 (바닥 뚫림 방지)
        if (isGrounded && rigid.linearVelocityY < 0f)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        CheckGround(other);     // 바닥 체크

        if (gameObject.CompareTag(tagName.throwingObj))
        {
            if (other.gameObject.CompareTag(tagName.enemy))     // 적과 닿았을 경우
            {
                if (other.gameObject.TryGetComponent<Enemy>(out var target))
                    target.TakeDamage(1);       // 닿은 적에게 데미지 주기
            }
        }

        if (explosion && other.gameObject.CompareTag(tagName.enemy))
        {
            if (other.gameObject.TryGetComponent<Enemy>(out var target))
            {
                target.TakeDamage(1);       // 닿은 적에게 데미지 주기
                Vector2 hitPoint = other.contacts[0].point;
                StartCoroutine(SpawnExplosionEffect(hitPoint));

                Destroy(gameObject); // 투척 오브젝트 제거
            }
        }
    }
    IEnumerator SpawnExplosionEffect(Vector2 position)
    {
        GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);

        yield return new WaitForSeconds(1.07f);

        Destroy(effect);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);     // 바닥 체크
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagName.ground))
            isGrounded = false;
    }

}
