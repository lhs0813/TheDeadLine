using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
 
    [SerializeField] private float minLoadTime = 3f; // 최소 로딩 시간
    [SerializeField] private Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    public static void LoadSceneForEndlessMode(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingSceneToInfinity");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (elapsedTime < minLoadTime)
        {
            elapsedTime += Time.deltaTime;
            float timeRatio = elapsedTime / minLoadTime;

            // 실제 로딩 진행도와 시간 기반 진행도를 비교해 더 느린 쪽을 선택
            float loadProgress = Mathf.Clamp01(op.progress / 0.9f); // 0~1로 정규화
            float visualProgress = Mathf.Min(timeRatio, loadProgress);

            progressBar.fillAmount = visualProgress;

            yield return null;
        }

        // 최소 시간은 지났으니 실제 로딩 완료 여부만 기다림
        while (op.progress < 0.9f)
        {
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress / 0.9f, Time.deltaTime * 5f);
            yield return null;
        }

        // 마지막 10% 채움
        while (progressBar.fillAmount < 1f)
        {
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, Time.deltaTime * 5f);
            yield return null;
        }

        op.allowSceneActivation = true;
    }

}