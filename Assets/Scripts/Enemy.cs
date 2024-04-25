using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public ParticleSystem hitParticle = null;
    public LivingEntity targetEntity = null;
    private NavMeshAgent pathFinder = null; // 경로계산 AI 에이전트
    public LayerMask targetLayerMask;
    private WaitForSeconds pathFindInterval = new WaitForSeconds(0.25f);
    private AudioSource enemyAudioPlayer = null;
    private Animator enemyAnimator = null;

    private float findTargetRadius = 5f;
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake()
    {
        if (!TryGetComponent(out enemyAudioPlayer))
        {
            enemyAudioPlayer.enabled = false;
            return;
        }

        if (!TryGetComponent(out enemyAnimator))
        {
            enemyAnimator.enabled = false;
            return;
        }

        if (!TryGetComponent(out pathFinder))
        {
            pathFinder.enabled = false;
            return;
        }

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        enemyAnimator.SetBool("Move", hasTarget);
    }

    IEnumerator UpdatePath()
    {
        while(!dead)
        {
            if(hasTarget)
            {
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                pathFinder.isStopped = true;
                var targets = Physics.OverlapSphere(transform.position, findTargetRadius, targetLayerMask);

                foreach(var collider in targets)
                {
                    LivingEntity livingEntity = collider.GetComponent<LivingEntity>();

                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }

            yield return pathFindInterval;
        }

        if(dead)
        {
            Destroy(gameObject, 3f);
        }
    }

    public override void Die()
    {
        base.Die();
        enemyAnimator.SetTrigger("Die");
        enemyAudioPlayer.PlayOneShot(deathClip);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        hitParticle.transform.position = hitPoint;
        hitParticle.transform.rotation = Quaternion.LookRotation(hitNormal);
        hitParticle.Play();
        enemyAudioPlayer.PlayOneShot(hitClip);
        base.OnDamage(damage, hitPoint, hitNormal);
    }
}
