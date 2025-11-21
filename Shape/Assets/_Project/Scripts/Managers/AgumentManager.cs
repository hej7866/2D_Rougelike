using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UIElements;


public class AgumentManager : SingleTon<AgumentManager>
{
    private PlayerManager _pm;
    private UIManager _um;
    private AgumentDatabase _ad;
    HashSet<int> hash = new HashSet<int>();
 
    public GameObject[] agumentBtns;

    void Start()
    {
        _pm = PlayerManager.Instance;
        _um = UIManager.Instance;
        _ad = GetComponent<AgumentDatabase>();
    }

    public void SetAgument(LevelSystem levelSystem)
    {
        hash.Clear();

        // 뽑을 수 있는 인덱스 목록 만들기
        List<int> candidate = new List<int>();
        float critProb = PlayerManager.Instance.playerStat.Stat[StatType.CriticalProb];

        for (int i = 0; i < _ad.aguments.Length; i++)
        {
            // 5번이 크리티컬 증가 증강
            if (i == 5 && critProb >= 100f)
                continue; // 후보에서 제외

            candidate.Add(i);
        }

        // 뽑아야 할 개수 = 버튼 개수 vs 후보 개수 중 작은 값
        int need = Mathf.Min(agumentBtns.Length, candidate.Count);

        // 후보 목록에서 랜덤으로 need개 뽑기 (중복 없이)
        for (int n = 0; n < need; n++)
        {
            int idx = Random.Range(0, candidate.Count);
            int pick = candidate[idx];
            candidate.RemoveAt(idx);   // 중복 방지

            hash.Add(pick);
        }

        // UI에 적용
        List<int> list = hash.ToList();
        for (int i = 0; i < need; i++)
        {
            AgumentData data = agumentBtns[i].GetComponent<AgumentData>();
            data.agument = _ad.aguments[list[i]];
        }
    }

    public void SelectAgument()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
        AgumentData data = null; 
        if (clickedObj != null)
        {
            data = clickedObj.GetComponent<AgumentData>();
            Debug.Log(data.agument.agumentName);
        }
        _pm.statCalculator.CalculateOnSelecetAgument(data);
        _um.SwitchUI(_um.agumentView.augument_Panel);
        hash.Clear();
        GameManager.Instance.SwitchGame();
    }
}
