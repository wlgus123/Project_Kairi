using UnityEngine;

public class CreateHookLine : MonoBehaviour
{
	int segmentsCount;

	public Transform pointA;
	public Transform pointB;

	public HingeJoint2D hingePrefeb;

	[HideInInspector] public Transform[] segments;

	// 세그먼트 위치 가져오기
	Vector2 GetSegmentPosition(int segmentIdx)
	{
		Vector2 posA = pointA.position;
		Vector2 posB = pointB.position;

		float fraction = 1f / (float)segmentsCount;
		return Vector2.Lerp(posA, posB, fraction * segmentIdx);
	}

	int GetSegmentCnt()
	{
		return (int)Vector2.Distance(pointA.position, pointB.position);
	}

	// 로프 생성
	void GenerateRope()
	{
		segments = new Transform[segmentsCount];

		for (int i = 0; i < segmentsCount; i++)
		{
			// 로프 세그먼트 인스턴스화
			var currJoint = Instantiate(hingePrefeb, GetSegmentPosition(i), Quaternion.identity, this.transform);
			segments[i] = currJoint.transform;

			// 각 세그먼트마다 이전 세그먼트 리지드바디를 연결하기 (첫 번째 세그먼트 제외)
			if (i == 0)
			{
				currJoint.connectedBody = pointA.GetComponent<Rigidbody2D>();
			}
			else if (i > 0)
			{
				int prevIdx = i - 1;
				currJoint.connectedBody = segments[prevIdx].GetComponent<Rigidbody2D>();
			}
		}
	}

	// (디버그) 초록 원으로 임시 표시선 그리기
	private void OnDrawGizmos()
	{
		if (pointA == null || pointB == null) return;
		Gizmos.color = Color.green;
		for (int i = 0; i < segmentsCount; i++)
		{
			Vector2 posAtIndex = GetSegmentPosition(i);
			Gizmos.DrawSphere(posAtIndex, 0.1f);
		}
	}
}
