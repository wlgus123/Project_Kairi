using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

using tagName = Globals.TagName;

public class TestGrapplingHook : MonoBehaviour
{
	[Header("그래플링 훅 갈고리 프리펩")]
	public GameObject hook;

	[Header("임시 표시선 visualizerLine 프리펩")]
	public GameObject visualizerLine;

	private Camera mainCam;     // 메인 카메라

	/* 훅 정보 */
	[HideInInspector] public bool isAttach;		// 훅 사용 여부
	[HideInInspector] public bool isGrab;       // 훅 잡음 여부
	private GameObject curHook;     // 현재 훅
	private float distance;             // 훅 길이

	/* 임시 표시선 */
	private LineRendererAtoB lineAtoB;  // 임시 표시선 관련 데이터

	private void Start()
	{
		/* 훅 정보 */
		isAttach = false;
		isGrab = false;

		/* 임시 표시선 */
		lineAtoB = Instantiate(visualizerLine).GetComponent<LineRendererAtoB>();    // 인스턴스화 시킨 오브젝트의 스크립트 컴포넌트 저장하기
		distance = GameManager.Instance.playerStatsRuntime.hookDistance;			// 표시선 길이 불러오기
		
		/* 카메라 */
		mainCam = Camera.main;		// 메인 카메라 정보 가져오기
	}

	private void Update()
	{
		CursorPathMarking();    // 임시 표시선 그리기
		ActiveHook();			// 훅 사용
	}

	// 훅 사용
	private void ActiveHook()
	{
		// 마우스 좌클릭 시
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			if (!isAttach)  // 훅이 활성화되지 않았을 경우
			{
				Vector3 mouseScreen = Mouse.current.position.ReadValue();       // 스크린 좌표 구하기
				mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);        // z값 보정
				Vector2 worldPos = mainCam.ScreenToWorldPoint(mouseScreen);     // 월드 좌표
				Vector2 dir = (worldPos - (Vector2)transform.position).normalized;              // 광선 방향
				LayerMask mask = LayerMask.GetMask(tagName.ground);                             // 레이케스트 땅만 맞출 수 있도록 마스크 생성
				RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, mask);  // 자기 위치에서 dir 방향으로 광선 발사

				hook.GetComponent<TestHooking>().HookMoveAction();      // 훅 움직이는 액션

				if (hit)
				{
					TestHooking hooking;
					Vector2 destiny = hit.point;    // Raycast로 쐈을 때 충돌된 위치
					curHook = Instantiate(hook, transform.position, Quaternion.identity);   // 플레이어 위치에 훅 생성

					hooking = curHook.GetComponent<TestHooking>();
					hooking.destiny = destiny;

					// 점 사이 거리를 고려하여 거리만큼의 점 갯수 구하기
					float len = Vector2.Distance(transform.position, destiny);
					hooking.lineLen = len;

					isAttach = true;    // 훅 활성 여부 변경
					
				}
			}
		}
		// 마우스를 뗐을 때
		else if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			if (isAttach)
			{
				Destroy(curHook);

				isAttach = false;
			}
		}
	}

	// 임시 표시선 그리기
	public void CursorPathMarking()
	{
		if (Mouse.current == null) return;
		if (GameManager.Instance.dialogSystem && GameManager.Instance.dialogSystem.isAction) return;    // 상호작용 중일 경우 표시선 그리지 않음

		Vector3 mouseScreen = Mouse.current.position.ReadValue();                       // 스크린 좌표 구하기
		mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);                        // z값 보정
		Vector2 worldPos = mainCam.ScreenToWorldPoint(mouseScreen);                     // 월드 좌표
		Vector2 dir = (worldPos - (Vector2)transform.position).normalized;              // 광선 방향
		LayerMask mask = ~LayerMask.GetMask(tagName.player);                            // 레이케스트 플레이어 충돌 무시
		RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, mask);  // 자기 위치에서 dir 방향으로 광선 발사

		if (isAttach || isGrab)   // 훅 사용 중일 경우 선 비활성화
		{
			lineAtoB.Stop();
			return;
		}

		if (hit)        // 광선에 부딪히는 오브젝트가 있으면 선 활성화
		{
			if (hit.collider.CompareTag(tagName.npc))   // 부딪힌 요소가 NPC일 경우 선 비활성화
			{
				lineAtoB.Stop();
				return;
			}

			// 부딪힌 요소에 따라 선 색상 변경
			// 뭔가를 들고 있을 때 오브젝트나 몬스터가 부딪혔을 경우
			if (isGrab && (hit.collider.CompareTag(tagName.enemy) || hit.collider.CompareTag(tagName.obj)))
				lineAtoB.SetLineColor(new Color(1f, 0.2f, 0.2f));
			else if (hit.collider.CompareTag(tagName.obj))
				lineAtoB.SetLineColor(new Color(0.49f, 0.85f, 0.45f));
			else
				lineAtoB.SetLineColor(new Color(0.18f, 0.76f, 1f));

			lineAtoB.Play(transform.position, hit.point);
		}
		else
			lineAtoB.Stop();
	}
}
