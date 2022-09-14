using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 5.0f;
    public float cameraZoomSpeed = 500.0f;

    private float tileSize = SceneController.tileSize;
    private int gridLength = SceneController.gridLength;
    private const int ZOOM_MIN = 6;
    private const int ZOOM_MAX = 60;
    //amount by which to adjust x-z movement
    private const int MOVE_ADJUST = 6;
    private const int MIN_SPEED = 1;
    private const int LOG_BASE = 6;

    void FixedUpdate()
    {
        //adjust vert. and hori. movement based on zoom (y position)
        float moveFactor = MIN_SPEED * (MOVE_ADJUST / Mathf.Log(transform.position.y, LOG_BASE));
        float finalSpeed = cameraSpeed * moveFactor;

        //get input from x-z axes
        float deltaX = Input.GetAxis("Horizontal") * finalSpeed;
        float deltaZ = Input.GetAxis("Vertical") * finalSpeed;
        //incorporate y (zoom) movement
        float deltaY = Input.GetAxisRaw("Mouse ScrollWheel") * cameraZoomSpeed;

        //generate movement vector
        Vector3 movementVect = new Vector3(deltaX, deltaY, deltaZ);
        
        //scale movement to be frame-independent
        movementVect *= Time.deltaTime;
        
        //update position vector
        Vector3 positionVect = transform.position;
        positionVect += movementVect;
        transform.position = positionVect;

        //bound camera to only view grid
        if (transform.position.x < 0)
        {
            Vector3 pos = transform.position;
            pos.x = 0;
            transform.position = pos;
        }
        else if (transform.position.x > (tileSize * gridLength))
        {
            Vector3 pos = transform.position;
            pos.x = tileSize * gridLength;
            transform.position = pos;
        }
        if (transform.position.z < 0)
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
        else if (transform.position.z > (tileSize * gridLength))
        {
            Vector3 pos = transform.position;
            pos.z = tileSize * gridLength;
            transform.position = pos;
        }

        //bound camera to reasonable zoom levels
        if (transform.position.y < ZOOM_MIN)
        {
            Vector3 pos = transform.position;
            pos.y = ZOOM_MIN;
            transform.position = pos;
        }
        else if (transform.position.y > ZOOM_MAX)
        {
            Vector3 pos = transform.position;
            pos.y = ZOOM_MAX;
            transform.position = pos;
        }
    }
}
