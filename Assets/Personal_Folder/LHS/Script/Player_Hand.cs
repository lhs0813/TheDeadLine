using Akila.FPSFramework;
using System.Collections;
using UnityEngine;

public class Player_Hand : MonoBehaviour
{
    public static Player_Hand Instance { get; private set; }
    public Animator _playerHandAnim;

    public GameObject inventory;
    public GameObject interactableHud;

    private GameObject _player;
    private GameObject _playerCamera;
    public bool _isCharging = false;
    private FirstPersonController _fpc;
    private Damageable _playerDamagable;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _moveDuration = 0.3f;
    private float _moveTimer = 0f;

    public GameObject _Charge_Position;
    public Vector3 chargePosition;
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
        _player = GetComponentInParent<Player_Manager>().gameObject;
        _playerCamera = _player.transform.GetChild(0).gameObject;
        _fpc = _player.GetComponent<FirstPersonController>();
        _playerDamagable = _player.GetComponent<Damageable>();

    }


    public void Charge()
    {
        _playerHandAnim.SetTrigger("Charge");

        _startPosition = _player.transform.position;
        _targetPosition = _Charge_Position.transform.position;
        _moveTimer = 0f;

        _isCharging = true;

        StartCoroutine(CameraLock());
        
    }

    private void Update()
    {
        if (_isCharging)
        {
            _playerCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
            _player.transform.position = _Charge_Position.transform.position;
        }
    }

    private float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }

    IEnumerator CameraLock()
    {
        inventory.gameObject.SetActive(false);
        interactableHud.gameObject.SetActive(false);


        yield return new WaitForSeconds(0.4f);
        _playerDamagable.health = _playerDamagable.playerMaxHealth;

        Player_Manager.PlayerMaxHpChange?.Invoke(_playerDamagable.playerMaxHealth);
        Player_Manager.PlayerHpChange?.Invoke(_playerDamagable.health);

        CameraShake.Instance.ShakeCamera(1.0f, 0.2f);
        yield return new WaitForSeconds(1.8f);
        _isCharging = false;

        if (_fpc != null)
        {
            Vector3 camEuler = _fpc._Camera.rotation.eulerAngles;

            _fpc.SetRotationAngles(NormalizeAngle(camEuler.x), NormalizeAngle(camEuler.y));
        }

        inventory.gameObject.SetActive(true);
        interactableHud.gameObject.SetActive(true);
    }

}
