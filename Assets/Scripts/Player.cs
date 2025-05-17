using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float verticalClamp = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation;

    [Header("Block Settings")]
    public List<Block> blockPrefabs;
    [HideInInspector]
    public Block activeBlock;

    private Camera mainCamera;
    float breakSeconds;
    Block targetBlock;
    Block breakingBlock;
    RaycastHit targetRaycastHit;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (blockPrefabs != null && blockPrefabs.Count > 0)
        {
            activeBlock = blockPrefabs[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        CheckTargetBlock();
        CheckDrop();
        HandleBlockSelection();

        if (Input.GetButton("Fire1")) { TryBreakBlock(); }

        else { breakSeconds = 0; }

        if (Input.GetButtonDown("Fire2")) { TryPlaceBlock(); }
    }

    void HandleBlockSelection()
    {
        for (int i = 0; i < blockPrefabs.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                activeBlock = blockPrefabs[i];
                Debug.Log($"Selected Block: {activeBlock.name}");
            }
        }
    }

    void TryBreakBlock()
    {
        if (!targetBlock) { breakSeconds = 0; return; }

        if (breakingBlock != targetBlock) { breakSeconds = 0; }

        breakingBlock = targetBlock;

        breakSeconds += Time.deltaTime;

        bool breakSuccess = targetBlock.TryBreak(breakSeconds);

        if (breakSuccess) { breakSeconds = 0; }
    }

    void TryPlaceBlock()
    {
        if(!targetBlock) { return; }

        GameObject block = Instantiate(activeBlock.gameObject);

        block.transform.position = targetBlock.transform.position;

        Vector3 incomingVector = targetRaycastHit.normal - Vector3.up;
        // South
        if (incomingVector == new Vector3(0, -1, -1))
        {
            block.transform.position += new Vector3(0, 0, -1);
        }

        // North
        if (incomingVector == new Vector3(0, -1, 1))
        {
            block.transform.position += new Vector3(0, 0, 1);
        }

        // Up
        if (incomingVector == new Vector3(0, 0, 0))
        {
            block.transform.position += new Vector3(0, 1, 0);
        }

        // Down
        if (incomingVector == new Vector3(0, -2, 0))
        {
            block.transform.position += new Vector3(0, -1, 0);
        }

        // West
        if (incomingVector == new Vector3(-1, -1, 0))
        {
            block.transform.position += new Vector3(-1, 0, 0);
        }

        // East
        if (incomingVector == new Vector3(1, -1, 0))
        {
            block.transform.position += new Vector3(1, 0, 0);
        }
    }

    void CheckTargetBlock()
    {

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {

            Transform objectHit = hit.transform;

            targetRaycastHit = hit;

            if (!objectHit.GetComponent<Block>()) { return; }

            targetBlock = objectHit.GetComponent<Block>();
        }

    }

    private void CheckDrop()
    {
        if (this.transform.position.y < -10)
        {
            // Reset player position if they fall off the map
            this.transform.position = new Vector3(0, 2, 3);
            velocity.y = 0;
        }
    }

    void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
