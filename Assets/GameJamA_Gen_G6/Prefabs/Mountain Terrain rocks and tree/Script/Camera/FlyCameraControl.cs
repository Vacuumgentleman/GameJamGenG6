using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCameraControl : MonoBehaviour
{
    public float rotationSpeed = 300.0f;
    public float moveSpeed = 5.0f; // The speed of the movement
    public float speedAdjustmentFactor = 100.0f; // The factor by which the speed is adjusted
    public float smoothSpeed = 0.01f; // The speed of the smooth movement

    private float x = 0.0f;
    private float y = 0.0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }

        targetRotation = Quaternion.Euler(y, x, 0);

        // Adjust the speed of the camera movement with the scroll wheel
        moveSpeed += Input.GetAxis("Mouse ScrollWheel") * speedAdjustmentFactor;

        // Add these lines to make the camera fly around
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = transform.TransformDirection(movement); // Transform the movement vector to world space
        targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }

        targetRotation = Quaternion.Euler(y, x, 0);

        // Adjust the speed of the camera movement with the scroll wheel
        moveSpeed += Input.GetAxis("Mouse ScrollWheel") * speedAdjustmentFactor;

        // Add these lines to make the camera fly around
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        movement = transform.TransformDirection(movement); // Transform the movement vector to world space
        targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;

        // Interpolate the current rotation and position to the target rotation and position
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

}
