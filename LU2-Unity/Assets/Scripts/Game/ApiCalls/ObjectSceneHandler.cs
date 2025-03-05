using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectSceneHandler : MonoBehaviour
{
    public bool IsSharedScene;
    public Button ShareButton, SaveButton, DeleteButton;
    public Button zero, one, two;
    public TextMeshProUGUI ResultText;
    private string sceneId;
    private List<EntityData> entityList = new List<EntityData>();
    public GameObject DragSaver;
    public GameObject ShareMenuPanel;
    public TMP_InputField SharedUserName;
    void Start()
    {
        if (PlayerPrefs.HasKey("SelectedSharedSceneID"))
        {
            sceneId = PlayerPrefs.GetString("SelectedSharedSceneID");
            PlayerPrefs.DeleteKey("SelectedSharedSceneID");
            APIManager.Instance.GetRequest($"/Scene/{sceneId}", OnSceneReceived);
            IsSharedScene = true;
            ShareButton.gameObject.SetActive(false);
            SaveButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(false);
            zero.gameObject.SetActive(false);
            one.gameObject.SetActive(false);
            two.gameObject.SetActive(false);
            Draggable.isDraggingDisabled = true;
        }
        else if (PlayerPrefs.HasKey("SelectedSceneID"))
        {
            sceneId = PlayerPrefs.GetString("SelectedSceneID");
            APIManager.Instance.GetRequest($"/Scene/{sceneId}", OnSceneReceived);
            IsSharedScene = false;
            ShareButton.gameObject.SetActive(true);
            SaveButton.gameObject.SetActive(true);
            DeleteButton.gameObject.SetActive(true);
            zero.gameObject.SetActive(true);
            one.gameObject.SetActive(true);
            two.gameObject.SetActive(true);
            Draggable.isDraggingDisabled = false;
        }
        else
        {
            SceneManager.LoadScene("PlanetSelector");
        }
    }
    public void OnShareButtonPressed()
    {
        ShareMenuPanel.SetActive(true);

    }

    public void OnExitButton()
    {
        ShareMenuPanel.SetActive(false);
    }

    public void ShareWorld()
    {
        var jsonData = "{\"sharedUserName\": \"" + SharedUserName.text + "\", \"worldId\": \"" + sceneId + "\"}";
        APIManager.Instance.PostRequest("/share/scene", jsonData, HandleGetSharesResponse);
    }

    private void HandleGetSharesResponse(APIResponse response)
    {
        if (response.Success)
        {
            ShareMenuPanel.SetActive(false);
            ResultText.text = "Wereld gedeeld.";
        }
    }

    public void OnBackButtonPressed() => SceneManager.LoadScene("PlanetSelector");
    public void OnDeleteButtonPressed() => APIManager.Instance.DeleteRequest($"/Scene/{sceneId}", OnDeleteResponse);
    private void OnDeleteResponse(APIResponse response)
    {
        if (response.Success) SceneManager.LoadScene("PlanetSelector");
        else ResultText.text = "Fout bij het verwijderen.";
    }
    private void OnSceneReceived(APIResponse response)
    {
        if (!response.Success)
        {
            SceneManager.LoadScene(1);
            return;
        }

        SceneData scene = JsonUtility.FromJson<SceneData>(response.Data);
        sceneId = scene.id;
        APIManager.Instance.GetRequest($"/Scene/{sceneId}/entities", OnEntitiesReceived);
    }

    private void OnEntitiesReceived(APIResponse response)
    {
        entityList.Clear();
        if (response.Success)
        {
            var entities = JsonUtility.FromJson<EntityListWrapper>("{\"entities\": " + response.Data + "}").entities;
            entityList.AddRange(entities);
            InstantiateEntities(entities);
            if (!IsSharedScene)
                ResultText.text = entities.Length > 0 ? "Objecten ingeladen. Dubbel klik een object om het te verwijderen" : "Geen objecten gevonden.";
        }
        else ResultText.text = response.StatusCode == 404 ? "Geen objecten gevonden." : "Fout bij het inladen.";
    }
    private void InstantiateEntities(EntityData[] entities)
    {
        foreach (var entity in entities)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{entity.prefab_Id.Replace(" ", "-")}");
            if (prefab == null) continue;

            Vector3 position = new Vector3(entity.positionX, entity.positionY, 0);
            Vector3 scale = new Vector3(entity.scaleX, entity.scaleY, 1);

            if (entity.positionX < -11 || entity.positionX > 11 || entity.positionY < -5 || entity.positionY > 3)
                position = Vector3.zero;

            if (entity.scaleX < 0 || entity.scaleY < 0)
                scale = new Vector3(5, 5, 1);

            GameObject newEntity = Instantiate(prefab, position, Quaternion.Euler(0, 0, entity.rotationZ));
            newEntity.name = entity.prefab_Id;
            newEntity.transform.localScale = scale;
            newEntity.tag = "DraggableEntity";

            if (!newEntity.GetComponent<Draggable>()) newEntity.AddComponent<Draggable>();
            if (!newEntity.GetComponent<Collider2D>()) newEntity.AddComponent<BoxCollider2D>();

            var entityComponent = newEntity.GetComponent<EntityComponent>() ?? newEntity.AddComponent<EntityComponent>();
            entityComponent.id = entity.id;
        }
    }

    public void SaveObjects()
    {
        var entitiesToDelete = new List<EntityData>();
        var entitiesToSave = new List<EntityData>();
        var sceneEntities = GameObject.FindGameObjectsWithTag("DraggableEntity");

        foreach (var entity in entityList)
        {
            if (!sceneEntities.Any(obj => obj.GetComponent<EntityComponent>()?.id == entity.id))
                entitiesToDelete.Add(entity);
        }

        foreach (var obj in sceneEntities)
        {
            var entityComponent = obj.GetComponent<EntityComponent>();
            if (entityComponent == null) continue;

            if (string.IsNullOrEmpty(entityComponent.id) || entityComponent.id == "0")
            {
                entityComponent.id = System.Guid.NewGuid().ToString();
            }

            var updatedEntity = new EntityData
            {
                id = entityComponent.id,
                prefab_Id = obj.name.Replace("(Clone)", ""),
                positionX = obj.transform.position.x,
                positionY = obj.transform.position.y,
                scaleX = obj.transform.localScale.x,
                scaleY = obj.transform.localScale.y,
                rotationZ = obj.transform.rotation.eulerAngles.z,
                sortingLayer = -1,
                environmentId = sceneId
            };

            var existingEntity = entityList.Find(e => e.id == entityComponent.id);
            if (existingEntity == null)
            {
                entitiesToSave.Add(updatedEntity);
            }
            else if (!IsSameEntity(existingEntity, updatedEntity))
            {
                APIManager.Instance.PutRequest($"/scene/entity/{updatedEntity.id}", JsonUtility.ToJson(updatedEntity), OnSaveResponse);
            }
        }

        foreach (var entity in entitiesToDelete)
        {
            APIManager.Instance.DeleteRequest($"/scene/entity/{entity.id}", response => OnEntityDeleteResponse(response, entity.id));
        }

        foreach (var entity in entitiesToSave)
        {
            APIManager.Instance.PostRequest("/scene/entity", JsonUtility.ToJson(entity), OnSaveResponse);
            entityList.Add(entity);
        }
        ResultText.text = "Objecten opgeslagen.";
    }
    private bool IsSameEntity(EntityData a, EntityData b) =>
        a != null && b != null &&
        a.positionX == b.positionX && a.positionY == b.positionY &&
        a.scaleX == b.scaleX && a.scaleY == b.scaleY &&
        a.rotationZ == b.rotationZ;

    private void OnSaveResponse(APIResponse response)
    {
        if (response.Success) return;
    }
    private void OnEntityDeleteResponse(APIResponse response, string entityId)
    {
        if (response.Success)
        {
            entityList.RemoveAll(e => e.id == entityId);
        }
    }
}
