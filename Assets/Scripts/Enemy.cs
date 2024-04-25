using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity
{
    public AudioClip deathClip; // ��� �Ҹ�
    public AudioClip hitClip; // �ǰ� �Ҹ�
    public ParticleSystem hitParticle = null;
    public LivingEntity targetEntity = null;
    private NavMeshAgent pathFinder = null; // ��ΰ�� AI ������Ʈ
    public LayerMask targetLayerMask;
    private WaitForSeconds pathFindInterval = new WaitForSeconds(0.25f);
    private AudioSource enemyAudioPlayer = null;
    private Animator enemyAnimator = null;

    private float findTargetRadius = 5f;
    private bool hasTarget
    {
        get
        {
            // ������ ����� �����ϰ�, ����� ������� �ʾҴٸ� true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            // �׷��� �ʴٸ� false
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
