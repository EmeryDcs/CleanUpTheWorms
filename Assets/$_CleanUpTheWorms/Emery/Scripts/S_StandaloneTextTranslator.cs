using UnityEngine;
using TMPro;
using System.Linq;

public class S_StandaloneTextTranslator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private bool isEnglish;
    [SerializeField] private bool isJapanese;

    [TextArea(3, 10)]
    [SerializeField] private string textFrench;

    [TextArea(3, 10)]
    [SerializeField] private string textEnglish;

    [TextArea(3, 10)]
    [SerializeField] private string textJapanese;

    [SerializeField] private TMP_FontAsset japaneseFont;

    private void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        if (System.Environment.GetCommandLineArgs().Contains("-isjapanese"))
        {
            isJapanese = true;
        }
        else if (System.Environment.GetCommandLineArgs().Contains("-isinenglish"))
        {
            isEnglish = true;
        }

        if (isJapanese)
        {
            textComponent.text = textJapanese;
            if (japaneseFont != null)
            {
                textComponent.font = japaneseFont;
            }
        }
        else if (isEnglish)
        {
            textComponent.text = textEnglish;
        }
        else
        {
            textComponent.text = textFrench;
        }
    }
}