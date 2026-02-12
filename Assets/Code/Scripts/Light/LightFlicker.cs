using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    Light2D light2D;

    public float cycleTime = 10f;      // 전체 주기
    public float blinkInterval = 0.1f; // 깜빡 간격
    public float lightOnIntensity = 2f;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // 10초 중 랜덤 시점 대기
            float randomTime = Random.Range(0f, cycleTime);
            yield return new WaitForSeconds(randomTime);

            // 2번 깜빡
            for (int i = 0; i < 2; i++)
            {
                light2D.color = new Color(
                    Random.value,
                    Random.value,
                    Random.value
                );
                light2D.intensity = 0f;
                yield return new WaitForSeconds(blinkInterval);
                light2D.intensity = lightOnIntensity;
                yield return new WaitForSeconds(blinkInterval);
            }

            // 남은 시간 대기
            float remainTime = cycleTime - randomTime;
            if (remainTime > 0f)
                yield return new WaitForSeconds(remainTime);
        }
    }
}
