using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("대화 텍스트")]
    public TextMeshProUGUI talkText;

    [Header("플레이어가 상호작용하는 오브젝트")]
    public GameObject scanObject;

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        talkText.text = "Name: " + scanObj.name + ".";
    }
}
