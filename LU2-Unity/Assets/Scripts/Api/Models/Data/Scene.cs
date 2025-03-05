using System;
using UnityEngine;


public class EnvironmentScene
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int MaxLength { get; set; }
    public int MaxHeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

[System.Serializable]
public class SceneData
{
    public string id;
    public string name;
}

[System.Serializable]
public class SceneRequest
{
    public string name;
    public int environmentType;
    public int maxLength;
    public int maxHeight;
}

[System.Serializable]
public class EntityData
{
    public string id;
    public string prefab_Id;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;
    public string environmentId;
}

[System.Serializable]
public class EntityDataRequest
{
    public string prefab_Id;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;
    public string environmentId;
}

[System.Serializable]
public class EntityListWrapper
{
    public EntityData[] entities;
}

[System.Serializable]
public class SavedObject
{
    public string prefabName;
    public Vector3 position;
}

[System.Serializable]
public class SceneListWrapper
{
    public SceneData[] scenes;
}

public class EntityComponent : MonoBehaviour
{
    public string id;
}