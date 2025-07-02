using UnityEngine;
using System;
using Akila.FPSFramework;
using System.Collections;
using Cinemachine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager Instance { get; private set; }

    public FirstPersonController playerController;
    public Inventory inventory;
    public CinemachineVirtualCamera fpsCamera;

    public static Action<float> PlayerHpChange;
    public static Action<float> PlayerMaxHpChange;

    public float playerHp;
    public float MaxHealth;

    public AudioSource _bonusRunSounds;

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
        _bonusRunSounds.Play();

        // z: 현재 위치에서 -0.3까지 부드럽게 이동
        yield return StartCoroutine(LerpInventoryZ(inventory.transform, inventory.transform.localPosition.z, -0.3f, 0.3f));

        // FOV: 60 → 70
        yield return StartCoroutine(LerpFOV(fpsCamera, 60f, 70f, 0.3f));

        yield return new WaitForSeconds(1.0f);

        playerController.walkSpeed = 4;
        playerController.sprintSpeed = 8;

        // FOV: 70 → 60
        yield return StartCoroutine(LerpFOV(fpsCamera, 65f, 60f, 0.3f));

        // z: -0.3에서 -0.5까지 다시 부드럽게 이동
        yield return StartCoroutine(LerpInventoryZ(inventory.transform, -0.3f, -0.5f, 0.3f));
    }

    private IEnumerator LerpInventoryZ(Transform target, float fromZ, float toZ, float duration)
    {
        Vector3 localPos = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.Lerp(fromZ, toZ, elapsed / duration);
            localPos.z = z;
            target.localPosition = localPos;
            yield return null;
        }

        // 보정
        localPos.z = toZ;
        target.localPosition = localPos;
    }

    private IEnumerator LerpFOV(CinemachineVirtualCamera cam, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.m_Lens.FieldOfView = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        // 보정
        cam.m_Lens.FieldOfView = to;
    }



}

