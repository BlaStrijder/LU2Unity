using UnityEngine;

public class PostObjectResponseDto
{
    public string id;
    public string environmentId = ApiClient.Instance.currentEnvironmentId;
    public string prefabId;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;
}
