using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using sceneName = Globals.SceneName;

public class BtnType : MonoBehaviour
{
    public EnumType.BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;
    bool isProcessing;      // ��ư ��Ÿ ����

    MainSetting cachedMainSetting;

    void Start()
    {
        if (buttonScale != null)
            defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick()    // OnCLick�� ����
    {
        if (isProcessing) return;
        StartCoroutine(OnBtnClickRoutine());
    }

    IEnumerator OnBtnClickRoutine()
    {
        isProcessing = true;

        switch (currentType)
        {
            case EnumType.BTNType.MainStart:
                SceneManager.LoadScene(sceneName.stage01);
                break;

            case EnumType.BTNType.MainSetting:
                GetMainSetting()?.OpenSetting();
                break;

            case EnumType.BTNType.MainQuit:
                GetMainSetting()?.OpenQuit();
                break;

            case EnumType.BTNType.MainQuitNo:
                GetMainSetting()?.CloseQuit();
                break;

            case EnumType.BTNType.Setting:
                GetESCKey()?.openSetting();
                break;

            case EnumType.BTNType.GameLeave:
                GetESCKey()?.openLeave();
                break;

            case EnumType.BTNType.GameQuit:
                GetESCKey()?.openQuit();
                break;

            case EnumType.BTNType.LeaveYes:
                SceneManager.LoadScene(sceneName.mainMenu);
                break;

            case EnumType.BTNType.LeaveNo:
                GetESCKey()?.CloseLeave();
                break;

            case EnumType.BTNType.QuitYes:
                QuitGame();
                break;

            case EnumType.BTNType.QuitNo:
                GetESCKey()?.CloseQuit();
                break;
        }

        isProcessing = false;
        yield break;
    }

    MainSetting GetMainSetting()
    {
        if (cachedMainSetting == null)
            cachedMainSetting = Object.FindFirstObjectByType<MainSetting>();

        if (cachedMainSetting == null)
            Debug.LogWarning("���� MainSetting ������Ʈ�� �����ϴ�.");

        return cachedMainSetting;
    }

    ESCKey GetESCKey()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance ����");
            return null;
        }

        if (GameManager.Instance.escKey == null)
            GameManager.Instance.escKey = Object.FindFirstObjectByType<ESCKey>();

        if (GameManager.Instance.escKey == null)
            Debug.LogWarning("���� ESCKey ������Ʈ�� �����ϴ�.");

        return GameManager.Instance.escKey;
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("������ �÷��� ����");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // CanvasGroup
    public void CanvasGroupOn(CanvasGroup cg)
    {
        if (cg == null) return;
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg)
    {
        if (cg == null) return;
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
