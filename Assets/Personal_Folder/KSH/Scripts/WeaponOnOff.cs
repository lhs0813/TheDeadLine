
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//
public class WeaponOnOff : MonoBehaviour
{
    [Header("Skill")]
    public GameObject next;
    public GameObject startPosition;
    bool _isUsing;
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




    void Start()
    {
        startPosition = GetComponentInChildren<TagWeaponStart>().gameObject;
        _effects = GetComponentsInChildren<ParticleSystem>().ToList();
        _lines = GetComponentsInChildren<LineReder>().ToList();


        for (int i = 0; i < _effects.Count; i++)
            _effects[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void Update()
    {
        // Fire turret
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!_isUsing)
            {
                _isUsing = true;
                SkillStart();
            }
        }



        if (_isUsing)
        {
            //마우스 위치기준 피격위치 
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99))
            { transform.LookAt(hit.point);}      

        }



        // Stop firing
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_isUsing)
            {
                _isUsing = false;
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

