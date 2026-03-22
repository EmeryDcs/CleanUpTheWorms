using UnityEngine;
using TMPro;
using System.Linq;

public class S_StandaloneTextTranslator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;

    [SerializeField] private bool isEnglish;

    [TextArea(3, 10)]
    [SerializeField] private string textFrench;

    [TextArea(3, 10)]
    [SerializeField] private string textEnglish;

    private void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        if (System.Environment.GetCommandLineArgs().Contains("-isinenglish"))
        {
            isEnglish = true;
        }

        if (isEnglish)
        {
            textComponent.text = textEnglish;
        }
        else
        {
            textComponent.text = textFrench;
        }
    }
}