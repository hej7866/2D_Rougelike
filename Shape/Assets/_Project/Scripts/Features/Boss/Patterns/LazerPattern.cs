using UnityEngine;
using System.Collections;
using System;


[CreateAssetMenu(menuName = "Boss/Patterns/Lazer")]
public class LazerPattern : BossPattern
{
    [Serializable]
    public class LazerParams : PatternParams
    {
        public GameObject lazerPrefab;
        public float damage;
        public float speed;
        public Vector3[] vectors = { Vector3.right * 10,  Vector3.left * 10, Vector3.up * 10, Vector3.down * 10};
    }

    public override Type ParamsType => typeof(LazerParams);

    public override PatternParams CreatePatternParams() => new LazerParams();

    public override IEnumerator ExecutePattern(BossController boss, PatternParams p)
    {
        var prm = (LazerParams)p;
        var bossRoom = GameObject.FindWithTag("BossRoom");

        int ran = UnityEngine.Random.Range(0, prm.vectors.Length);

        Vector3 pos = bossRoom.transform.position + prm.vectors[ran];

        // 기본 회전값
        Quaternion rot = Quaternion.identity;

        // ran이 2 또는 3일 경우 Z += 90
        if (ran == 2 || ran == 3)
            rot = Quaternion.Euler(0f, 0f, 90f);

        var lazer = Instantiate(prm.lazerPrefab, pos, rot);
        lazer.GetComponent<BossLazer>().InitBossLazer(prm.damage);
        float duration = 20f / prm.speed;
        float t = 0;
        while(t <= duration)
        {
            t += Time.deltaTime;
            switch(ran)
            {
                case 0:
                    lazer.transform.position += new Vector3 (-1,0,0) * prm.speed * Time.deltaTime;
                    break;
                case 1:
                    lazer.transform.position += new Vector3 (1,0,0) * prm.speed * Time.deltaTime;
                    break;        
                case 2:
                    lazer.transform.position += new Vector3 (0,-1,0) * prm.speed * Time.deltaTime;
                    break;
                case 3:
                    lazer.transform.position += new Vector3 (0,1,0) * prm.speed * Time.deltaTime;
                    break;

            }
            yield return null;
        }
        Destroy(lazer);
    }

}
