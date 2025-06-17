using UnityEngine;
using System;
using Akila.FPSFramework;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager Instance { get; private set; }

    public static Action<float> PlayerHpChange;
    public static Action<float> PlayerMaxHpChange;

    public float playerHp;
    public float MaxHealth;

    private void Awake()
    {
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



}

