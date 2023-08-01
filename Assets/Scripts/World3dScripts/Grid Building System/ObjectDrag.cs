using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset; //Save the distance between your click position and the center of an object to avoid any jumping

    private void OnMouseDown() //When mouse down, its give a value to offset (the place were we select vs the object we select)
    {
        offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    }
    private void OnMouseDrag() //
    {
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset; //New position of the mouse with the offset
        transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos); //the transform position will snap to the coordinates of the grid
    }

}
