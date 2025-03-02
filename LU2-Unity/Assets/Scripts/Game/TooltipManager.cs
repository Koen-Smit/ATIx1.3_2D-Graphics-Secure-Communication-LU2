using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public TMP_Text tooltipText;

    private void Awake()
    {
        Instance = this;
        tooltipText.text = "";
    }

    public void ShowTooltip(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (message == tooltipText.text)
        {
            return;
        }
        tooltipText.text = message;
    }

    public void HideTooltip()
    {
        tooltipText.text = "";
    }
}
