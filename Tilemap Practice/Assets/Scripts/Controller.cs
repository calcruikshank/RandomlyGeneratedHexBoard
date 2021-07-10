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
            Debug.Log(baseMap.GetInstantiatedObject(currentCellPosition));
            previousCellPosition = currentCellPosition;
        }
    }
}
