using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    public TextMeshProUGUI worldName;
    public TextMeshProUGUI worldNameShadow;

    public GameObject uiSideMenu;

    public Transform canvasTransform;
    public Canvas canvas;

    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    // Lijst met objecten die geplaatst zijn in de wereld
    private List<TrackedObject> placedObjects = new List<TrackedObject>();
    public class TrackedObject
    {
        public GameObject instance;
        public string id;
        public PostObjectRequestDto data;
    }

    public async Task Start()
    {
        await ApiClient.Instance.GetCurrentEnvironment();
        worldName.text = ApiClient.Instance.currentEnvironmentName;
        worldNameShadow.text = ApiClient.Instance.currentEnvironmentName;
        await LoadObjects();
    }

    //loads environments and puts them inside a list
    public async Task LoadObjects()
    {
        List<GetEnvironmentObjectsResponseDto> loadedObjects = await ApiClient.Instance.GetEnvironmentObjects();
    
        foreach (var objData in loadedObjects)
        {
            GameObject matchingPrefab = prefabObjects.Find(prefab => prefab.name == objData.prefabId);

            //Convert saved world space position to screen space position
            Vector3 worldPos = new Vector3(objData.positionX, objData.positionY, 0);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            //Convert screen position to local position inside canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPos,
                canvas.worldCamera,  
                out Vector2 localPoint
            );

            //Instantiate on canvas
            GameObject instance = Instantiate(matchingPrefab, canvasTransform.transform);
            RectTransform rectTransform = instance.GetComponent<RectTransform>();

            //Set object's anchored position, rotation, and scale
            rectTransform.anchoredPosition = localPoint;
            rectTransform.localRotation = Quaternion.Euler(0, 0, objData.rotationZ);
            rectTransform.localScale = new Vector3(objData.scaleX, objData.scaleY, 1);

            //Getting the instance object data
            Object2D object2D = instance.GetComponent<Object2D>();
            object2D.objectManager = this;
            object2D.isDragging = false;

            //Putting object data in var data
            var data = new PostObjectRequestDto
            {
                environmentId = objData.environmentId,
                prefabId = objData.prefabId,
                positionX = objData.positionX,
                positionY = objData.positionY,
                scaleX = objData.scaleX,
                scaleY = objData.scaleY,
                rotationZ = objData.rotationZ,
                sortingLayer = objData.sortingLayer
            };
            
            //Putting all data inside list to track the object
            placedObjects.Add(new TrackedObject
            {
                instance = instance,
                id = objData.id,
                data = data
            });
        }
    }

    //Places new object on screen and adds it to the list
    public async void PlaceNewObject2D(int index)
    {
        uiSideMenu.SetActive(false);

        GameObject instance = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);
        instance.transform.SetParent(canvasTransform, false);

        var data = new PostObjectRequestDto
        {
            prefabId = $"{prefabObjects[index].name}",
            sortingLayer = 100
        };
        data.UpdateFromTransform(instance.transform);

        var tracked = new TrackedObject
        {
            instance = instance,
            data = data
        };
        placedObjects.Add(tracked);

        Object2D object2D = instance.GetComponent<Object2D>();
        object2D.objectManager = this;
        object2D.isDragging = true;

        var response = await ApiClient.Instance.CreateObject(data);
        if (!string.IsNullOrEmpty(response))
        {
            var responseDto = JsonUtility.FromJson<PostObjectResponseDto>(response);
            tracked.id = responseDto.id; 
        }
    }
    
    //Updates the object after dropping the object
    public void OnEndDrag(GameObject draggedObject)
    {
        var allObjects = GetPlacedObjectData();

        foreach (TrackedObject tracked in allObjects)
        {
            if (tracked.instance == draggedObject)
            {
                tracked.data.UpdateFromTransform(draggedObject.transform);

                var putData = new PutObjectRequestDto();
                putData.CopyFrom(tracked.data);

                //Debug.Log($"Updating ID: {tracked.id}");
                ApiClient.Instance.UpdateObject(tracked.id, putData);
                break;
            }
        }
    }
    public List<TrackedObject> GetPlacedObjectData()
    {
        return placedObjects;
    }

    public void ShowMenu()
    {
        uiSideMenu.SetActive(true);
    }
}