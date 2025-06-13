using TMPro;
using UnityEngine.Video;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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


    public void Show(string nameKey, string descKey, int cost, VideoClip clip)
    {
        panel.SetActive(true);

        var localizedName = new LocalizedString("SkillTable", nameKey);
        var localizedDesc = new LocalizedString("SkillTable", descKey);
        var localizedCost = new LocalizedString("NoticeTable", "PointText");

        localizedName.StringChanged += value => nameText.text = value;
        localizedDesc.StringChanged += value => descText.text = value;

        localizedCost.Arguments = new object[] { cost };
        localizedCost.StringChanged += value =>
        {
            Debug.Log("[Localized PointText] " + value);
            pointText.text = value;
        };

        videoPlayer.clip = clip;
        videoPlayer.Prepare();
    }

    public void Hide()
    {
        videoPlayer.Stop();
        panel.SetActive(false);
    }
  

}
