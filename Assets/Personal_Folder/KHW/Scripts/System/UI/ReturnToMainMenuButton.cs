using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenuButton : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        if (!GamePlayManager.instance.isStoryMode)
        {
            int currentStage = GamePlayManager.instance.currentMapIndex - 2;
            RecordManager.Instance.RecordInfiniteStage(currentStage);
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;
        SceneManager.LoadScene("Main_Menu");
    }

}
