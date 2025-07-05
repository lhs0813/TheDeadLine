using UnityEngine;

public class LoadingSceneEndlessButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadGameScene()
    {
        LoadingSceneManager.LoadSceneForEndlessMode("EndlessModeScene");
    }
}
