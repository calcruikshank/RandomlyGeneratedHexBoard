using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
{
    MousePositionScript mousePositionScript;

    Vector3 mousePosition;
    public TileBase highlightTile;
    public Tilemap highlightMap;
    public Tilemap baseMap;
    public Grid grid;
    Vector3Int previousCellPosition;


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
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(currentCellPosition);
            Vector3 positionToSpawn = highlightMap.GetCellCenterWorld(currentCellPosition);
            Instantiate(testPrefabToSpawn, positionToSpawn, Quaternion.identity);
        }
    }
}
