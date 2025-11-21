using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Weapon/Weapons/RotateWeapon")]
public class RotateWeapon : Weapon
{
    public GameObject weaponPrefab; // 웨폰 프리팹
    public float weaponDamage; // 무기 데미지
    public int count = 3; // 웨폰갯수
    [SerializeField] private float _angle = 0f; // angle 0f
    public float radius = 1.5f;   // 플레이어로부터 거리
    public float rotateSpeed = 180f; // 초당 회전 각도


    public override IEnumerator WeaponController(GameObject player)
    {
        while (true)
        {
            _angle += rotateSpeed * Time.deltaTime;

            float angleOffset = 360f / count;

            for (int i = 0; i < weaponList.Count; i++)
            {
                float currentAngle = _angle + angleOffset * i;
                float rad = currentAngle * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;

                weaponList[i].transform.position = player.transform.position + offset;

                // 무기가 바라보는 방향도 회전시키고 싶으면 ↓
                weaponList[i].transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            }

            yield return null;
        }
    }

    List<GameObject> weaponList = new List<GameObject>();
    public override void InitWeapon(GameObject player)
    {
        // 1. 기존 무기 정리
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i] != null)
            {
                Destroy(weaponList[i]);   // ScriptableObject라 this.Destroy 말고 Object.Destroy
            }
        }
        weaponList.Clear();

        // 2. 새로 생성
        _angle = 0f;
        for (int i = 0; i < count; i++)
        {
            var wp = Object.Instantiate(weaponPrefab, player.transform.position, Quaternion.identity);
            wp.transform.SetParent(player.transform);
            wp.GetComponent<RotateWeaponObj>().IninRotateWeaponObj(weaponDamage);
            weaponList.Add(wp);
        }
    }
}
