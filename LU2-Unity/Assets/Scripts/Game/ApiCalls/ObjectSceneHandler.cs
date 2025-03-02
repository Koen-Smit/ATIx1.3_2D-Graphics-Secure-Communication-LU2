using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSceneHandler : MonoBehaviour
{
    public TextMeshProUGUI ResultText;
    public string sceneId = "2aceb5e8-4754-4bc4-aef2-2c9975bb090a";
    public string scenePath;
    public string entityPath;
    public string sceneName;

    void Start()
    {
        scenePath = "/Scene/" + sceneId;
        APIManager.Instance.GetRequest(scenePath, OnSceneReceived);
    }

    public void OnDeleteButtonPressed()
    {
        APIManager.Instance.DeleteRequest(scenePath, OnDeleteResponse);
    }

    private void OnDeleteResponse(APIResponse response)
    {
        if (response.Success)
            SceneManager.LoadScene(1);
        else
            ResultText.text = "Fout bij het verwijderen.";
    }

    private void OnSceneReceived(APIResponse response)
    {
        if (response.Success)
        {
            SceneData scene = JsonUtility.FromJson<SceneData>(response.Data);
            sceneId = scene.id;
            sceneName = scene.name;

            var entityPath = "/Scene/" + sceneId + "/entities";
            APIManager.Instance.GetRequest(entityPath, OnEntitiesReceived);
        }
        else
        {
            Debug.LogError("Failed to receive scene: " + response.StatusCode);
            SceneManager.LoadScene(1);
        }
    }

    private void OnEntitiesReceived(APIResponse response)
    {
        if (response.Success)
        {
            EntityListWrapper entityList = JsonUtility.FromJson<EntityListWrapper>("{\"entities\":" + response.Data + "}");
            if (entityList.entities.Length > 0)
            {
                InstantiateEntities(entityList.entities);
                ResultText.text = "Objecten ingeladen.";
            }
            else
            {
                ResultText.text = "Geen objecten gevonden.";
            }
        }
        else if (response.StatusCode == 404)
        {
            ResultText.text = "Geen objecten gevonden.";
        }
        else
        {
            ResultText.text = "Fout bij het inladen.";
        }
    }

    private void InstantiateEntities(EntityData[] entities)
    {
        foreach (EntityData entity in entities)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + entity.prefab_Id);
            if (prefab == null)
            {
                Debug.LogError("Prefab not found: " + entity.prefab_Id);
                continue;
            }

            Vector3 position = new Vector3(entity.positionX, entity.positionY, 0);
            Vector3 scale = new Vector3(entity.scaleX, entity.scaleY, 1);

            GameObject newEntity = Instantiate(prefab, position, Quaternion.Euler(0, 0, entity.rotationZ));
            newEntity.name = entity.prefab_Id;
            newEntity.transform.localScale = scale;
        }
    }
}

[System.Serializable]
public class EntityListWrapper
{
    public EntityData[] entities;
}


