using UnityEngine;
using UnityEngine.Animations;

public class HitText : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        PoolManager.Instance.hitTextPools.ReturnHitText(transform.parent.gameObject);
    }
}
