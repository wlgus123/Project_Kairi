using UnityEngine;
public class FlexibleEnemy : MonoBehaviour
{
    [Header("감지 거리")]
    public float distance;
    [Header("조준 거리")]
    public float shootDistince;
    [Header("근거리 감지 거리")]
    public float atkDistince;
    [Header("감지 대상 레이어 (보통 Player 레이어)")]
    public LayerMask isLayer;
    [Header("이동 속도")]
    public float speed;
    [Header("총알이 생성 위치 오프셋 (적 기준)")]
    public Vector3 bulletPos;
    [Header("한 번 발사 후 다음 발사까지의 쿨타임")]
    public float coolTime;
    private float currentTime;
    [Header("감지 후 발사까지 대기 시간")]
    public float detectDelay = 1.0f;
    private float detectTimer = 0f;
    [Header("플레이어에게 붙는 조준 프리팹")]
    public GameObject aiming;
    [Header("벽 감지")]
    public float jumpForce = 1f;
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;

    private GameObject currentAiming;
    private Transform enemyTransform;
    private Transform targetPlayer;
    private Transform targetAimPoint;
    private float atkCooldown = 1f;     // 공격 타이머
    private float atkTimer = 0f;        // 공격 쿨다운
    private bool isAttacking = false;
    public bool isGrabbed = false;

    private void Start()
    {
        enemyTransform = transform;
    }

    private void FixedUpdate()
    {
        HandleRaycast();
        HandleGrabbed();
        HandleAttack();
    }
    public void HandleRaycast() // Raycast 설정
    {
        Vector2 rightDir = Vector2.right;
        Vector2 leftDir = Vector2.left;

        RaycastHit2D hitRight = Physics2D.CapsuleCast(
            transform.position,
            new Vector2(3f, 16f),
            CapsuleDirection2D.Vertical,
            0f,
            rightDir,
            distance,
            isLayer
        );

        RaycastHit2D hitLeft = Physics2D.CapsuleCast(
            transform.position,
            new Vector2(3f, 16f),
            CapsuleDirection2D.Vertical,
            0f,
            leftDir,
            distance,
            isLayer
        );

        RaycastHit2D raycast = hitRight.collider != null ? hitRight : hitLeft;

        if (raycast.collider != null && targetPlayer == null)
        {
            targetPlayer = raycast.collider.transform;
            targetAimPoint = targetPlayer.Find("AimPoint");
        }

        HandleDrawRaycast();
    }
    public void HandleDrawRaycast() // Raycast 시각화
    {
        Vector3 upOffset = Vector3.up;
        Vector3 downOffset = Vector3.down;
        Debug.DrawRay(transform.position + upOffset, Vector2.right * distance, Color.red);
        Debug.DrawRay(transform.position + downOffset, Vector2.right * distance, Color.red);
        Debug.DrawRay(transform.position + upOffset, Vector2.left * distance, Color.blue);
        Debug.DrawRay(transform.position + downOffset, Vector2.left * distance, Color.blue);
    }

    public void HandleGrabbed()     
    {
        if (isGrabbed)          // 플레이어에게 잡혔을 때
        {
            ResetAttackState(); // 에이밍 UI 제거
            return;
        }
    }
    public void HandleAttack()
    {
        if (atkTimer > 0f)
            atkTimer -= Time.deltaTime;     // 근거리 공격 쿨타임 감소

        if (targetPlayer != null && targetAimPoint != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.position);

            if (dist <= atkDistince) // 근거리
            {
                ResetAttackState(); // 조준, 공격  리셋

                float dirX = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                Vector2 moveDir = new Vector2(dirX, 0f);

                RaycastHit2D wallHit = Physics2D.Raycast(
                    transform.position,
                    moveDir,
                    wallCheckDistance,
                    wallLayer
                );

                if (wallHit.collider != null && IsGrounded())
                    Jump();
                else
                {
                    Vector3 targetPos = new Vector3(
                        targetPlayer.position.x,
                        transform.position.y,
                        transform.position.z
                    );

                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPos,
                        Time.deltaTime * speed
                    );
                }

                return; // 사격 방지
            }

