using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        int currentStage = GamePlayManager.instance.currentMapIndex - 2;
        RecordManager.Instance.RecordInfiniteStage(currentStage);
        SceneManager.LoadScene("Main_Menu");
    }

}
