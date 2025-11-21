using UnityEngine;

public class Aim : MonoBehaviour
{
    [Header("바라보는 방향")]
    public Transform dir;   // 화살표 오브젝트 (자식)
    public float radius = 1.5f; // 원 반지름

    void FixedUpdate()
    {
        Look();
    }

    public void Look()
    {
        // 마우스의 월드 좌표 구하기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 원 중심(this.transform) → 마우스 방향 벡터
        Vector3 dirVec = (mousePos - transform.position).normalized;

        // Dir 오브젝트를 원 둘레에 위치시키기
        dir.position = transform.position + dirVec * radius;

        // Dir 오브젝트가 마우스를 바라보게 회전시키기
        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
        dir.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public Vector3 GetAimPos()
    {
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return aimPos;
    }
}
