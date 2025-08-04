
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


//
public class WeaponOnOff : MonoBehaviour
{
    [Header("Skill")]
    public bool isUsing;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
    [Space(30)]


    [Header("Effect")]
    public ParticleSystem effect;
    List<ParticleSystem> _effects; 
    [Space(30)]


    [Header("Sound")]
    public AudioSource soundStart;
    public AudioSource soundLoop;
    public AudioSource soundEnd;


    Akila.FPSFramework.Firearm firearm;



    void Start()
    {
        _effects = GetComponentsInChildren<ParticleSystem>().ToList();
        firearm = GetComponentInParent<Akila.FPSFramework.Firearm>();

        for (int i = 0; i < _effects.Count; i++)
            _effects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);


        firearm.onFire += FireSound;
    }

    private void OnEnable()
    {
        if (isUsing)
        {
            isUsing = false;
            SkillStop();
        }
    }

    void Update()
    {
        if (firearm.isReloading)
        {
            if (isUsing)
            {
                isUsing = false;
                SkillStop();
            }
            return;
        }

        if (firearm.remainingAmmoCount == 0)
        {
            if (isUsing)
            {
                isUsing = false;
                SkillStop();
            }
            return;
        }
        if (firearm.IsPlayingRestrictedAnimation())
        {
            if (isUsing)
            {
                isUsing = false;
                SkillStop();
            }
            return;
        }





        // Fire turret
        if (firearm.itemInput.Controls.Firearm.Fire.IsPressed() == true)
        {
            if (!isUsing)
            {
                isUsing = true;
                SkillStart();
            }
        }

        // Stop firing
        if (firearm.itemInput.Controls.Firearm.Fire.IsPressed() == false)
        {
            if (isUsing)
            {
                isUsing = false;
                SkillStop();
            }
        }
    }
    public void SkillStart() 
    {
        for (int i = 0; i < _effects.Count; i++)         
            _effects[i].Play(true);

       if(soundStart)soundStart.Play(); 
        if(soundLoop)soundLoop.Play();
        OnStart.Invoke();
    }
    public void SkillStop()
    {
        for (int i = 0; i < _effects.Count; i++)        
            _effects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);


        if (soundEnd) soundEnd.Play();
        if (soundLoop) soundLoop.Stop();
        OnEnd.Invoke();
    }   

    void FireSound(Vector3 a,Quaternion b,Vector3 z)
    {
        if (soundStart) soundStart.Play();
    }
}

/*
 * ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
ParticleSystem.Play(true);


        //충전 부족함 
        if (_charging < chargeMax)
            return ;

 
        //================== 충전중 ==================
        if (_charging < chargeMax)
        {
            _charging += Time.deltaTime;


            //과충전
            if (_charging >= chargeMax)
                _charging = chargeMax;
        }
 
 

    [Header("Charge")]
    public float chargeMax;
    float _charging;
    [Space(30)]
*/

