using System;
using UnityEngine;

[Serializable]
public class PutObjectRequestDto
{
    public string environmentId;
    public string prefabId;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;

    public void CopyFrom(PostObjectRequestDto postData)
    {
        environmentId = postData.environmentId;
        prefabId = postData.prefabId;
        positionX = postData.positionX;
        positionY = postData.positionY;
        scaleX = postData.scaleX;
        scaleY = postData.scaleY;
        rotationZ = postData.rotationZ;
        sortingLayer = postData.sortingLayer;
    }
}
