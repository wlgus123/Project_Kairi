using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class PlayerController : MonoBehaviour, IDamageable
{
    public bool isGrounded;
    public bool hasCollided = false;

    public Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    GrapplingHook grappling;

	PlayerInteraction interaction;  // 상호작용

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        grappling = GetComponent<GrapplingHook>();
        interaction = GetComponent<PlayerInteraction>();
    }
    void Start()
    {
        isGrounded = true;
    }
    void FixedUpdate()
    {
        if (interaction && interaction.GetIsAction()) return;

        float speed = GameManager.Instance.playerStatsRuntime.speed;

        if (grappling.isAttach) // 훅 매달림
        {
            Vector2 hookPos = grappling.hook.position;
            Vector2 playerPos = transform.position;

            // 훅 -> 플레이어 방향벡터
            Vector2 dir = (playerPos - hookPos).normalized;

            float inputX = inputVec.x;

            if (inputX > 0.1f)  // 오른쪽 방향키 = 항상 반시계 방향
            {
                Vector2 counterClockwise = new Vector2(-dir.y, dir.x);
                rigid.AddForce(counterClockwise * GameManager.Instance.playerStatsRuntime.hookSwingForce);
            }
            else if (inputX < -0.1f) // 왼쪽 방향키 = 항상 시계 방향
            {
                Vector2 clockwise = new Vector2(dir.y, -dir.x);
                rigid.AddForce(clockwise * GameManager.Instance.playerStatsRuntime.hookSwingForce);
            }

            // 최대 속도 제한
            if (rigid.linearVelocity.magnitude > GameManager.Instance.playerStatsRuntime.maxSwingSpeed)
            {
                rigid.linearVelocity = rigid.linearVelocity.normalized * GameManager.Instance.playerStatsRuntime.maxSwingSpeed;
            }

        }
        else // 일반 이동
        {
            float x = inputVec.x * speed * Time.deltaTime; // translate
            transform.Translate(x, 0, 0);
            Debug.Log("Grounded : " + isGrounded);

        }

        // 방향 플립
        if (inputVec.x > 0)
            sprite.flipX = false;
        else if (inputVec.x < 0)
            sprite.flipX = true;

		// 플레이어가 그래플링 훅 사용 중일 때
		if (grappling.isAttach)
			isGrounded = false;     // 바닥에 있지 X

    }

    void OnJump()
	{
		// 그래플링 사용 중 점프 시
		if (grappling.isAttach) return;

		// 플레이어가 바닥이 아닐 경우
		if (!isGrounded) return;

        GameManager.Instance.audioManager.PlayJumpSound(1f);
        rigid.AddForce(Vector2.up * GameManager.Instance.playerStatsRuntime.jumpForce, ForceMode2D.Impulse);

        isGrounded = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
	{
		// 바닥 체크
		foreach (var contact in collision.contacts)
		{
			if (contact.normal.y > 0.7f &&
				contact.point.y < transform.position.y)
			{
				isGrounded = true;
                rigid.linearVelocity = new Vector2(0f, rigid.linearVelocityY);
				break;
			}
		}

        // 충돌 체크
        hasCollided = true;

        //if (isGrounded && rigid.linearVelocityY < 0f)
        //{
        //	rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
        //}

    }


	void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 플레이어 데미지
    void IDamageable.TakeDamage(int attack)
    {
        // 플레이어 체력 줄어들기
        GameManager.Instance.playerStatsRuntime.currentHP -= attack;
    }

}
