using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const float MOVE_RANGE = 7.5f;

    [SerializeField] float moveSpeed = 5f;
    int laneIndex;
    Vector2 direction;

    public Rigidbody rb;
    public PlayerInputActions controls;
    private InputAction move;

    void Awake()
    {
        controls = new PlayerInputActions();
    }

    void Start()
    {
        laneIndex = 0;
    }

    void Update()
    {
        direction = move.ReadValue<Vector2>();
        if (direction.Equals(Vector2.left) && laneIndex != -1)
        {
            transform.Translate(-MOVE_RANGE, 0, 0);
            laneIndex--;
        }
        else if (direction.Equals(Vector2.right) && laneIndex != 1)
        {
            transform.Translate(MOVE_RANGE, 0, 0);
            laneIndex++;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.forward * moveSpeed;
    }

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }
}
