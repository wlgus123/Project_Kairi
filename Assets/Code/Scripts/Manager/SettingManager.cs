using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Global Light 오브젝트")]
    public Light2D globalLight;
    public Slider brightSlider;
    private float bright;

    private void Update()
    {
        if (globalLight == null || brightSlider == null) return;
            globalLight.intensity = brightSlider.value;
    }
}