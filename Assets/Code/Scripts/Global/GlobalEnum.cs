using UnityEngine;

/// <summary>
/// �������� ������(Enum) ��Ƴ��� ����
/// </summary>
/// 
namespace EnumType
{
	// �޴� ���� ��ư Ÿ��
	public enum BTNType
	{
		// MainMenu��
		MainStart,
		MainSetting,
		MainQuit,
		MainQuitNo,
		// Game��
		Check,
        Setting,
        GameLeave,
        GameQuit,
		LeaveYes,
		LeaveNo,
		QuitYes,
		QuitNo
	}

	// �� ����
	public enum EnemyState
	{
		Idle = 0,	// �⺻
		Thrown,		// ������
	}

	// ������Ʈ ����
	public enum ObjState
	{
		Idle = 0,	// �⺻
		Thrown,		// ������
	}

	// �÷��̾� ����
	public enum PlayerState
	{
		Idle = 0,		// �⺻ (���)
		Run,			// �޸���
		Jump,           // ����
		Damaged,        // ������
		Grappling,		// ���̾� ������
		Hanging,		// ���̾� �Ŵ޸���
		SpeedUp,		// ���ӵ� ���
		PickUp,			// �� �� ������Ʈ ����
		Throw,			// �� �� ������Ʈ ������
		PickAndHook,	// �� �� ������Ʈ ���� ���¿��� ���̾� ������
	}
}