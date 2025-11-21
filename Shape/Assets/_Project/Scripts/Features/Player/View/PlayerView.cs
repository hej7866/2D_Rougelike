using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class PlayerView : MonoBehaviour
{
    [Header("Status Slider")]
    public Slider hpBar;
    public Slider mpBar;
    public Slider expBar;

    [Header("레벨 관련")]
    public Text level_Text;

    [Header("스탯 관련")]
    public GameObject stat_panel;
    public GameObject statText_panel;
    [SerializeField] private List<Text> statTexts = new List<Text>();

    [Header("크로스 헤어")]
    [SerializeField] private GameObject aim;


    void Update()
    {
        OnAim();
    }

    public void UpdateUIOnChangePlayerVital<T>(T t)
    {
        switch (t)
        {
            case LevelSystem levelSystem:
                expBar.value = levelSystem.Exp / levelSystem.RequiredExp;
                break;
            case PlayerController playerController:
                hpBar.value = playerController.Hp / playerController.pm.playerStat.stat[StatType.MaxHp];
                mpBar.value = playerController.Mp / playerController.pm.playerStat.stat[StatType.MaxMp];
                break;
            default:
                break;
        }
    }

    public void UpdateUIOnChangePlayerStat(Dictionary<StatType, float> stat)
    {
        if (statTexts.Count != stat.Count)
        {
            for (int i = 0; i < stat.Count; i++)
            {
                statTexts.Add(statText_panel.transform.GetChild(i).GetComponent<Text>());
            }
            PlayerManager pm = PlayerManager.Instance;
            for (int i = 0; i < pm.playerStat.StatList.Count; i++)
            {
                float displayedValue = Mathf.Round(pm.playerStat.StatList[i].value * 10) / 10f;
                statTexts[i].text = $"[{pm.playerStat.StatList[i].type}] : {displayedValue}";
            }
        }
        else
        {
            PlayerManager pm = PlayerManager.Instance;
            for (int i = 0; i < pm.playerStat.StatList.Count; i++)
            {
                float displayedValue = Mathf.Round(pm.playerStat.StatList[i].value * 10) / 10f;
                statTexts[i].text = $"[{pm.playerStat.StatList[i].type}] : {displayedValue}";
            }
        }
    }

    void OnToggleStat()
    {
        bool isActive = !stat_panel.activeSelf;
        stat_panel.SetActive(isActive);
    }

    public void UpdateUIOnLevelUp(LevelSystem levelSystem)
    {
        level_Text.text = $"LV : {levelSystem.Level}";
        UIManager.Instance.SwitchUI(UIManager.Instance.agumentView.augument_Panel);
        GameManager.Instance.SwitchGame();
    }

    public void OnAim()
    {
        Vector3 aimPos = PlayerManager.Instance.aim.GetAimPos();
        aimPos.z = 0;

        aim.transform.position = aimPos;
    }
}
