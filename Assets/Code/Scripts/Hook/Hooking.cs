using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Globals;

using hookVal = Globals.HookValue;

public class Hooking : MonoBehaviour
{
	[Header("훅")]
	public Vector2 destiny;
	public float speed = 1f;            // 훅 발사 속도 (TODO: 스크립터블 오브젝트에 있는 speed로 사용하기)

	[Header("중력")]
	public Vector2 gravityForce = new Vector2(0f, -2f);     // 로프 중력값
	public float dampingFactor = 0.99f;     // 제동 계수 (과도한 흔들림 제어용)

	[Header("제약 조건")]
	public int constraintRuns = 50;    // 실행 횟수

	[Header("노드 프리펩")] public GameObject nodePrefab;   // 노드 프리펩

	[HideInInspector] public GameObject player;             // 플레이어 오브젝트
	[HideInInspector] public GameObject lastNode;           // 마지막에 생성한 노드
	[HideInInspector] public LineRenderer line;
	[HideInInspector] public int segmentCnt;    // 점 갯수
	[HideInInspector] public float lineLen;    // 줄 길이
	[HideInInspector] public List<GameObject> nodeList = new List<GameObject>();  // 노드 리스트

	private List<HookSegment> hookSegments = new List<HookSegment>();
	private Vector3 ropeStartPoint;     // 줄 시작점

	private void Awake()
	{
		line = GetComponent<LineRenderer>();
	}

	private void Start()
	{
		segmentCnt = (int)(lineLen / hookVal.segmentLen);
		line.positionCount = segmentCnt;

		player = GameObject.FindGameObjectWithTag(TagName.player);    // 플레이어 태그로 정보 불러오기
		lastNode = transform.gameObject;    // 마지막 노드를 자기 자신으로 설정

		ropeStartPoint = player.transform.position;     // 로프 시작점 설정(플레이어 위치)

		lastNode = transform.gameObject;    // 마지막 태그를 자기 자신으로 설정
		nodeList.Add(transform.gameObject);

		for (int i = 0; i < segmentCnt; i++)
		{
			hookSegments.Add(new HookSegment(ropeStartPoint));
			ropeStartPoint.y -= hookVal.segmentLen;
		}


	}

	private void Update()
	{
		transform.position = Vector2.MoveTowards(transform.position, destiny, speed);
	
		RenderLine();
	}

	private void FixedUpdate()
	{
		// 줄 위치 업데이트
		Simulate();

		for (int i = 0; i < constraintRuns; i++)
			ApplyContraints();

		// 플레이어 위치 보정
		Vector2 toPlayer = (Vector2)player.transform.position - destiny;
		float ropeLength = lineLen;

		if (toPlayer.magnitude > ropeLength)
		{
			// 플레이어 위치를 로프 길이 안으로 강제 보정
			Vector2 clampedPos = destiny + toPlayer.normalized * ropeLength;
			player.transform.position = clampedPos;

			// 로프 방향 속도 제거 (늘어짐 방지 핵심)
			Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
			rb.linearVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, toPlayer.normalized);
		}
	}

	// 선 그리기
	void RenderLine()
	{
		// 세그먼트 갯수와 세그먼트 리스트 갯수가 다를 경우 리스트 초기화
		if (segmentCnt != hookSegments.Count)
		{
			hookSegments.Clear();

			for (int i = 0; i < segmentCnt; i++)
			{
				hookSegments.Add(new HookSegment(ropeStartPoint));
				ropeStartPoint.y -= hookVal.segmentLen;
			}
		}

		Vector3[] ropePos = new Vector3[segmentCnt];
		for (int i = 0; i < hookSegments.Count; i++)
		{
			ropePos[i] = hookSegments[i].CurrPos;
		}

		line.SetPositions(ropePos);
	}

	// 줄 구체화
	private void Simulate()
	{
		for (int i = 1; i < hookSegments.Count; i++)
		{
			HookSegment segment = hookSegments[i];
			Vector2 velocity = (segment.CurrPos - segment.OldPos) * dampingFactor;

			segment.OldPos = segment.CurrPos;
			segment.CurrPos += velocity;
			segment.CurrPos += gravityForce * Time.fixedDeltaTime * Time.fixedDeltaTime;
			hookSegments[i] = segment;  // 현재 세그먼트 리스트에 적용하기
		}
	}

	// 세그먼트 위치 조정 (Verlet 적산법 사용)
	private void ApplyContraints()
	{
		HookSegment firstSegment = hookSegments[0];         // 첫 번째 세그먼트
		HookSegment lastSegment = hookSegments[hookSegments.Count - 1]; // 마지막 세그먼트
		firstSegment.CurrPos = player.transform.position;   // 첫 번째 세그먼트는 플레이어 위치
		lastSegment.CurrPos = destiny;      // 마지막 세그먼트는 라인으로 충돌된 위치
		hookSegments[0] = firstSegment;     // 현재 세그먼트 리스트에도 반영
		hookSegments[hookSegments.Count - 1] = lastSegment;

		for (int i = 0; i < segmentCnt - 1; i++)
		{
			HookSegment currSeg = hookSegments[i];
			HookSegment nextSeg = hookSegments[i + 1];

			float dist = (currSeg.CurrPos - nextSeg.CurrPos).magnitude;   // 두 세그먼트 사이 거리 계산
			float difference = (dist - hookVal.segmentLen);  // 세그먼트 길이 차이 계산

			Vector2 changeDir = (currSeg.CurrPos - nextSeg.CurrPos).normalized;     // 변경할 세그먼트 방향 정규화
			Vector2 changeVector = changeDir * difference;                          // 변경할 세그먼트 위치 벡터값 계산

			if (i == 0)    // 첫 번째 세그먼트일 경우 전체 보정값을 다음 세그먼트에 적용
			{
				nextSeg.CurrPos += changeVector;
			}
			else if (i == segmentCnt - 2)   // 마지막 세그먼트일 경우
			{
				currSeg.CurrPos -= changeVector;
			}
			else  // 첫 번째 세그먼트가 아닐 경우 해당 세그먼트와 다음 세그먼트에 수정값을 분배
			{
				currSeg.CurrPos -= (changeVector * 0.5f);
				nextSeg.CurrPos += (changeVector * 0.5f);
			}
			hookSegments[i] = currSeg;  // 현재 세그먼트 리스트에 반영
			hookSegments[i + 1] = nextSeg;
		}
	}

	// 세그먼트 구조체
	public struct HookSegment
	{
		public Vector2 CurrPos;     // 현재 세그먼트 위치
		public Vector2 OldPos;      // 이전 세그먼트 위치

		public HookSegment(Vector2 pos)
		{
			CurrPos = pos;
			OldPos = pos;
		}
	}
}
