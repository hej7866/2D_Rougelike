using UnityEngine;

[CreateAssetMenu(menuName="Player/character")]
public class Character : ScriptableObject
{
    public Sprite sprite;
    public float hp;
    public float mp;
    public float speed;
    public float exp;
    public int level;
}
