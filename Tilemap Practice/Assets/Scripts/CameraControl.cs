using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float minYForCamera = 8f;
    [SerializeField] float maxYForCamera = 20f;

    public float speed =2000;
    float mouseSensitivity = 3.0f;
    private Vector3 lastPosition;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
        Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
            MoveCamera(Input.GetAxis("Mouse ScrollWheel"));
        }
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {

            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(-delta.x * mouseSensitivity * Time.deltaTime, -delta.y * mouseSensitivity * Time.deltaTime, 0) ;
            lastPosition = Input.mousePosition;
        }
    }
    void MoveCamera(float y)
    {
        if (transform.position.y > maxYForCamera && y < 0) return;
        if (transform.position.y < minYForCamera && y > 0) return;
        Vector3 movementAmount = new Vector3(transform.position.x, transform.position.y - (y * speed * Time.deltaTime), transform.position.z);
        if (movementAmount.y > maxYForCamera)
        {
            movementAmount = new Vector3(transform.position.x, maxYForCamera, transform.position.z);
        }
        if (movementAmount.y < minYForCamera)
        {
            movementAmount = new Vector3(transform.position.x, minYForCamera, transform.position.z);
        }
        transform.position = movementAmount;
    }
}
