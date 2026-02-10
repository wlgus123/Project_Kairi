using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Global Light 오브젝트")]
    public Light2D globalLight;
    public Slider brightSlider;
    private float bright;
    [Header("사운드 조절")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Update()
    {
        if (globalLight == null || brightSlider == null) return;
            globalLight.intensity = brightSlider.value;
    }
}