using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillRuntimeView : MonoBehaviour
{
    [Header("Utile")]
    public SkillSlot D_skillSlot;   // D 슬롯
    [SerializeField] private Slider _D_slider;         // D 슬롯의 슬라이더 (참조 연결)


    [Header("Active")]
    public SkillSlot Q_skillSlot;   // D 슬롯
    [SerializeField] private Slider _Q_slider;         // D 슬롯의 슬라이더 (참조 연결)
    public SkillSlot W_skillSlot;   // D 슬롯
    [SerializeField] private Slider _W_slider;         // D 슬롯의 슬라이더 (참조 연결)
    public SkillSlot E_skillSlot;   // D 슬롯
    [SerializeField] private Slider _E_slider;         // D 슬롯의 슬라이더 (참조 연결)
    public SkillSlot R_skillSlot;   // D 슬롯
    [SerializeField] private Slider _R_slider;         // D 슬롯의 슬라이더 (참조 연결)



    void Start()
    {
        _D_slider.minValue = 0f;
        _D_slider.maxValue = 1f;

        _Q_slider.minValue = 0f;
        _Q_slider.maxValue = 1f;

        _W_slider.minValue = 0f;
        _W_slider.maxValue = 1f;

        _E_slider.minValue = 0f;
        _E_slider.maxValue = 1f;

        _R_slider.minValue = 0f;
        _R_slider.maxValue = 1f;
    }

    void Update()
    {
        if (D_skillSlot.skillInstance.skill == null) return;
        // 채워졌다가 줄어드는 느낌이면 그대로:
        _D_slider.value = D_skillSlot.skillInstance.Normalized; // 1 → 0

    }
}

