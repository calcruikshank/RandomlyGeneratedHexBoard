using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionScript : MonoBehaviour
{
    Camera mainCamera;
    Vector3 mousePositionWorldPoint;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mousePositionWorldPoint = raycastHit.point;
        }
    }

    public Vector3 GetMousePositionWorldPoint()
    {
        return mousePositionWorldPoint;
    }
}
