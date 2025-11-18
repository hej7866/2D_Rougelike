using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemys/enemy")]
public class Enemy : ScriptableObject
{
    public Sprite sprite;
    public float hp;
    public float speed;
    public float attackDamage;
    public float xpValue;
}
