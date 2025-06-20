using Akila.FPSFramework;
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

    private Image[] _inventorySlots;

    public float originalScale;
    public float upscale;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _playerInventory = FindAnyObjectByType<Inventory>().gameObject;

        // Image 배열 초기화
        _inventorySlots = new Image[] { inventory1, inventory2, inventory3 };

        InventoryCheck();
    }

    private void Update()
    {
        InventoryCheck();
    }

    public void InventoryCheck()
    {
        // 1. 자식 GameObject 리스트 수집
        List<GameObject> pocket = new List<GameObject>();
        for (int i = 0; i < _playerInventory.transform.childCount; i++)
        {
            pocket.Add(_playerInventory.transform.GetChild(i).gameObject);
        }

        // 2. 각 슬롯마다 UI 갱신
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            Image gunBackground = _inventorySlots[i].transform.GetChild(1).GetComponent<Image>();
            Image gunImage = _inventorySlots[i].transform.GetChild(2).GetComponent<Image>();
            
            if (i < pocket.Count && pocket[i] != null)
            {
                Firearm firearm = pocket[i].GetComponent<Firearm>();
                gunImage.enabled = true;
                gunBackground.enabled = true;
                gunBackground.color = firearm.GetGradeColor();
                gunImage.sprite = firearm != null ? firearm.gunImage : null;
            }
            else
            {
                gunImage.enabled = false;
                gunBackground.enabled = false;
            }
        }

        // 3. 첫 번째 활성 무기 색상 강조
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (i < pocket.Count && pocket[i] != null)
            {
                if (pocket[i].activeSelf)
                {
                    _inventorySlots[i].transform.localScale = new Vector3(upscale, upscale, upscale); // 활성화된 무기 강조
                }
                else
                {
                    _inventorySlots[i].transform.localScale = new Vector3(originalScale, originalScale, originalScale); // 비활성화된 무기
                }
            }
            else
            {
                // pocket[i]가 없을 경우에도 스케일을 초기화
                _inventorySlots[i].transform.localScale = new Vector3(originalScale, originalScale, originalScale);
            }
        }

    }
}

