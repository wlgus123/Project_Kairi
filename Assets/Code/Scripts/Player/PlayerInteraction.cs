using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    // 대화창 매니저
    [Header("대화 시스템 매니저")]
    public DialogManager dialogMng;

    // 액션 여부
    public bool GetIsAction()
    {
        if(dialogMng)
            return dialogMng.isAction;

        return false;
    }

    // E키 클릭 시 상호작용 발생 (대화창)
    void OnInteract(InputValue value)
    {
        dialogMng.Action(this.gameObject);
    }
}
