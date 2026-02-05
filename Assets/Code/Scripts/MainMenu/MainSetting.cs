using UnityEngine;
using UnityEngine.InputSystem;

public class MainSetting : MonoBehaviour    // MainMenu -> OptionManager
{
    public GameObject settingPanel;
    public GameObject quitPanel;

    private bool isSetting;
    private bool isQuit;

    void Awake()
    {
        CloseAll();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (isQuit)
            CloseQuit();
        else if (isSetting)
            CloseSetting();
    }

    public void OpenSetting()
    {
        if (settingPanel == null) return;

        isSetting = true;
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        if (settingPanel == null) return;

        isSetting = false;
        settingPanel.SetActive(false);
    }

    public void OpenQuit()
    {
        if (quitPanel == null) return;

        isQuit = true;

        quitPanel.SetActive(true);
    }

    public void CloseQuit()
    {
        if (quitPanel == null) return;

        isQuit = false;
        quitPanel.SetActive(false);
    }

    void CloseAll()
    {
        isSetting = isQuit = false;

        if (settingPanel != null) settingPanel.SetActive(false);
        if (quitPanel != null) quitPanel.SetActive(false);
    }
}
