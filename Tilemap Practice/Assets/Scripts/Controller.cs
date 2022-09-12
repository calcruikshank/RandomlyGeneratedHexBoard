using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    MousePositionScript mousePositionScript;

    Vector3 mousePosition;
    public TileBase highlightTile; 
    public Tilemap highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
    public Tilemap baseMap;
    public Tilemap environmentMap;
    public Grid grid;
    Vector3Int previousCellPosition;

    Creature creatureSelected;

    [SerializeField] Transform testPrefabToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        mousePositionScript = GetComponent<MousePositionScript>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = mousePositionScript.GetMousePositionWorldPoint();
        Vector3Int currentCellPosition = grid.WorldToCell(mousePosition);

        if (currentCellPosition != previousCellPosition)
        {
            highlightMap.SetTile(previousCellPosition, null);
            highlightMap.SetTile(currentCellPosition, highlightTile);
            previousCellPosition = currentCellPosition;
            //Debug.Log(baseMap.GetInstantiatedObject(currentCellPosition));
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition) != null) 
            {
                creatureSelected = BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition);
                Debug.Log(creatureSelected);
                return;
            }
            

            Vector3 positionToSpawn = highlightMap.GetCellCenterWorld(currentCellPosition);
            if (baseMap.GetInstantiatedObject(currentCellPosition))
            {
                positionToSpawn = new Vector3(positionToSpawn.x, positionToSpawn.y + baseMap.GetInstantiatedObject(currentCellPosition).gameObject.GetComponent<Collider>().bounds.size.y, positionToSpawn.z);
            }
            if (environmentMap.GetInstantiatedObject(currentCellPosition))
            {
                GameObject instantiatedObject = environmentMap.GetInstantiatedObject(currentCellPosition);
                if (instantiatedObject.GetComponent<ChangeTransparency>() == null)
                {
                    instantiatedObject.AddComponent<ChangeTransparency>();
                }
                ChangeTransparency instantiatedObjectsChangeTransparency = instantiatedObject.GetComponent<ChangeTransparency>();
                instantiatedObjectsChangeTransparency.ChangeTransparent(100);
                //Material instantiatedObjectMaterial = instantiatedObjectRenderer.material;
                //Material newMaterial = new Material(instantiatedObjectMaterial.shader);
                
                //positionToSpawn = new Vector3(positionToSpawn.x, positionToSpawn.y + environmentMap.GetInstantiatedObject(currentCellPosition).gameObject.GetComponent<Collider>().bounds.size.y, positionToSpawn.z);
            }
            Instantiate(testPrefabToSpawn, positionToSpawn, Quaternion.identity);
        }
    }
}
