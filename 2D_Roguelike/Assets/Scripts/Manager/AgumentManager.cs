using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AgumentManager : SingleTon<AgumentManager>
{
    private PlayerManager pm;
    private UIManager um;
    private AgumentDatabase ad;
    HashSet<int> hash = new HashSet<int>();
 
    public GameObject[] agumentBtns;

    void Start()
    {
        pm = PlayerManager.Instance;
        um = UIManager.Instance;
        ad = GetComponent<AgumentDatabase>();
    }

    public void SetAgument(LevelSystem levelSystem)
    {
        while (hash.Count < ad.aguments.Length)
        {
            int ran = Random.Range(0, ad.aguments.Length);
            hash.Add(ran);
        }

        List<int> list = hash.ToList();  // using System.Lin
        for(int i=0; i<agumentBtns.Length; i++)
        {
            AgumentData data = agumentBtns[i].GetComponent<AgumentData>();
            data.agument = ad.aguments[list[i]];
        }
    }

    public void SelectAgument()
    {
        Debug.Log("증강을 선택하였습니다. 스탯을 재계산합니다.");
        pm.statCalculator.CalculateOnSelecetAgument();
        um.SwitchUI(um.augument_Panel);
    }
}
