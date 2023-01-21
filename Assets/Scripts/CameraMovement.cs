using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Min(0)] private float _scrollSpeed;
    [SerializeField, Min(0)] private float _mouseSensitivity;
    [SerializeField, Min(0)] private float _maxZoom;
    [SerializeField, Min(0)] private float _minZoom;

    private Camera _camera;

    private Vector3 _beginCamPos;
    private Vector3 _defaultCamPos;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _defaultCamPos = transform.position;
    }

    private void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
            _camera.fieldOfView = 
                Mathf.Clamp(_camera.fieldOfView +
                scrollDelta * _scrollSpeed,
                _minZoom, _maxZoom);

        if (Input.GetMouseButtonDown(0))
            _beginCamPos = GetWorldMousePos();

        if (Input.GetMouseButton(0))
        {
            Vector3 moveDirection = _beginCamPos - GetWorldMousePos();
            Vector3 rotX = Vector3.right * _scrollSpeed;
            Vector3 rotY = Vector3.up * _scrollSpeed;
            transform.position = Vector3.zero;
            transform.Rotate(rotX, moveDirection.y * 180);
            transform.Rotate(rotY, -moveDirection.x * 180, Space.World);
            transform.Translate(_defaultCamPos);
            _beginCamPos = GetWorldMousePos();
        }
    }

    private Vector3 GetWorldMousePos()
    {
        return _camera.ScreenToViewportPoint(Input.mousePosition);
    }
}
