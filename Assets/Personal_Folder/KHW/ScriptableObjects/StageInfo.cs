using UnityEngine;

[CreateAssetMenu(fileName = "StageInfo", menuName = "Scriptable Objects/StageInfo")]
public class StageInfo : ScriptableObject
{
    public int stageIndex;
    public int fuseCount;
    public float combatTime;
}
