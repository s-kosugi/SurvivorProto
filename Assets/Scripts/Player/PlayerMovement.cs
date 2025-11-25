using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVisuals playerVisuals;
    [SerializeField] private PlayerController controller;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lastMoveDir = Vector2.right;

    public bool IsFacingLeft => lastMoveDir.x < 0;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        if (!controller.CanMove())
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
        }

        playerVisuals.UpdateMoveAnimation(moveInput.magnitude);

        // 向きの更新
        if (moveInput.x > 0.01f) lastMoveDir = Vector2.right;
        else if (moveInput.x < -0.01f) lastMoveDir = Vector2.left;

        playerVisuals.UpdateFacing(IsFacingLeft);
    }

    void FixedUpdate()
    {
        if (!controller.CanMove()) return;

        rb.MovePosition(rb.position + moveInput * controller.CurrentMoveSpeed * Time.fixedDeltaTime);
    }
    /// <summary>
    /// 移動停止
    /// </summary>
    public void MoveStop()
    {
        rb.velocity = Vector2.zero;
    }
}
