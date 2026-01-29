using UnityEngine;
using System.Collections;
using tagName = Globals.TagName;    // 태그
using static Globals;

public class ObjectController : MonoBehaviour
{
    [Header("터지는 오브젝트")]
    public bool explosionObject;
    [Header("폭발 이펙트")]
    public GameObject explosionEffectPrefab;
    [Header("부서지는 오브젝트")]
    public bool crackObject = false;
    [Header("최대 내구도")]
    public int maxCount = 3;
    public int count;          // 현재 내구도
    [Header("닿으면 죽는 오브젝트")]
    public bool playerDieObject = false;
    public bool isGrounded;
    public bool hasCollided = false;
    Rigidbody2D rigid;
    SpriteRenderer sprite;

    private void Awake()
	{
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        count = maxCount;
        UpdateColor();
    }

    void Start()
    {
        isGrounded = true;
    }
    void Update()
    {
        if (isGrounded && rigid.linearVelocity == Vector2.zero)
            gameObject.tag = tagName.obj;
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

    void UpdateColor()
    {
        float ratio = (float)count / maxCount;

        if (ratio > 0.66f)          // 2/3 이상
            sprite.color = Color.white;   // 정상
        else if (ratio > 0.33f)     // 1/3 ~ 2/3
            sprite.color = Color.yellow;
        else if (ratio > 0f)        // 0 ~ 1/3
            sprite.color = Color.red;
        else                        // 파괴
        {
            if (explosionObject)
            {
                Vector2 thisObject = transform.position;
                StartCoroutine(SpawnExplosionEffect(thisObject));
                Destroy(gameObject);
            }   
            else
                Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (crackObject && collision.CompareTag(tagName.bullet))
        {
            count--;
            UpdateColor();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);     // 바닥 체크

        if (gameObject.CompareTag(tagName.throwingObj))
        {
            if (collision.gameObject.CompareTag(tagName.enemy))     // 적과 닿았을 경우
            {
                if (collision.gameObject.TryGetComponent<Enemy>(out var target))
                    target.TakeDamage(1);       // 닿은 적에게 데미지 주기
            }
        }

        if (explosionObject && collision.gameObject.CompareTag(tagName.enemy))
        {
            if (collision.gameObject.TryGetComponent<Enemy>(out var target))
            {
                target.TakeDamage(1);       // 닿은 적에게 데미지 주기
                Vector2 hitPoint = collision.contacts[0].point;
                StartCoroutine(SpawnExplosionEffect(hitPoint));

                Destroy(gameObject); // 투척 오브젝트 제거
            }
        }

        if (crackObject && collision.gameObject.CompareTag(tagName.throwingObj) || collision.gameObject.CompareTag(tagName.throwingEnemy))
        {
            count--;
            UpdateColor();
        }

        if (playerDieObject && collision.gameObject.CompareTag(tagName.player))
        {
            GameManager.Instance.playerController.TakeDamage(1000000);
            Debug.Log("낙사함 ㅅㄱ");
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
