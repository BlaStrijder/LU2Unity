using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GetEnvironmentObjectsResponseDto
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
[Serializable]
public class ObjectsListWrapper
{
    public List<GetEnvironmentObjectsResponseDto> objects;
}
