using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    public Animator _fadeAnimation;

    private void Start()
    {
        
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadSceneDelay());
    }


    IEnumerator LoadSceneDelay()
    {
        _fadeAnimation.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.5f);
        LoadingSceneManager.LoadScene("StoryMode");
    }

}
