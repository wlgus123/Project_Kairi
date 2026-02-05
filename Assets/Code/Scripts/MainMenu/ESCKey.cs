using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class ESCKey : MonoBehaviour		// GameManager ������Ʈ
{
    public GameObject optionPanel;
    public GameObject settingPanel;
    public GameObject leavePanel;
    public GameObject quitPanel;

    private bool isOption;
    private bool isSetting;
    private bool isLeave;
    private bool isQuit;

    void Awake()
    {
        CloseAll();
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        // �켱����: Quit > Leave > Setting > Option
        if (isQuit)
            CloseQuit();
        else if (isLeave)
            CloseLeave();
        else if (isSetting)
            CloseSetting();
        else
            ToggleOption();

        UpdateTimeScale();
    }

    // ================= Option =================
    public void ToggleOption()
    {
        if (optionPanel == null) return;

        isOption = !isOption;
        optionPanel.SetActive(isOption);
    }

    // ================= Setting =================
    public void openSetting()
    {
        if (settingPanel == null || optionPanel == null) return;

        isSetting = true;
        isOption = false;

        settingPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void CloseSetting()
    {
        if (settingPanel == null || optionPanel == null) return;

        isSetting = false;
        settingPanel.SetActive(false);

        isOption = true;
        optionPanel.SetActive(true);
    }

    // ================= Leave =================
    public void openLeave()
    {
        if (leavePanel == null || optionPanel == null) return;

        isLeave = true;
        isOption = false;

        leavePanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void CloseLeave()
    {
        if (leavePanel == null || optionPanel == null) return;

        isLeave = false;
        leavePanel.SetActive(false);

        isOption = true;
        optionPanel.SetActive(true);
    }

    // ================= Quit =================
    public void openQuit()
    {
        if (quitPanel == null || optionPanel == null) return;

        isQuit = true;
        isOption = false;

        quitPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void CloseQuit()
    {
        if (quitPanel == null || optionPanel == null) return;

        isQuit = false;
        quitPanel.SetActive(false);

        isOption = true;
        optionPanel.SetActive(true);
    }

    // ================= Common =================
    void CloseAll()
    {
        isOption = isSetting = isLeave = isQuit = false;

        if (optionPanel != null) optionPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
        if (leavePanel != null) leavePanel.SetActive(false);
        if (quitPanel != null) quitPanel.SetActive(false);
    }

    void UpdateTimeScale()
    {
        Time.timeScale = (isOption || isSetting || isLeave || isQuit) ? 0f : 1f;
    }
}
