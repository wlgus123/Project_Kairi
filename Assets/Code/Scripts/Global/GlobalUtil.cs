using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 전역 유틸리티
/// 자주 사용되는 기능들을 클래스로 모아놓은 파일
/// </summary>
public class GlobalUtil
{
	public static void CheckGround(Transform transform, Collision2D collision, Rigidbody2D rigid)
	{
		bool isGrounded = false;

		foreach (var contact in collision.contacts)     // 바닥 체크
		{
			if (contact.normal.y > 0.7f &&
				contact.point.y < transform.position.y)
			{
				isGrounded = true;
				break;
			}
		}

		if (isGrounded && rigid.linearVelocityY < 0f)       // y값 보정 (바닥 뚫림 방지)
			rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
	}

	// 카메라 월드 좌표 구하기
	//public static Vector2 GetCameraWorldPos()
	//{
	//	Vector3 mouseScreen = Mouse.current.position.ReadValue();       // 스크린 좌표 구하기
	//	mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);    // z값 보정
	//	Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseScreen); // 월드 좌표
			
	//	return worldPos;
	//}
}
