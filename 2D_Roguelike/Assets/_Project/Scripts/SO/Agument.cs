using UnityEngine;


public enum OperationType { add, mul }

[CreateAssetMenu(fileName = "Agument", menuName = "Aguments/Agument")]
public class Agument : ScriptableObject
{
    public string agumentName;
    public string agumentDesc;
    public StatType statType;
    public OperationType operationType;
    public float value;
}
