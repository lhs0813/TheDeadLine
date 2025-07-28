using System.Collections;
using UnityEngine;

public class PreGunSpawner : MonoBehaviour
{
    [SerializeField] WeaponGrade weaponGrade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitForGunBuilderAndSpawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator WaitForGunBuilderAndSpawn()
    {
        yield return new WaitUntil(() => SpawnedGunBuilder.isInitialized);
        InitializeGunProp(); 
    }
    
    public void InitializeGunProp()
    {
        GameObject prefab = SpawnedGunBuilder.GetRandomGunPrefabByGrade(weaponGrade);
        if (prefab != null)
        {
            Instantiate(prefab, transform);
        }
    }

}
