using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    public GameObject panel;
    public TextMeshProUGUI nameText, descText, pointText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }


    public void Show(string nameKey, string descKey, int cost)
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
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
  

}
