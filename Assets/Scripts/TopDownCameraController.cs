using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    public float rotationAngle = 45f;   // 카메라 좌우 회전 각도
    public float moveSpeed = 10f;       // 카메라 WASD 이동 속도

    public float edgeMoveSpeed = 10f;   // 마우스가장자리 카메라 이동 속도
    public int edgeSize = 10;           // 이동판정 화면가장자리 기준 두께

    private void Start()
    {
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleRotation();
        HandleZoom();

        //HandleEdgeMovement();
    }

    void HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A, D
        float v = Input.GetAxisRaw("Vertical");   // W, S

        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // 현재 카메라의 방향 기준으로 이동
        Vector3 worldMove = transform.TransformDirection(moveDir);
        worldMove.y = 0f; // Y축 고정

        transform.position += worldMove * moveSpeed * Time.deltaTime;
    }

    void HandleRotation() // Q, E로 카메라 45도씩 회전 (XCOM 스타일)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(rotationAngle);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(-rotationAngle);
        }
    }

    void RotateCamera(float angle)
    {
        transform.RotateAround(transform.position, Vector3.up, angle);
    }

    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minHeight = 10f;
    [SerializeField] private float maxHeight = 100f;

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 zoomDirection = transform.forward;

            // 확대/축소는 Y축 위치로만 제한
            float newY = transform.position.y + -scroll * zoomSpeed * Time.deltaTime;

            if (newY >= minHeight && newY <= maxHeight)
            {
                transform.position += zoomDirection * scroll * zoomSpeed * Time.deltaTime;
            }
        }
    }


    void HandleEdgeMovement() // 마우스커서가 화면 가장자리로 가면 그 방향으로 카메라 이동
    {
        Vector3 move = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < edgeSize) move.x = -1;
        else if (mousePos.x > Screen.width - edgeSize) move.x = 1;

        if (mousePos.y < edgeSize) move.z = -1;
        else if (mousePos.y > Screen.height - edgeSize) move.z = 1;

        Vector3 worldMove = transform.TransformDirection(move.normalized);
        worldMove.y = 0f;

        transform.position += worldMove * edgeMoveSpeed * Time.deltaTime;
    }
}
