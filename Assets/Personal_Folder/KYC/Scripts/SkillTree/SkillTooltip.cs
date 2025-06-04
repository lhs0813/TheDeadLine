using TMPro;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    public GameObject panel;
    public TextMeshProUGUI nameText, descText, pointText;
    public VideoPlayer videoPlayer;
    public RawImage previewImage;
    public RenderTexture renderTexture;

    private void Awake()
    {
        Instance = this;

        videoPlayer.targetTexture = renderTexture;
        previewImage.texture = renderTexture;

        // 🎯 준비 완료 후 play
        videoPlayer.prepareCompleted += vp =>
        {
            Debug.Log("[VideoPlayer] prepareCompleted → Play()");
            vp.Play();
        };

        // ❗ 디버깅용
        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError($"[VideoPlayer ERROR] {msg}");
        };

        panel.SetActive(false);
    }



    public void Show(string name, string desc, int cost, VideoClip clip)
    {
        panel.SetActive(true);

        nameText.text = name;
        descText.text = desc;
        pointText.text = $"Need Point: {cost}";

        videoPlayer.clip = clip;
        videoPlayer.Prepare(); // 🎯 중요: Prepare 먼저
        Debug.Log("[VideoPlayer] Prepare() 호출됨");
    }

    public void Hide()
    {
        videoPlayer.Stop();
        panel.SetActive(false);
    }
  

}
