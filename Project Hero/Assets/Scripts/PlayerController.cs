using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const float GRAVITY = -9.81f;
    const float MOVE_RANGE = 7.5f;
    const float ONE = 1f;

    [SerializeField] float moveSpeed = 250f;
    [SerializeField] float jumpStrength = 500f;
    [SerializeField] float strafeCooldown = 0.33f;
    Vector3 gravity;
    Vector2 initStrafe;
    int laneIndex;
    float initJump;

    public Rigidbody rb;
    public PlayerInputActions controls;
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
                state = State.Grounded;
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
        rb.velocity = (Vector3.forward + ApplyGravititationalAcceleration(gravity) * Time.fixedDeltaTime) * moveSpeed * Time.fixedDeltaTime;
        StartCoroutine(Strafe(initStrafe));
        Jump(initJump);
    }

    IEnumerator Strafe(Vector2 direction)
    {
        if (state.Equals(State.Strafing)) yield break;

        if (direction.Equals(Vector2.left) && (laneIndex != -1))
        {
            // Prevents going left if already in left lane
            if (state.Equals(State.Grounded)) state = State.Strafing;
            transform.Translate(-MOVE_RANGE, 0, 0);
            laneIndex--;
            if (state.Equals(State.Strafing)) state = State.Any;
            yield return new WaitForSeconds(strafeCooldown);
        }
        else if (direction.Equals(Vector2.right) && (laneIndex != 1))
        {
            // Prevents going right if already in right lane
            if (state.Equals(State.Grounded)) state = State.Strafing;
            transform.Translate(MOVE_RANGE, 0, 0);
            laneIndex++;
            if (state.Equals(State.Strafing)) state = State.Any;
            yield return new WaitForSeconds(strafeCooldown);
        }
    }

    void Jump(float initJump)
    {
        if (state.Equals(State.Strafing) || state.Equals(State.Jumping)) return;
        if (initJump.Equals(ONE))
        {
            rb.AddForce(Vector3.up * jumpStrength);
            state = State.Jumping;
        }
    }

    Vector3 ApplyGravititationalAcceleration(Vector3 gravity)
    {
        if (state.Equals(State.Jumping))
        {
            return gravity += gravity;
        } else
        {
            return gravity;
        }
    }
}
