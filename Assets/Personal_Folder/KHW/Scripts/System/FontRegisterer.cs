using TMPro;
using UnityEngine;

public class FontRegisterer : MonoBehaviour
{
    TextMeshProUGUI tmp;
    void OnEnable()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.font = FontManager.currentFont;
    }
}
