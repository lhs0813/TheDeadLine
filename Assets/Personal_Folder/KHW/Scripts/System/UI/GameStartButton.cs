using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    public void LoadGameScene()
    {
        LoadingSceneManager.LoadScene("StoryMode");
    }
}
