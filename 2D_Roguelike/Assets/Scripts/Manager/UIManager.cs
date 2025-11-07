using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : SingleTon<UIManager>
{
    [Header("Status Slider")]
    public Slider hpBar;
    public Slider mpBar;
    public Slider expBar;

    [Header("레벨 관련")]
    public Text level_Text;

    [Header("증강 관련")]
    public GameObject augument_Panel;

    public void UpdateUIOnChangePlayerConditon<T>(T t)
    {
        switch (t)
        {
            case LevelSystem levelSystem:
                expBar.value = levelSystem.Exp / levelSystem.RequiredExp;
                break;
            case PlayerController playerController:
                hpBar.value = playerController.Hp / playerController.pm.playerStats.Stat.MaxHp;
                mpBar.value = playerController.Mp / playerController.pm.playerStats.Stat.MaxMp;
                break;
            default:
                break;
        }
    }

    public void UpdateUIOnLevelUp(LevelSystem levelSystem)
    {
        level_Text.text = $"LV : {levelSystem.Level}";
        UpdateAgumentBtnUI();
        SwitchUI(augument_Panel);
    }

    void UpdateAgumentBtnUI()
    {
        GameObject[] agumentBtns = AgumentManager.Instance.agumentBtns;
        for(int i=0; i<agumentBtns.Length; i++)
        {
            AgumentData data = agumentBtns[i].GetComponent<AgumentData>();
            Text agumentName = agumentBtns[i].transform.GetChild(0).GetComponent<Text>();
            Text agumentDesc = agumentBtns[i].transform.GetChild(1).GetComponent<Text>();
            Image agumentImg = agumentBtns[i].transform.GetChild(2).GetComponent<Image>();

            agumentName.text = data.agument.agumentName;
            agumentDesc.text = data.agument.agumentDesc;
        }
    }

    public void SwitchUI(GameObject ui)
    {
        bool b = ui.activeSelf ? false : true;
        ui.SetActive(b); 
    }
}
