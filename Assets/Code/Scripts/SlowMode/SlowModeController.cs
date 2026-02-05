using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class SlowModeController : MonoBehaviour
{

    [Header("슬로우 게이지 UI")]
    public Slider slowGaugeSlider;
    [Header("슬로우 비율")]
    public float slowFactor;
    [Header("슬로우 게이지 최대치")]
    public float slowMaxGauge;
    [Header("슬로우 게이지 현재치")]
    public float slowGauge;
    [Header("슬로우 게이지 감소 속도")]
    public float slowDecreaseRate;
    [Header("슬로우 게이지 회복 속도")]
    public float slowRecoverRate;
    [Header("슬로우 상태")]
    public bool isSlow = false;

    [Header("플레이어")]
    public SpriteRenderer playerSprite;

    [Header("적들")]
    public List<SpriteRenderer> enemySprites = new List<SpriteRenderer>();

    [Header("배경들")]
    public List<SpriteRenderer> backgroundSprites = new List<SpriteRenderer>();

    void Update()
    {
        if (TimelineController.isTimelinePlaying) return;
        HandleSlowMode();           // 슬로우 모드
        UpdateSlowGauge();	        // 슬로우 게이지 업데이트
    }

    public void HandleSlowMode()        // 슬로우 모드
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            if (!isSlow && slowGauge > 0f) StartSlow();
            else StopSlow();
        }
    }

    void UpdateSlowGauge()      // 슬로우 게이지 업데이트
    {
        if (isSlow)
        {
            slowGauge -= slowDecreaseRate * Time.unscaledDeltaTime;

            if (slowGauge <= 0f)
            {
                slowGauge = 0f;
                StopSlow(); // 자동 해제
            }
        }
        else
        {
            slowGauge += slowRecoverRate * Time.unscaledDeltaTime;
            if (slowGauge > slowMaxGauge)
                slowGauge = slowMaxGauge;
        }
        slowGaugeSlider.value = slowGauge / slowMaxGauge;
    }

    void StopSlow()     // 슬로우 효과 종료
    {
        if (!isSlow) return;
        isSlow = false;
        Time.timeScale = 1f;            // 시간 원래대로
        Time.fixedDeltaTime = 0.02f;

        ApplyNormalColor();
    }

    void StartSlow()    // 슬로우 효과 시작
    {
        if (isSlow) return;
        isSlow = true;
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        ApplySlowColor();
    }

    void ApplySlowColor()   // 슬로우 ON
    {
        //ApplySlowColorToPlayer();
        //ApplySlowColorToEnemies();
        ApplySlowColorToBackgrounds();
    }
    
    void ApplyNormalColor() // 슬로우 OFF
    {
        if (playerSprite)
            playerSprite.color = Color.white;

        foreach (var enemy in enemySprites)
            if (enemy)
                enemy.color = Color.white;

        foreach (var bg in backgroundSprites)
            if (bg)
                bg.color = Color.white;
    }

    void ApplySlowColorToPlayer()
    {
        if (playerSprite)
            playerSprite.color = BoostSaturation(playerSprite.color);
    }

    void ApplySlowColorToEnemies()
    {
        foreach (var enemy in enemySprites)
            if (enemy)
                enemy.color = BoostSaturation(enemy.color);
    }

    void ApplySlowColorToBackgrounds()
    {
        foreach (var bg in backgroundSprites)
            if (bg)
                bg.color = ReduceSaturation(bg.color);
    }

    Color BoostSaturation(Color original)
    {
        return original;
    }

    Color ReduceSaturation(Color original)  // 채도 감소
    {
        float h, s, v;
        Color.RGBToHSV(original, out h, out s, out v);

        s = 0f; // 채도 완전 제거 -> 회색 계열
        v *= 0.6f;  // 약간 어둡게

        Color c = Color.HSVToRGB(h, s, v);

        // ★ 톤을 조금 더 죽여서 배경이 확실히 흐려짐
        c.r *= 0.9f;
        c.g *= 0.9f;
        c.b *= 0.9f;

        return c;
    }
}
