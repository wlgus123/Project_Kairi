using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

using tagName = Globals.TagName;
using hookVal = Globals.HookValue;

public class ThrowHook : MonoBehaviour
{
    [Header("그래플링 훅 갈고리 프리펩")]
    public GameObject hook;
    public bool isHookActive;   // 훅 활성화 여부

    [HideInInspector] public Vector2 hitPoint;

    Camera mainCam;         // 메인 카메라
    GameObject curHook;     // 현재 훅
    float distance;         // 발사 훅 길이

    private void Start()
    {
        distance = GameManager.Instance.playerStats.hookDistance;
        mainCam = Camera.main;
    }

    private void Update()
    {
        // 마우스 좌클릭 시
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!isHookActive)  // 훅이 활성화되지 않았을 경우
            {
                Vector3 mouseScreen = Mouse.current.position.ReadValue();       // 스크린 좌표 구하기
                mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);    // z값 보정
                Vector2 worldPos = mainCam.ScreenToWorldPoint(mouseScreen); // 월드 좌표
                Vector2 dir = (worldPos - (Vector2)transform.position).normalized;              // 광선 방향
                LayerMask mask = LayerMask.GetMask(tagName.ground);                        // 레이케스트 땅만 맞출 수 있도록 마스크 생성
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, mask);  // 자기 위치에서 dir 방향으로 광선 발사

                hook.GetComponent<TestHooking>().HookMoveAction();      // 훅 움직이는 액션

                if (hit)
                {
                    TestHooking hooking;
                    Vector2 destiny = hit.point;  // Raycast로 쐈을 때 충돌된 위치
                    curHook = Instantiate(hook, transform.position, Quaternion.identity);   // 플레이어 위치에 훅 생성

                    hooking = curHook.GetComponent<TestHooking>();
                    hooking.destiny = destiny;

                    // 점 사이 거리를 고려하여 거리만큼의 점 갯수 구하기
                    float len = Vector2.Distance(transform.position, destiny);
                    hooking.lineLen = len;

                    isHookActive = true;    // 훅 활성 여부 변경
                }
            }
        }
        // 마우스를 땠을 때
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isHookActive)
            {
                Destroy(curHook);

                isHookActive = false;
            }
        }
    }
}
