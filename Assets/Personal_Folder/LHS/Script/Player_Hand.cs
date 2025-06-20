using Akila.FPSFramework;
using UnityEngine;

public class Player_Hand : MonoBehaviour
{
    public static Player_Hand Instance { get; private set; }
    public Animator _playerHandAnim;
    ParticleSystem _spark;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _playerHandAnim = GetComponent<Animator>();
        
    }

    
    public void Charge()
    {
        _playerHandAnim.SetTrigger("Charge");

    }



}
