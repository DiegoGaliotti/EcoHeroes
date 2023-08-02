using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout; //Accessible from another scripts
    private Grid grid; //Main grid system
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase whiteTile; //White tile indicator taken area

    public GameObject prefab1; //For the prefab que want to locate
    public GameObject prefab2;

    private PlaceableObject objectToPlace;

    #region Unity methodss

    private void Awake()
    { 
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>(); //
    }

    private void Update()
    {
        //Todo write the method when push de botton.
        if(Input.GetKeyDown(KeyCode.A))
        {
            InitializeWithObject(prefab1);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            InitializeWithObject(prefab2);
        }
        if (!objectToPlace)
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Create a ray with mouse position
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) //Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene.
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
        //Tomas la posicion en el mundo con numeros decimales y la convertis en discreta.
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        //es el centro de la celda correspondiente a la posición cellPos en el mundo,
        //y este valor se almacena nuevamente en la variable position
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
            Vector3Int pos = new Vector3Int(v.x, 0, v.z); //***PORQUE X, Y SI EL MAPA ES v.x
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }
    
    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab) //A method to inicialize a game object
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero); //The place were it will apear the first time

        GameObject obj = Instantiate(prefab, position, Quaternion.identity); //Inicialize in the position
        objectToPlace = obj.GetComponent<PlaceableObject>(); //Get component (PlaceableObject). I understand that every game object we want to instanciate will have this placebleObject component
        obj.AddComponent<ObjectDrag>(); //When a gameobject is instantiate we give the property to be an ObjectDrag
    }

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeableObject.Size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        TileBase[] baseArray = GetTilesBlock(area, mainTilemap);

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
        mainTilemap.BoxFill(start, whiteTile, start.x, start.y,
            start.x + size.x, start.y + size.y);
    }
    
    #endregion
}
