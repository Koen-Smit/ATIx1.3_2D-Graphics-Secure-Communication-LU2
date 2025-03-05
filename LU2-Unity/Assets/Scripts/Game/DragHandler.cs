using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.5f;

    public static bool isDraggingDisabled = false;

    private Vector3 originalScale;
    private Vector3 dragScale = new Vector3(1.2f, 1.2f, 1f);

    private void Start()
    {
        if (!GetComponent<Collider2D>())
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isDragging && !isDraggingDisabled)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousePosition.z = 0;
            transform.position = mousePosition;
            transform.localScale = dragScale;
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

        transform.localScale = dragScale;
    }

    private void OnMouseUp()
    {
        if (!isDraggingDisabled)
        {
            isDragging = false;
            transform.localScale = originalScale;
        }
    }
}



//using UnityEngine;

//public class Draggable : MonoBehaviour
//{
//    public static bool isDraggingDisabled = false;
//    public GameObject prefabToClone;
//    private bool hasCloned = false;

//    private bool isDragging = false;

//    private void Update()
//    {
//        if (isDragging && !isDraggingDisabled)
//        {
//            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
//            mousePosition.z = 0;
//            transform.position = mousePosition;
//        }
//    }

//    private void OnMouseDown()
//    {
//        isDragging = true;
//        if (!hasCloned)
//        {
//            Clone();
//            hasCloned = true;
//        }
//    }

//    private void Clone()
//    {
//        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
//        Draggable cloneHandler = clone.GetComponent<Draggable>();
//        clone.transform.localScale = new Vector3(5, 5, 5);
//        cloneHandler.hasCloned = false;
//    }
//    private void OnMouseUp()
//    {
//        isDragging = false;

//        Destroy(this);
//        Debug.Log("Dropped, api request");
//    }
//}


//using UnityEngine;

//public class DragObjects : MonoBehaviour
//{
//    public static bool isDraggingDisabled = false;
//    public GameObject prefabToClone;
//    private bool hasCloned = false;

//    private bool isDragging = false;

//    private void Update()
//    {
//        if (isDragging && !isDraggingDisabled)
//        {
//            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
//            mousePosition.z = 0;
//            transform.position = mousePosition;
//        }
//    }

//    private void OnMouseDown()
//    {
//        isDragging = true;
//        if (!hasCloned)
//        {
//            Clone();
//            hasCloned = true;
//        }
//    }

//    private void Clone()
//    {
//        if (prefabToClone != null)
//        {
//            GameObject clone = Instantiate(prefabToClone, transform.position, transform.rotation);
//            clone.transform.localScale = new Vector3(7f, 7f, 5f);
//            clone.transform.SetParent(transform.parent, true);

//            DragObjects cloneHandler = clone.GetComponent<DragObjects>();
//            if (cloneHandler != null)
//            {
//                cloneHandler.hasCloned = false;
//            }
//        }
//        else
//        {
//            Debug.LogError("Prefab to clone is not assigned!");
//        }
//    }

//    private void OnMouseUp()
//    {
//        isDragging = false;

//        Destroy(this);
//        Debug.Log("Dropped, API request");
//    }
//}
