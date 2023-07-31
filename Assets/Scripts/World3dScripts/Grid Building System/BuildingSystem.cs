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

        if(!objectToPlace)
        {
            return;
        }


        //Check the area.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            objectToPlace.Place();
            Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
            TakeArea(start, objectToPlace.Size);
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(objectToPlace.gameObject);
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

    public static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    //esta función permite seleccionar y extraer los azulejos que están presentes
    //dentro de un área específica en un Tilemap

    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach(var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
            
        }
        return array;


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

    private bool CanBePlaced(PlaceableOject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeableObject.Size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z + 1);

        TileBase[] baseArray = GetTilesBlock(area, Maintilemap);

        foreach(var b in baseArray)
        {
            if(b == whiteTile)
            {
                return false;
            }

        }

        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {

        Maintilemap.BoxFill(start, whiteTile, start.x, start.y,
            start.x + size.x, start.y + size.y);
    }
    
    #endregion
}
