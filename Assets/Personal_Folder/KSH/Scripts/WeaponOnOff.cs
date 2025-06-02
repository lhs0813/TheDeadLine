
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//
public class WeaponOnOff : MonoBehaviour
{
    [Header("Skill")]

   public  bool isUsing;
    [Space(30)]


    [Header("Effect")]
    public ParticleSystem effect;
    List<ParticleSystem> _effects; 
    List<LineReder> _lines;
    [Space(30)]


    [Header("Sound")]
    public AudioSource soundStart;
    public AudioSource soundLoop;
    public AudioSource soundEnd;


    Akila.FPSFramework.Firearm firearm;



    void Start()
    {
        _effects = GetComponentsInChildren<ParticleSystem>().ToList();
        _lines = GetComponentsInChildren<LineReder>().ToList();
        firearm = GetComponentInParent<Akila.FPSFramework.Firearm>();

        for (int i = 0; i < _effects.Count; i++)
            _effects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
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

        for (int i = 0; i < _lines.Count; i++)
            _lines[i].enabled = true;

        soundStart.Play(); 
        soundLoop.Play();
    }
    public void SkillStop()
    {
        for (int i = 0; i < _effects.Count; i++)        
            _effects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);

        for (int i = 0; i < _lines.Count; i++)
            _lines[i].enabled = false;

        soundEnd.Play();
        soundLoop.Stop();
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

