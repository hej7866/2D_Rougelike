using Unity.VisualScripting;
using UnityEngine;

public class Drain : MonoBehaviour
{
    [SerializeField] CircleCollider2D cc;

    public void ChangeCircleSize(Stat stat)
    {
        cc.radius = stat.DrainArea;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(cc.IsTouching(collision))
        {
            if(collision.CompareTag("Exp"))
            {
                Exp exp = collision.GetComponent<Exp>();
                exp.isDrainArea = true;
            }
        }
    }
}
