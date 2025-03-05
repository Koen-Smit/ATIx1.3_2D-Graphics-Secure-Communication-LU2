using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] public string tooltipText;
    private Vector3 originalScale;
    private Transform targetTransform;

    private void Start()
    {
        targetTransform = transform;
        originalScale = targetTransform.localScale;
    }

    private void OnEnable()
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetTransform.localScale = originalScale * scaleFactor;

        if (TooltipManager.Instance != null)
        {
            if (tooltipText == null)
            {
                tooltipText = "";
            }
            TooltipManager.Instance.ShowTooltip(tooltipText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetTransform.localScale = originalScale;

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}
