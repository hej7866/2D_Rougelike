using UnityEngine;

public class Exp : MonoBehaviour
{
    public int value; // 경험치 양
    public float speed = 5f;
    public bool isDrainArea = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"경험치 {value}만큼 획득");
            PlayerManager.Instance.levelSystem.AddExp(value);
            ExpManager.Instance.ReturnExp(gameObject);
        }
    }
    
    void Update()
    {
        if (!isDrainArea) return;

        Vector3 dir = (PlayerManager.Instance.player.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}
