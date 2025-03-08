using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.5f;

    public static bool isDraggingDisabled = false;

    private Vector3 originalScale;
    private Vector3 dragScale = new Vector3(1.2f, 1.2f, 1f);
    private Vector3 baseScale;

    private void Start()
    {
        if (!GetComponent<Collider2D>())
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        originalScale = transform.localScale;
        baseScale = originalScale;
    }

    private void Update()
    {
        if (isDragging && !isDraggingDisabled)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousePosition.z = 0;
            transform.position = mousePosition;
            //transform.localScale = dragScale;
            transform.localScale = baseScale * 1.2f;

            if (Input.GetKey(KeyCode.R))
            {
                transform.Rotate(0, 0, 5f);
            }
            if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
            {
                baseScale *= 1.1f;
                baseScale.x = Mathf.Clamp(baseScale.x, 1, 10);
                baseScale.y = Mathf.Clamp(baseScale.y, 1, 10);
                transform.localScale = baseScale * (isDragging ? 1.2f : 1f);
            }
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                baseScale *= 0.9f;
                baseScale.x = Mathf.Clamp(baseScale.x, 1, 10);
                baseScale.y = Mathf.Clamp(baseScale.y, 1, 10);
                transform.localScale = baseScale * (isDragging ? 1.2f : 1f);
            }
        }
    }

    private void OnMouseDown()
    {
        if (gameObject.tag != "DraggableEntity" || isDraggingDisabled) return;
        isDragging = true;

        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            Destroy(gameObject);
        }
        else
        {
            lastClickTime = Time.time;
        }
        //transform.localScale = dragScale;
        transform.localScale = baseScale * 1.2f;
    }

    private void OnMouseUp()
    {
        if (!isDraggingDisabled)
        {
            isDragging = false;
            //transform.localScale = originalScale;
            transform.localScale = baseScale;
        }
    }
}