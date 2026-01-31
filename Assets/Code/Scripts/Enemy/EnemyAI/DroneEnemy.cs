using UnityEngine;
using DG.Tweening;
public class DroneEnemy : MonoBehaviour
{
    [Header("조준 / 공격 거리 (감지 거리 포함)")]
    public float atkDistince;

    [Header("감지 대상 레이어")]
    public LayerMask isLayer;

    [Header("총알 오프셋")]
    public Vector3 bulletPos;

    [Header("발사 쿨타임")]
    public float coolTime;

    [Header("감지 후 발사까지 대기 시간")]
    public float detectDelay = 1.0f;

    [Header("발사 유지 시간")]
    public float attackWindowTime = 2f;

    [Header("조준선 깜빡임 속도")]
    public float blinkSpeed = 6f;

    [Header("조준 프리팹")]
    public GameObject aiming;

    [Header("공중 부유 효과")]
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 2f;

    private float currentTime;
    private float detectTimer;
    private float attackWindowTimer;

    private Transform targetPlayer;
    private Transform targetAimPoint;

    private GameObject currentAiming;
    private LineRenderer line;

    private bool blinking = false;
    public bool isGrabbed = false;
    private Tween floatTween;
    private Vector3 basePos;
    private float phaseOffset;

    private void Awake()
    {
        phaseOffset = Random.Range(0f, 10f);
    }

    private void Start()
    {
        basePos = transform.position;

        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.enabled = false;
        line.startColor = Color.red;
        line.endColor = Color.red;

        StartFloating();
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            ResetAttackState();
            return;
        }

        HandleRaycast();
        HandleAttack();
    }
    float startY;

    void StartFloating()
    {
        startY = transform.position.y;

        floatTween = DOTween.To(
            () => transform.position.y,
            y => transform.position = new Vector3(transform.position.x, y, transform.position.z),
            startY + floatAmplitude,
            1f / floatSpeed
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);
    }


    void HandleRaycast()
    {
        Vector2[] dirs = { Vector2.right, Vector2.left };

        foreach (var dir in dirs)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(
                transform.position,
                new Vector2(3f, 16f),
                CapsuleDirection2D.Vertical,
                0f,
                dir,
                atkDistince,
                isLayer
            );

            if (hit.collider != null)
            {
                targetPlayer = hit.collider.transform;
                targetAimPoint = targetPlayer.Find("AimPoint");
                return;
            }
        }

        // 범위 내 플레이어 없음
        targetPlayer = null;
        targetAimPoint = null;
        ResetAttackState();
    }

    void HandleAttack()
    {
        if (targetPlayer == null || targetAimPoint == null)
            return;

        line.enabled = true;
        CreateAiming();

        // 조준선 표시
        Vector2 origin = transform.position;
        line.SetPosition(0, origin);
        line.SetPosition(1, targetAimPoint.position);

        detectTimer += Time.deltaTime;

        if (detectTimer < detectDelay)
        {
            CreateAiming();
            blinking = (detectDelay - detectTimer) <= 1f;
        }
        else
        {
            line.enabled = false;
            blinking = false;

            attackWindowTimer += Time.deltaTime;

            if (currentTime <= 0f)
            {
                GameManager.Instance.audioManager.EnemyShootSound(1f);
                GameManager.Instance.poolManager.SpawnFromPool(
                    "Bullet",
                    transform.position + bulletPos,
                    transform.rotation
                );
                currentTime = coolTime;
                RemoveAiming();
            }

            if (attackWindowTimer >= attackWindowTime)
            {
                ResetAttackState();
            }
        }

        if (currentTime > 0f)
            currentTime -= Time.deltaTime;

        if (blinking)
        {
            line.enabled = Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0;
        }
    }

    public void ResetAttackState()
    {
        detectTimer = 0f;
        attackWindowTimer = 0f;
        currentTime = 0f;
        blinking = false;
        line.enabled = false;
        RemoveAiming();
    }

    void CreateAiming()
    {
        if (currentAiming == null && targetAimPoint != null)
        {
            currentAiming = Instantiate(aiming, targetAimPoint);
            currentAiming.transform.localPosition = Vector3.zero;
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
    private void OnDisable()
    {
        RemoveAiming();
        targetPlayer = null;
        targetAimPoint = null;
        floatTween?.Kill();
        ResetAttackState();
    }

    private void OnDestroy()
    {
        RemoveAiming();
        targetPlayer = null;
        targetAimPoint = null;
        floatTween?.Kill();
        ResetAttackState();
    }
}
