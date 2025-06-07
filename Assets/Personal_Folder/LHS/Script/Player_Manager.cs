using UnityEngine;
using System;
using Akila.FPSFramework;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager Instance { get; private set; }

    public static Action<float> PlayerHpChange;

    public float playerHp;
    public float MaxHealth;

    private void Awake()
    {
        PlayerHpChange = null;
    }

    void Start()
    {



        playerHp = 100;
        MaxHealth = playerHp;

        PlayerHpChange.Invoke(100);

        PlayerHpChange += hpUpdate;

        
    }

    private void hpUpdate(float hp)
    {
        playerHp = hp;
    }

}

