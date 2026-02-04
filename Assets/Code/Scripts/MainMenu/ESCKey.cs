using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class ESCKey : MonoBehaviour		// GameManager 오브젝트
{
	public GameObject optionCanvas; // UI 캔버스
	public GameObject settingCanvas; // UI 캔버스
	private bool isOption; // 현재 일시정지 상태
	private bool isSetting; // 현재 일시정지 상태

	void Awake()
	{
		isOption = false;
		isSetting = false;
		if (optionCanvas != null)
			optionCanvas.SetActive(false); // 게임 시작 시 비활성화
		 Time.timeScale = 1f; // 게임 시작 시 확실하게 1로 설정
	}
    void Update()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (isSetting)  // 세팅창 닫기
        {
            isSetting = false;
            settingCanvas.SetActive(false);

            isOption = true;
            optionCanvas.SetActive(true);
        }
        else
            openOption();   // 옵션창 토글

        if (isSetting)
            Time.timeScale = 0f;
        else if (isOption)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    // 옵션창 띄우기 함수
    public void openOption()
    {
        if (optionCanvas == null) return;

        isOption = !isOption;
        isSetting = false;

        optionCanvas.SetActive(isOption);
        if (settingCanvas != null)
            settingCanvas.SetActive(false);
    }

    public void openSetting()
    {
        if (settingCanvas == null) return;

        isSetting = true;
        isOption = false;

        settingCanvas.SetActive(true);
        if (optionCanvas != null)
            optionCanvas.SetActive(false);
    }
}
