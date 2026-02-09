using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;
using playerState = EnumType.PlayerState;
using tagName = Globals.TagName;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Glitch Global Volume 오브젝트")]
    public GameObject glitchGlobalVolume;
    [Header("TV Global Volume 오브젝트")]
    public GameObject tvGlobalVolume;
    [Header("데미지 UI")]
    public GameObject damagedCanvas;
    [Header("검은 화면 UI")]
    public Image blackCanvas;
    [Header("땅에서 움직이지 않을 때 일정 시간 이후 Run에서 Idle")]
    public float maxTime;
    private float curTime;
    [Header("땅 체크")]
    public bool isGrounded;
    [Header("충돌 체크")]
    public bool hasCollided = false;
    [Header("걷기 사운드")]
    public float walkSoundInterval = 0.7f;
    private float walkSoundTimer = 0f;
    public bool isWalking = false;

    public Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    GrapplingHook grappling;
    PlayerInteraction interaction;  // 상호작용
    Animator animator;              // 애니메이션

    private Coroutine playerDieCoroutine;
    private Coroutine damageCanvasCoroutine;
    private Coroutine damagedColorCoroutine;
    private bool wasAttach;
 
    void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		sprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
        interaction = GameManager.Instance.playerInteraction;
        grappling = GameManager.Instance.grapplingHook;
	}

	void Start()
	{
		isGrounded = true;
        SetPlayerState(playerState.Idle);
	}

    void Update()
    {
        HandleTimelinePlaying();    // 타임라인 중 못 움직임
        UpdateAnimation();          // 애니메이션
        HandleWalkSound();          // 걷기 사운드
    }

    void FixedUpdate()
	{
		if (interaction && interaction.GetIsAction()) return;
        HandleMove();   // 플레이어 이동
        HandleFlip();   // 방향 플립
    }

	void OnMove(InputValue value)
	{
        if (TimelineController.isTimelinePlaying)
        {
            inputVec = Vector2.zero;
            return;   // 컷씬 재생 중일 때는 플레이어 컨트롤 불가
        }
        inputVec = value.Get<Vector2>();
	}

    void OnJump()
    {
        if (TimelineController.isTimelinePlaying)
        {
            inputVec = Vector2.zero;
            return;   // 컷씬 재생 중일 때는 플레이어 컨트롤 불가
        }
        if (!isGrounded) return;    // 플레이어가 바닥이 아닐 경우

        GameManager.Instance.audioManager.PlayJumpSound(1f);
        rigid.AddForce(Vector2.up * GameManager.Instance.playerStatsRuntime.jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    public void HandleTimelinePlaying()
    {
        if (TimelineController.isTimelinePlaying)
        {
            inputVec = Vector2.zero;
            return;   // 컷씬 재생 중일 때는 플레이어 컨트롤 불가
        }
    }

    public void HandleMove()    // 플레이어 이동
    {
        float speed = GameManager.Instance.playerStatsRuntime.speed;

        //if (!wasAttach && grappling.isAttach)       // 그래플 시작 순간
        //    rigid.linearVelocity = new Vector2(0f, rigid.linearVelocity.y); // 입력 방향으로 쌓인 속도만 수평 가속도 제거, 수직 가속도 유지

        //if (grappling.isAttach)
        //{
        //    float hookSwingForce = GameManager.Instance.playerStatsRuntime.hookSwingForce;
        //    rigid.AddForce(new Vector2(inputVec.x * hookSwingForce, 0f));

        //    if (rigid.linearVelocity.magnitude > GameManager.Instance.playerStatsRuntime.maxSwingSpeed)
        //        rigid.linearVelocity = rigid.linearVelocity.normalized * GameManager.Instance.playerStatsRuntime.maxSwingSpeed;
        //}
        //else
        //{
            float x = inputVec.x * speed * Time.deltaTime;
            transform.Translate(x, 0, 0);
        //}
        //wasAttach = grappling.isAttach;     // 상태 저장 (맨 마지막!)
    }

    void HandleWalkSound()
    {
        if (!isWalking)
        {
            walkSoundTimer = 0f; // 멈추면 타이머 리셋
            return;
        }

        walkSoundTimer += Time.deltaTime;

        if (walkSoundTimer >= walkSoundInterval)
        {
            GameManager.Instance.audioManager.PlayWalkSound(1f);
            walkSoundTimer = 0f;
        }
    }

    public void HandleFlip()    // 방향 플립
    {
        if (inputVec.x > 0) 
            sprite.flipX = false;
        else if (inputVec.x < 0) 
            sprite.flipX = true;
    }

    public void TakeDamage(int attack)      // 플레이어 데미지
    {
        GameManager.Instance.audioManager.PlayDamagedSound(1f);         // 데미지 사운드 재생
        GameManager.Instance.playerStatsRuntime.currentHP -= attack;    // 체력 감소

        if (GameManager.Instance.playerStatsRuntime.currentHP <= 0)     // 체력이 0 이하일 때
        {
            if (playerDieCoroutine == null)
                playerDieCoroutine = StartCoroutine(PlayerDie());
            return;
        }

        // 이미 실행 중이면 중단 (연속 피격 대응)
        if (damageCanvasCoroutine != null)
            StopCoroutine(damageCanvasCoroutine);

        if (damagedColorCoroutine != null)
            StopCoroutine(damagedColorCoroutine);

        damageCanvasCoroutine = StartCoroutine(ShowDamagedCanvas());
        damagedColorCoroutine = StartCoroutine(PlayerDamagedColor());
    }

    IEnumerator PlayerDie()             // 데미지 UI 코루틴
    {
        if (glitchGlobalVolume != null & tvGlobalVolume)
        {
            glitchGlobalVolume.SetActive(true);
            tvGlobalVolume.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            blackCanvas.gameObject.SetActive(true);
            GameManager.Instance.sceneReloader.SetAlpha(1f);
            yield return new WaitForSeconds(0.5f);
            Destroy(glitchGlobalVolume.gameObject);
            Destroy(tvGlobalVolume.gameObject);
            GameManager.Instance.sceneReloader.Reload();    // 씬 리로드
        } 
    }

    IEnumerator ShowDamagedCanvas()             // 데미지 UI 코루틴
    {
        damagedCanvas.SetActive(true);
        yield return new WaitForSeconds(1f);
        damagedCanvas.SetActive(false);
    }

    IEnumerator PlayerDamagedColor()            // 데미지 플레이어 색 변경
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white;
    }

    public void CheckGround(Collision2D collision)      // 바닥 체크
    {
		foreach (var contact in collision.contacts)
		{
			if (contact.normal.y > 0.7f &&
				contact.point.y < transform.position.y)
			{
				isGrounded = true;
				break;
			}
		}	
		hasCollided = true;     // 충돌 체크

        if (isGrounded && rigid.linearVelocityY < 0f)       // y값 보정 (바닥 뚫림 방지)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
	}

	void SetPlayerState(playerState state)      // 플레이어 상태 변경
    {
		animator.SetInteger(Globals.AnimationVarName.playerState, (int)state);
	}
	
	void UpdateAnimation()      // 애니메이션 업데이트
    {
        if (isGrounded)
		{
            bool hasMoveInput = Mathf.Abs(inputVec.x) > 0.01f;      // 플레이어가 가만히 있을 때

            if (!hasMoveInput)
            {
                curTime += Time.deltaTime;

                if (curTime >= maxTime)
                    SetPlayerState(playerState.Idle);
                isWalking = false;
            }
            else
            {
                SetPlayerState(playerState.Run);
                curTime = 0f;
                isWalking = true;
            }
        }
		else
        {
            SetPlayerState(playerState.Idle);
            isWalking = false;
        }
    }
   
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);     // 바닥 체크
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