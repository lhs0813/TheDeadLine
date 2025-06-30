using UnityEngine;
using System;
using Akila.FPSFramework;
using System.Collections;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager Instance { get; private set; }

    public FirstPersonController playerController;
    public Inventory inventory;

    public static Action<float> PlayerHpChange;
    public static Action<float> PlayerMaxHpChange;

    public float playerHp;
    public float MaxHealth;

    private void Awake()
    {
        Instance = this;

        PlayerHpChange = null;
        PlayerMaxHpChange = null;
    }

    void Start()
    {
        PlayerHpChange += hpUpdate;
        PlayerMaxHpChange += MaxhpUpdate;
    }
    
    private void hpUpdate(float hp)
    {
        playerHp = hp;
    }

    private void MaxhpUpdate(float maxHp)
    {
        MaxHealth = maxHp;
    }

    public IEnumerator SpeedReturn()
    {
        Vector3 localPos = inventory.transform.localPosition;
        localPos.z = -0.3f;
        inventory.transform.localPosition = localPos;

        yield return new WaitForSeconds(1.0f);
        playerController.walkSpeed = 5;
        playerController.sprintSpeed = 10;

        localPos.z = -0.5f;
        inventory.transform.localPosition = localPos;
    }



}

