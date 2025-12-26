using UnityEngine;
using System.Collections;

public class SwingBoostController : MonoBehaviour
{
    public float boostMultiplier = 1.5f;  // 속도 증가 배율
    public float boostDuration = 0.5f;     // Boost 지속 시간

    private Coroutine currentBoost;

    PlayerController player;
    SwingGaugeController swing;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        swing = GetComponent<SwingGaugeController>();
    }

    // GrapplingHook에서 훅을 놓을 때 호출
    public void Boost()
    {
        if (currentBoost != null)
            StopCoroutine(currentBoost);

        currentBoost = StartCoroutine(BoostRoutine());
    }

    private IEnumerator BoostRoutine()
    {
        var stats = GameManager.Instance.playerStatsRuntime;

        float originalSpeed = stats.speed;   // 현재 speed 저장

        float boostFactor = 1 + (boostMultiplier - 1) * swing.GetGaugePercent();  // 게이지 비례 배율 계산
        stats.speed = originalSpeed * boostFactor;    // 속도 증가

        float boostTime = 0f; // Boost 지속 시간 측정을 위한 타이머 변수

        while (boostTime < boostDuration)
        {
            if (player.hasCollided) // 어떤 충돌이든 즉시 종료
                break;

            boostTime += Time.deltaTime;
            yield return null;
        }

        stats.speed = originalSpeed;    // 원래 속도로 복귀
        player.hasCollided = false;     // 충돌 플래그 초기화
        currentBoost = null;
    }
}
