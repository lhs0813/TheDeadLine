using Akila.FPSFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Update : MonoBehaviour
{
    public static UI_Inventory_Update Instance { get; private set; }

    public Image inventory1;
    public Image inventory2;
    public Image inventory3;

    [SerializeField] private GameObject _playerInventory;



    private void Start()
    {
        _playerInventory = FindAnyObjectByType<Inventory>().gameObject;
        InventoryCheck();
    }

    private void Update()
    {

        InventoryCheck();
    }

    public void InventoryCheck()
    {
        // 1. 자식 게임오브젝트 수집
        List<GameObject> pocket = new List<GameObject>();
        for (int i = 0; i < _playerInventory.transform.childCount; i++)
        {
            GameObject child = _playerInventory.transform.GetChild(i).gameObject;
            pocket.Add(child);
        }

        // 2. 전부 색 초기화 (예: 흰색)
        inventory1.color = Color.black;
        inventory2.color = Color.black;
        inventory3.color = Color.black;

        // 3. 활성화된 첫 번째 오브젝트를 찾고 그 인덱스를 기반으로 색 변경
        for (int i = 0; i < pocket.Count; i++)
        {
            if (pocket[i].activeSelf)
            {
                switch (i)
                {
                    case 0:
                        inventory1.color = Color.red;
                        break;
                    case 1:
                        inventory2.color = Color.red;
                        break;
                    case 2:
                        inventory3.color = Color.red;
                        break;
                }
                break; // 첫 번째 활성화된 오브젝트만 체크
            }
        }
    }


}
