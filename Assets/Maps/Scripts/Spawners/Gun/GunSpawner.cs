using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 총이 생성될 위치에 있는 GameObject의 component로 동작합니다. 해당 GameObject는 Dungen에 의해 일정 수량 생성됩니다.
/// </summary>
public class GunSpawner : MonoBehaviour
{
    /// <summary>
    /// 총기 프롭을 생성합니다. FindObjectsByType<GunSpawner>() -> Initialize(stageIndex(int)) 로 사용.
    /// </summary> 
    /// <param name="stageIndex"></param>
    public void InitializeGunProp(int stageIndex)
    {
        Debug.Log("initialize Gun");

        GameObject prefab = SpawnedGunBuilder.GetRandomGunPrefab(stageIndex);
        if (prefab != null)
        {
            Instantiate(prefab, transform);
        }
    }


}
