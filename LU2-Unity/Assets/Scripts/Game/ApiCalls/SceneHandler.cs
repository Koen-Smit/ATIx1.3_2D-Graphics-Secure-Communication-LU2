using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Net;

public class SceneHandler : MonoBehaviour
{
    private List<SceneData> sceneList = new List<SceneData>();
    public GameObject World_1, World_2, World_3, World_4, World_5;
    [SerializeField] private TMP_InputField nameField, typeField, lengthField, heightField;
    public GameObject CreateWorldPanel;
    public TextMeshProUGUI ResultText;



    void Start()
    {
        CreateWorldPanel.SetActive(false);
        APIManager.Instance.GetRequest("/Scene", OnSceneReceived);
    }

    void OnSceneReceived(APIResponse response)
    {
        if (response.Success)
        {
            SceneListWrapper wrapper = JsonUtility.FromJson<SceneListWrapper>("{\"scenes\":" + response.Data + "}");
            sceneList.Clear();

            GameObject[] worlds = { World_1, World_2, World_3, World_4, World_5 };
            int sceneCount = wrapper.scenes.Length;

            for (int i = 0; i < worlds.Length; i++)
            {
                HoverAnimation animationHandles = worlds[i].GetComponent<HoverAnimation>();
                if (i < sceneCount)
                {
                    SceneData scene = wrapper.scenes[i];
                    sceneList.Add(scene);
                    animationHandles.tooltipText = scene.name;
                }
                else
                {
                    animationHandles.tooltipText = "Create World";
                }
            }
        }
    }

    public void OnWorldClicked(int worldIndex)
    {
        if (worldIndex < sceneList.Count)
        {
            LoadSceneByID(sceneList[worldIndex].id);
        }
        else
        {
            CreateNewWorld();
        }
    }
    public void LoadSceneByID(string sceneID)
    {
        APIManager.Instance.GetRequest("/Scene", OnSceneReceived);
    }

    public void CreateNewWorld()
    {
        CreateWorldPanel.SetActive(true);
    }

    public void OnCreateWorldButtonClicked()
    {
        SceneRequest newScene = new SceneRequest
        {
            name = nameField.text
        };

        if (int.TryParse(typeField.text, out int type))
            newScene.environmentType = type;

        if (int.TryParse(lengthField.text, out int maxLength))
            newScene.maxLength = maxLength;
        else
        {
            ResultText.text = "Max Length moet een geldig nummer zijn!";
            return;
        }

        if (int.TryParse(heightField.text, out int maxHeight))
            newScene.maxHeight = maxHeight;
        else
        {
            ResultText.text = "Max Height moet een geldig nummer zijn!";
            return;
        }

        string jsonData = JsonUtility.ToJson(newScene);
        APIManager.Instance.PostRequest("/Scene", jsonData, OnSceneRespone);
    }

    public void OnSceneRespone(APIResponse response)
    {
        if (response.Success)
        {
            ResultText.text = "";
            nameField.text = "";
            typeField.text = "";
            lengthField.text = "";
            heightField.text = "";
            CreateWorldPanel.SetActive(false);
            SceneData scene = JsonUtility.FromJson<SceneData>(response.Data);
            sceneList.Add(scene);
            LoadSceneByID(scene.id);
        }
        else
        {
            ResultText.text = "Kan wereld niet aanmaken.";
        }
    }

    public void OnBackButtonClicked()
    {
        ResultText.text = "";
        nameField.text = "";
        typeField.text = "";
        lengthField.text = "";
        heightField.text = "";
        CreateWorldPanel.SetActive(false);
    }
}

[System.Serializable]
public class SceneListWrapper
{
    public SceneData[] scenes;
}
