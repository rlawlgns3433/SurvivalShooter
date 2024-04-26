using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerInput playerInput;
    private PlayerShooter playerShooter;

    private void Awake()
    {
        if (!TryGetComponent(out playerAudioPlayer))
        {
            playerAudioPlayer.enabled = false;
            return;
        }
        if (!TryGetComponent(out playerAnimator))
        {
            playerAnimator.enabled = false;
            return;
        }
        if (!TryGetComponent(out playerMovement))
        {
            playerMovement.enabled = false;
            return;
        }
        if (!TryGetComponent(out playerInput))
        {
            playerInput.enabled = false;
            return;
        }
        if (!TryGetComponent(out playerShooter))
        {
            playerShooter.enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Die();
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health / startingHealth;
        playerAudioPlayer.PlayOneShot(hitClip);
    }

    public override void Die()
    {
        base.Die();
        playerAnimator.SetTrigger("Die");
        playerAudioPlayer.PlayOneShot(deathClip);
        playerMovement.enabled = false;
        playerInput.enabled = false;
        playerShooter.enabled = false;
    }

    private void RestartLevel()
    {
        playerAnimator.SetTrigger("Respawn");
        dead = false;
        health = startingHealth;
        healthSlider.value = health / startingHealth;
        playerMovement.enabled = true;
        playerInput.enabled = true;
        playerShooter.enabled = true;

        Vector3 randomPos = Random.insideUnitSphere * 20f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 20f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }
}