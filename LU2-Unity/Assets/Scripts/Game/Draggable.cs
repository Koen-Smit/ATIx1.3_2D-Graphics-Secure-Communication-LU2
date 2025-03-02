using System;
using System.Runtime.Serialization;
using UnityEngine;

/*
* The GameObject also needs a collider otherwise OnMouseUpAsButton() can not be detected.
*/
public class Draggable : MonoBehaviour
{
    public Transform trans;

    private bool isDragging = false;

    public void StartDragging()
    {
        isDragging = true;
    }

    public void Update()
    {
        if (isDragging)
            trans.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            // Stopped dragging. Add any logic here that you need for this scenario.
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }

}


//using System;
//using UnityEngine;
//using UnityEngine.Events;

//public class Object2D : MonoBehaviour
//{
//    public ObjectManager objectManager;

//    public bool isDragging = false;

//    public void Update()
//    {
//        if (isDragging)
//            this.transform.position = GetMousePosition();
//    }

//    private void OnMouseUpAsButton()
//    {
//        isDragging = !isDragging;

//        if (!isDragging)
//        {
//            objectManager.ShowMenu();
//        }
//    }

//    private Vector3 GetMousePosition()
//    {
//        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        positionInWorld.z = 0;
//        return positionInWorld;
//    }

//}


//public class ObjectManager : MonoBehaviour
// {
//     // Menu om objecten vanuit te plaatsen
//     public GameObject UISideMenu;
//     // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
//     public List<GameObject> prefabObjects;

//     // Lijst met objecten die geplaatst zijn in de wereld
//     private List<GameObject> placedObjects;

//     // Methode om een nieuw 2D object te plaatsen
//     public void PlaceNewObject2D(int index)
//     {
//         // Verberg het zijmenu
//         UISideMenu.SetActive(false);
//         // Instantieer het prefab object op de positie (0,0,0) met geen rotatie
//         GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);
//         // Haal het Object2D component op van het nieuw geplaatste object
//         Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();
//         // Stel de objectManager van het object in op deze instantie van ObjectManager
//         object2D.objectManager = this;
//         // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
//         object2D.isDragging = true;
//     }

//     // Methode om het menu te tonen
//     public void ShowMenu()
//     {
//         UISideMenu.SetActive(true);
//     }

//     // Methode om de huidige sc�ne te resetten
//     public void Reset()
//     {
//         // Laad de huidige sc�ne opnieuw
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }
// }


//public class PrefabInstantiator : MonoBehaviour
//{
//    public Vector3 position;
//    public GameObject prefab;

//    public void CreateInstanceOfPrefab()
//    {
//        Instantiate(prefab, position, Quaternion.identity);
//    }
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frameWS
//    void Update()
//    {

//    }
//}
