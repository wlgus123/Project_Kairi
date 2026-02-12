using UnityEngine;
using UnityEngine.UI;

public class EnemyCameraMark : MonoBehaviour
{
    Camera mainCam;

    [Header("마커 프리팹 (Image)")]
    public RectTransform markerPrefab;

    RectTransform markerUI;
    Canvas markerCanvas;

    void Start()
    {
        mainCam = Camera.main;

        CreateMarkerCanvas();
        CreateMarker();
    }

    void CreateMarkerCanvas()
    {
        // 이미 존재하면 재사용
        GameObject canvasObj = GameObject.Find("EnemyMarkerCanvas");

        if (canvasObj == null)
        {
            canvasObj = new GameObject("EnemyMarkerCanvas");

            markerCanvas = canvasObj.AddComponent<Canvas>();
            markerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            markerCanvas.sortingOrder = 100;

            // Graphic Raycaster 끄기
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
            raycaster.enabled = false;

            // 해상도 대응용 (권장)
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<RectTransform>();
        }
        else
        {
            markerCanvas = canvasObj.GetComponent<Canvas>();
        }
    }

    void CreateMarker()
    {
        markerUI = Instantiate(markerPrefab, markerCanvas.transform);
        markerUI.gameObject.SetActive(true);

        // 중앙 기준 이동을 위해 Anchor 고정
        markerUI.anchorMin = new Vector2(0.5f, 0.5f);
        markerUI.anchorMax = new Vector2(0.5f, 0.5f);
        markerUI.pivot = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (markerUI == null) return;

        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);

        bool isOnScreen =
            viewportPos.x > 0 && viewportPos.x < 1 &&
            viewportPos.y > 0 && viewportPos.y < 1 &&
            viewportPos.z > 0;

        markerUI.gameObject.SetActive(!isOnScreen);

        if (!isOnScreen)
        {
            viewportPos.x = Mathf.Clamp01(viewportPos.x);
            viewportPos.y = Mathf.Clamp01(viewportPos.y);
            SetMarkerPosition(viewportPos);
        }
    }

    void SetMarkerPosition(Vector3 viewportPos)
    {
        RectTransform canvasRect = markerCanvas.GetComponent<RectTransform>();

        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 markerSize = markerUI.sizeDelta;

        // 화면 반 크기
        float halfW = canvasSize.x * 0.5f;
        float halfH = canvasSize.y * 0.5f;

        // 마커 반 크기
        float markerHalfW = markerSize.x * 0.5f;
        float markerHalfH = markerSize.y * 0.5f;

        // viewport -> canvas 좌표
        float x = (viewportPos.x - 0.5f) * canvasSize.x;
        float y = (viewportPos.y - 0.5f) * canvasSize.y;

        // 화면 안쪽으로 클램프
        x = Mathf.Clamp(x, -halfW + markerHalfW, halfW - markerHalfW);
        y = Mathf.Clamp(y, -halfH + markerHalfH, halfH - markerHalfH);

        markerUI.anchoredPosition = new Vector2(x, y);
    }

    void OnDestroy()
    {
        if (markerUI != null)
            Destroy(markerUI.gameObject);
    }
}
