using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float mouseSensitivity;

    [SerializeField] private float minVerticalAngle;
    [SerializeField] private float maxVerticalAngle;

    [SerializeField] private float zoomSpeed;

    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void LateUpdate()
    {
        transform.position = player.position + offset;

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationX -= mouseY;
            rotationY += mouseX;

            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            transform.position = player.position + rotation * offset;

            transform.LookAt(player.position);
        }
        else
        {
            transform.position = player.position + Quaternion.Euler(rotationX, rotationY, 0) * offset;
            transform.LookAt(player.position);
        }

        HandleZoom();
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        float newOffsetMagnitude = offset.magnitude - scrollInput * zoomSpeed;
        
        newOffsetMagnitude = Mathf.Clamp(newOffsetMagnitude, minZoom, maxZoom);

        offset = offset.normalized * newOffsetMagnitude;
    }
}