using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Player/Death Camera")]
    [RequireComponent(typeof(Camera), typeof(AudioListener), typeof(Rigidbody))]
    public class DeathCamera : MonoBehaviour
    {
        public static DeathCamera Instance;

        public float lookDistance = 5;
        public Vector3 lookOffset;

        public Camera Camera { get; private set; }
        public AudioListener AudioListener { get; private set; }
        private Rigidbody _rigidbody;

        private GameObject _Inventory;

        private AudioSource _dieBgm;

        public GameObject deathPanel;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            Camera = GetComponent<Camera>();
            AudioListener = GetComponent<AudioListener>();
            _rigidbody = GetComponent<Rigidbody>();
            _dieBgm = GetComponent<AudioSource>();

            


            
            // Rigidbody 설정
            _rigidbody.isKinematic = true; // 기본적으로 비활성화

            Disable();
        }

        /// <summary>
        /// Enables the death camera and applies torque for dramatic effect.
        /// </summary>
        /// <param name="self">The player object</param>
        /// <param name="killer">The killer to look at</param>
        public void Enable(GameObject self, GameObject killer)
        {
            if (!killer) return;

            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            _Inventory = Player_Manager.Instance.gameObject;

            transform.position = mainCam.transform.position;
            transform.rotation = mainCam.transform.rotation;

            _dieBgm.Play();
            GamePlayManager.instance.bgmController.StopCombatMusic(1.0f);

            _Inventory.gameObject.SetActive(false);

            // Look at killer's upper body
            Vector3 lookTarget = killer.transform.position + Vector3.up * 1.5f;
            Vector3 direction = lookTarget - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;

            // Enable rigidbody physics
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            // Apply torque (random spin)
            Vector3 randomTorque = new Vector3(
                Random.Range(-10f, 10f),
                0,0
            );
            _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);

            deathPanel.SetActive(true);
            deathPanel.GetComponent<DeadEnding>().Death();


            Camera.enabled = true;
            AudioListener.enabled = true;
        }

        public void Disable()
        {
            Camera.enabled = false;
            AudioListener.enabled = false;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = true;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
