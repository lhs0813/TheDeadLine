using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead_Btn : MonoBehaviour
{
    public void Gomain()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void retry()
    {
        if (SceneManager.GetActiveScene().name == "StoryMode" || SceneManager.GetActiveScene().name == "StoryModeLoop" || SceneManager.GetActiveScene().name == "EndingScene")
        {
            SceneManager.LoadScene("StoryModeLoop");
        }
        else if (SceneManager.GetActiveScene().name == "EndlessModeScene")
        {
            SceneManager.LoadScene("EndlessModeScene");
        }
    }


}
