using UnityEngine;

public class RotateWeaponObj : MonoBehaviour
{
    private float _damage;

    public void IninRotateWeaponObj(float damage)
    {
        _damage = damage;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().TakeDamage(_damage);
            return;
        }

        if(collision.CompareTag("Boss"))
        {
            collision.GetComponent<BossController>().TakeDamage(_damage);
            return;
        }
    }
}
