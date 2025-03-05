using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ShareHandler : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject shareItemPrefab;

    private void Start()
    {
        APIManager.Instance.GetRequest("/share/scenes", HandleGetSharesResponse);
    }

    private void HandleGetSharesResponse(APIResponse response)
    {
        if (!response.Success || string.IsNullOrEmpty(response.Data)) return;

        foreach (Transform child in scrollViewContent.transform)
            Destroy(child.gameObject);

        ShareScene[] scenes = JsonHelper.FromJson<ShareScene>(response.Data);

        if (scenes == null || scenes.Length == 0) return;

        foreach (ShareScene scene in scenes)
        {
            if (scene == null) continue;

            GameObject shareItem = Instantiate(shareItemPrefab, scrollViewContent.transform);
            TMP_Text shareItemText = shareItem.GetComponentInChildren<TMP_Text>();

            if (shareItemText != null)
                shareItemText.text = scene.environmentName;

            Button button = shareItem.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => OnShareItemClicked(scene.environmentId));
        }
    }

    private void OnShareItemClicked(string sceneID)
    {
        PlayerPrefs.SetString("SelectedSharedSceneID", sceneID);
        SceneManager.LoadScene("PlanetScene");
    }

    public void OnBackButtonPressed() => SceneManager.LoadScene("PlanetSelector");
}

[Serializable]
public class ShareScene
{
    public string environmentId;
    public string environmentName;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
