using System;
using UnityEngine;
[Serializable]
public class PostObjectRequestDto
{
    public string environmentId = ApiClient.Instance.currentEnvironmentId;
    public string prefabId;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;
    public void UpdateFromTransform(Transform transform)
    {
        positionX = transform.position.x;
        positionY = transform.position.y;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        rotationZ = transform.rotation.eulerAngles.z;
    }
}
