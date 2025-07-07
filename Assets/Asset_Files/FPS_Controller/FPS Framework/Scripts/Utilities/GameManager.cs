using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Managers/Game Manager")]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] DeathCamera deathCamera;
        [SerializeField] DeathCamera endlessDeathCamera;
        [SerializeField] UIManager uIManager;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
                Destroy(gameObject);

            Time.timeScale = 1f;


            if (SceneManager.GetActiveScene().name == "StoryMode" || SceneManager.GetActiveScene().name == "StoryModeLoop")
            {
                Instantiate(deathCamera, transform);
            }
            else if (SceneManager.GetActiveScene().name == "EndlessModeScene")
            {
                Instantiate(endlessDeathCamera, transform);
            }



            Instantiate(uIManager, transform);
        }
    }
}