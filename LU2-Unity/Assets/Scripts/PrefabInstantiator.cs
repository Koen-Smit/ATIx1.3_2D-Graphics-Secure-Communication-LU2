using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{
    public Vector3 position;
    public GameObject prefab;

    public void CreateInstanceOfPrefab()
    {
        Instantiate(prefab, position, Quaternion.identity);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frameWS
    void Update()
    {
        
    }
}
