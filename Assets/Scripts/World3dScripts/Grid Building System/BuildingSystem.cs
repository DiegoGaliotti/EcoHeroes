using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap Maintilemap;
    [SerializeField] private TileBase whiteTile;

    public GameObject prefab;
    

    private PlaceableOject objectToPlace;

    #region Unity methodss

    private void Awake()
    { 
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update()
    {
        //Todo write the method when push de botton.
        if(Input.GetKeyDown(KeyCode.A))
        {
            InitializeWithObject(prefab);
        }
    }

    #endregion

    #region Utils

    public static Vector3 GetMouseWorldPosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;

        }
        else
        {
            return Vector3.zero;
        }

    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        //Tomas la posicion en el mundo con numeros decimales y la convertis en discrtea.
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        //es el centro de la celda correspondiente a la posición cellPos en el mundo,
        //y este valor se almacena nuevamente en la variable positio
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }


    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        objectToPlace = obj.GetComponent<PlaceableOject>();

        obj.AddComponent<ObjectDrag>();
       

    }
    
    #endregion
}
