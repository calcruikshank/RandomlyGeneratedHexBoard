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
    Vector3Int currentCellPosition;
    [SerializeField] Transform testPrefabToSpawn;

    [SerializeField] LayerMask creatureMask;
    // Start is called before the first frame update
    void Start()
    {
        mousePositionScript = GetComponent<MousePositionScript>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = mousePositionScript.GetMousePositionWorldPoint();
        currentCellPosition = grid.WorldToCell(mousePosition);

        if (currentCellPosition != previousCellPosition)
        {
            highlightMap.SetTile(previousCellPosition, null);
            highlightMap.SetTile(currentCellPosition, highlightTile);
            previousCellPosition = currentCellPosition;
            //Debug.Log(baseMap.GetInstantiatedObject(currentCellPosition));
        }
        if (Input.GetMouseButtonDown(0))
        {
            #region creatureSelected
            if (creatureSelected != null)
            {
                //determine if 
                Vector3 positionToTarget = GetWorldPositionOfCell();
                creatureSelected.SetMove(positionToTarget);
                creatureSelected = null;
                return;
            }
            //make sure to shoot a raycast to check to see if it hits a creature first
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, creatureMask))
            {
                if (raycastHit.transform.GetComponent<Creature>() != null)
                {
                    creatureSelected = raycastHit.transform.GetComponent<Creature>();
                    return;
                }
            }
            /*if (BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition) != null) 
            {
                creatureSelected = BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition);
                Debug.Log(creatureSelected);
                return;
            }*/
            #endregion

            #region spawningObjects
            Vector3 positionToSpawn = GetWorldPositionOfCell();
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
            #endregion
        }
    }

    Vector3 GetWorldPositionOfCell()
    {
        Vector3 worldPositionOfCell = highlightMap.GetCellCenterWorld(currentCellPosition);
        if (baseMap.GetInstantiatedObject(currentCellPosition))
        {
            worldPositionOfCell = new Vector3(worldPositionOfCell.x, worldPositionOfCell.y + baseMap.GetInstantiatedObject(currentCellPosition).gameObject.GetComponent<Collider>().bounds.size.y, worldPositionOfCell.z);
        }
        return worldPositionOfCell;
    }
}