            if (isAttacking && dist > shootDistince)    // 원거리
            {
                ResetAttackState();
                return;
            }

            if (dist < shootDistince || isAttacking)
            {
                if (dist < shootDistince)
                {
                    Vector2 origin = enemyTransform.position;
                    Vector2 dir = (targetAimPoint.position - enemyTransform.position).normalized;
                    float rayDistance = Vector2.Distance(origin, targetAimPoint.position);

                    int mask = ~LayerMask.GetMask("Player", "Enemy");

                    RaycastHit2D hit = Physics2D.Raycast(
                        origin,
                        dir,
                        rayDistance,
                        mask
                    );
                }

                detectTimer += Time.deltaTime;

                if (detectTimer < detectDelay)
                {
                    float remain = detectDelay - detectTimer;
                    CreateAiming(targetPlayer);
                }
                else
                {
                    isAttacking = true;

                    if (currentTime <= 0)
                    {
                        GameManager.Instance.audioManager.EnemyShootSound(1f);      // 발사 사운드 재생
                        GameManager.Instance.poolManager.SpawnFromPool("Rock", transform.position + bulletPos, transform.rotation);     // Rock 소환
                        currentTime = coolTime;
                    }
                }
            }
            else
            {
                float dirX = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                Vector2 moveDir = new Vector2(dirX, 0f);

                // 벽 체크
                RaycastHit2D wallHit = Physics2D.Raycast(
                    transform.position,
                    moveDir,
                    wallCheckDistance,
                    wallLayer
                );

                Debug.DrawRay(transform.position, moveDir * wallCheckDistance, Color.green);

                if (wallHit.collider != null && IsGrounded())
                    Jump();
                else
                {
                    Vector3 targetPos = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
                }
            }
            if (currentTime > 0)
                currentTime -= Time.deltaTime;
        }
    }
    bool IsGrounded()
    {
        return GetComponent<EnemyController>().isGrounded;      // 땅 체크 함수 호출
    }

    void Jump()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ResetAttackState()
    {
        detectTimer = 0f;
        RemoveAiming();
        isAttacking = false;
    }

    public void ResetAfterThrown()
    {
        isGrabbed = false;
        isAttacking = false;

        // 타겟을 null로 만들면 적이 다시 플레이어를 감지(Raycast)해야 공격을 시작합니다.
        // 만약 던져지자마자 다시 공격하게 하고 싶다면 targetPlayer는 유지하는게 좋습니다.
        targetPlayer = null;
        targetAimPoint = null;

        detectTimer = 0f;
        currentTime = 0f; // 즉시 발사 가능하도록 0으로 설정

        RemoveAiming();
    }


    void CreateAiming(Transform player)
    {
        if (currentAiming == null && targetAimPoint != null)
        {
            currentAiming = Instantiate(aiming);
            currentAiming.transform.SetParent(targetAimPoint);
            currentAiming.transform.localPosition = Vector3.zero;
            currentAiming.transform.localRotation = Quaternion.identity;
        }
    }

    void RemoveAiming()
    {
        if (currentAiming != null)
        {
            Destroy(currentAiming);
            currentAiming = null;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (atkTimer <= 0f)
        {
            GameManager.Instance.audioManager.EnemyAttackSound(1f);      // 근거리 사운드 재생
            GameManager.Instance.playerController.TakeDamage(1);
            Debug.Log("플레이어 근거리 피격");
            atkTimer = atkCooldown;
        }
    }


    private void OnDisable()
    {
        RemoveAiming();
        targetPlayer = null;
        targetAimPoint = null;
        isAttacking = false;
    }

    private void OnDestroy()
    {
        RemoveAiming();
        targetPlayer = null;
        targetAimPoint = null;
        isAttacking = false;
    }
}
