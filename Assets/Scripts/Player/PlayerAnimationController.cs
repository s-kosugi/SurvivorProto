using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Vector2 lastMoveDir = Vector2.right;

    // フォーム状態を外部から変えたい場合などに使うプロパティ
    public bool IsDarkForm { get; set; }

    public void UpdateAnimation(Vector2 moveInput)
    {
        float moveSpeed = moveInput.magnitude;
        animator.SetFloat("MoveSpeed", moveSpeed);

        // 光/闇のフォーム切り替え
        animator.SetBool("IsDarkForm", IsDarkForm);

        // 左右判定（左右のみ）
        if (moveInput.x > 0.01f)
            lastMoveDir = Vector2.right;
        else if (moveInput.x < -0.01f)
            lastMoveDir = Vector2.left;

        animator.SetBool("IsFacingLeft", lastMoveDir.x < 0);
    }
}
