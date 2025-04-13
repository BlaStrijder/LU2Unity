using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Object2D : MonoBehaviour
{
    public ObjectManager objectManager;
    public bool isDragging = false;

    public void Update()
    {
        if (isDragging)
        {
            this.transform.position = GetMousePosition();
        }
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            objectManager.ShowMenu();
            objectManager.OnEndDrag(this.gameObject);
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }

}
