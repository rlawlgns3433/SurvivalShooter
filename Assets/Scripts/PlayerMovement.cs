using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    private PlayerInput playerInput = null;
    private Rigidbody playerRigidbody = null;
    private Animator playerAnimator = null;

    private void Awake()
    {
        if(!TryGetComponent(out playerInput))
        {
            Debug.LogError("PlayerInput ");
            playerInput.enabled = false;
            return;
        }
        if (!TryGetComponent(out playerRigidbody))
        {
            Debug.LogError("playerRigidbody ");
            return;
        }
        if (!TryGetComponent(out playerAnimator))
        {
            Debug.LogError("playerAnimator");
            playerAnimator.enabled = false;
            return;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    private void Update()
    {
        playerAnimator.SetFloat("MoveX", playerInput.moveX);
        playerAnimator.SetFloat("MoveZ", playerInput.moveZ);

        if (playerInput.moveX != 0 || playerInput.moveZ != 0)
        {
            playerAnimator.SetBool("Move", true);
        }
        else
        {
            playerAnimator.SetBool("Move", false);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Rotate();
        Move();
    }

    private void Move()
    {
        var pos = playerRigidbody.position;
        pos.x += playerInput.moveX * moveSpeed * Time.deltaTime;
        pos.z += playerInput.moveZ * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(pos);
    }

    private void Rotate()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (GroupPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            var dir = pointTolook - transform.position;
            dir.y = 0;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10);
        }
    }
}
