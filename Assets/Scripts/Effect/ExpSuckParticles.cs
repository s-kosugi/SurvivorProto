using UnityEngine;

public class ExpSuckParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    private Transform playerTarget;

    [Header("Movement Settings")]
    [SerializeField] private float suckSpeed = 5f;

    // ▼ Inspector 調整可能な「最低生存時間」
    [SerializeField] private float minAliveTime = 0.15f;

    private ParticleSystem.Particle[] particles;

    private float[] prevDistances;
    private float[] elapsedTimes;

    private void Awake()
    {
        particles = new ParticleSystem.Particle[64];
        prevDistances = new float[64];
        elapsedTimes = new float[64];
    }

    private void LateUpdate()
    {
        // ゲームがプレイ状態でなければできない
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;
        // プレイヤー再取得
        if (playerTarget == null)
        {
            if (PlayerManager.Instance != null &&
                PlayerManager.Instance.MainPlayer != null)
            {
                playerTarget = PlayerManager.Instance.MainPlayer.transform;
            }
        }

        if (playerTarget == null)
            return;

        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 particleWorldPos = transform.TransformPoint(particles[i].position);
            Vector3 dir = (playerTarget.position - particleWorldPos).normalized;

            float currentDist = Vector3.Distance(particleWorldPos, playerTarget.position);

            // ▼ 経過時間（Inspector 調整可能!）
            elapsedTimes[i] += Time.deltaTime;

            // ▼ 初回距離記録のみ
            if (prevDistances[i] == 0f)
            {
                prevDistances[i] = currentDist;
            }

            // ▼ 吸い込み
            particles[i].velocity += dir * suckSpeed * Time.deltaTime;

            // ▼ 指定時間経過後（minAliveTime）に追い越し判定
            if (elapsedTimes[i] > minAliveTime)
            {
                if (currentDist > prevDistances[i])
                {
                    particles[i].remainingLifetime = -1f;
                }
            }

            // ▼ 距離更新
            prevDistances[i] = currentDist;
        }

        ps.SetParticles(particles, count);
    }

    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
}
