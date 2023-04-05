using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const float ONE = 1f;
    const float GRAVITY = -4.905f;
    const float MOVE_RANGE = 7.5f;
    const float JUMP_SCALE = 100f;
    const float MOVE_SCALE = 10f;

    [SerializeField] float moveSpeed = 200f;
    [SerializeField] float jumpStrength = 100f;
    [SerializeField] float strafeCooldown = 0.33f;
    Vector3 gravity;
    Vector2 initStrafe;
    float initJump;
    int laneIndex;

    public PlayerInputActions controls;
    public Rigidbody rb;
    public State state;

    private InputAction move;
    private InputAction jump;

    public enum State
    {
        Any,
        Strafing,
        Jumping,
        Grounded
    }

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();
        jump = controls.Player.Jump;
        jump.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                // Reset gravity on touching the ground
                state = State.Grounded;
                gravity.Set(0, GRAVITY, 0);
                break;
        }
    }
   
    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                if (!state.Equals(State.Strafing)) state = State.Grounded;
                gravity.Set(0, GRAVITY, 0);
                break;
        }
    }

    void Awake()
    {
        controls = new PlayerInputActions();
        gravity = new Vector3(0, GRAVITY, 0);
    }

    void Start()
    {
        laneIndex = 0;
        initJump = 0;
        state = State.Any;
    }

    void Update()
    {
        initStrafe = move.ReadValue<Vector2>();
        initJump = jump.ReadValue<float>();
    }

    void FixedUpdate()
    {
        rb.velocity = (Vector3.forward + (gravity * Time.fixedDeltaTime)) * (moveSpeed * MOVE_SCALE) * Time.fixedDeltaTime;
        Strafe(initStrafe);
        Jump(initJump);
        if (!state.Equals(State.Grounded) || state.Equals(State.Strafing))
        {
            ApplyGravititationalAcceleration();
        }
    }

    void Strafe(Vector2 initStrafe)
    {
        if (state.Equals(State.Strafing)) return;

        if (initStrafe.Equals(Vector2.left) && (laneIndex != -1))
        {
            // Prevents going left if already in left lane
            transform.Translate(-MOVE_RANGE, 0, 0);
            laneIndex--;
            StartCoroutine(ApplyStrafeCooldown(strafeCooldown));
        }
        else if (initStrafe.Equals(Vector2.right) && (laneIndex != 1))
        {
            // Prevents going right if already in right lane
            transform.Translate(MOVE_RANGE, 0, 0);
            laneIndex++;
            StartCoroutine(ApplyStrafeCooldown(strafeCooldown));
        }
    }

    IEnumerator ApplyStrafeCooldown(float strafeCooldown)
    {
        state = State.Strafing;
        yield return new WaitForSeconds(strafeCooldown);
        state = State.Any;
    }

    void Jump(float initJump)
    {
        if (!state.Equals(State.Grounded)) return;
        if (initJump.Equals(ONE))
        {
            rb.AddForce(Vector3.up * JUMP_SCALE * jumpStrength);
            state = State.Jumping;
        }
    }

    void ApplyGravititationalAcceleration()
    {
        gravity += gravity * 0.25f;
        StartCoroutine(PerSecond());
    }

    IEnumerator PerSecond()
    {
        yield return new WaitForSeconds(ONE);
    }
}
