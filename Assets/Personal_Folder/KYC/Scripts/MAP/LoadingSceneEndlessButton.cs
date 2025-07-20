using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneEndlessButton : MonoBehaviour
{
    public Animator _fadeAnimation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadGameScene()
    {
        StartCoroutine(LoadSceneDelay());
    }

    IEnumerator LoadSceneDelay()
    {
        _fadeAnimation.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        LoadingSceneManager.LoadSceneForEndlessMode("EndlessModeScene");
    }
}

