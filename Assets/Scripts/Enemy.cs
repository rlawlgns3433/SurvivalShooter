using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyTypes
{
    Bunny,
    Bear,
    Elephant,
    None,
}

public enum EnemyStatus
{ 
    Idle,
    Move,
    Attack,
    Death,
    None,
}


public class Enemy : LivingEntity
{
    public static readonly int TYPE_COUNT = 3;

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public ParticleSystem hitParticle = null;
    public LivingEntity targetEntity = null;
    private NavMeshAgent pathFinder = null; // 경로계산 AI 에이전트
    public LayerMask targetLayerMask;
    private WaitForSeconds pathFindInterval = new WaitForSeconds(0.25f);
    private AudioSource enemyAudioPlayer = null;
    private Animator enemyAnimator = null;
    private Coroutine updatePathCoroutine = null;

    public EnemyTypes type = EnemyTypes.None;
    public EnemyStatus status = EnemyStatus.Idle;

    public float damage = 10f;
    private float attackInterval = 1f;
    private float lastAttackTime;
    private float findTargetRadius = 50f;
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                status = EnemyStatus.Move;
                return true;
            }
            else
            {
                status = EnemyStatus.Idle;
                // 그렇지 않다면 false
                return false;
            }
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
    }

    private void OnEnable()
    {
        dead = false;
        health = startingHealth;

        var colliders = GetComponents<Collider>();
        foreach(var collider in colliders)
        {
            collider.enabled = true;
        }
        
        pathFinder.enabled = true;
        pathFinder.isStopped = false;

        updatePathCoroutine = StartCoroutine(UpdatePath());
    }

    private void Start()
    {
        switch (type)
        {
            case EnemyTypes.Bunny:
                {
                    startingHealth = 80;
                    damage = 10;
                }
                break;

            case EnemyTypes.Bear:
                {
                    startingHealth = 80;
                    damage = 15;
                }
                break;

            case EnemyTypes.Elephant:
                {
                    startingHealth = 200;
                    damage = 25;
                }
                break;
        }
    }

    private void Update()
    {
        enemyAnimator.SetBool("Move", hasTarget);

        switch (status)
        {
            case EnemyStatus.Idle:
                break;
            case EnemyStatus.Move:
                break;
            case EnemyStatus.Attack:
                break;
            case EnemyStatus.Death:
                break;
        }
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
    }

    public override void Die()
    {
        base.Die();
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        StopCoroutine(updatePathCoroutine);
        updatePathCoroutine = null;

        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        targetEntity = null;

        enemyAnimator.SetTrigger("Die");
        enemyAudioPlayer.PlayOneShot(deathClip);
        GameManager.instance.AddScore((int)(startingHealth + damage));

        Invoke("AfterDie", 3f);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        hitParticle.transform.position = hitPoint;
        hitParticle.transform.rotation = Quaternion.LookRotation(hitNormal);
        hitParticle.Play();
        enemyAudioPlayer.PlayOneShot(hitClip);
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public void StartSinking()
    {
        // Code to handle the event
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        // 공격
        if (dead)
            return;

        if (Time.time > lastAttackTime + attackInterval)
        {
            var livingEntity = collision.gameObject.GetComponent<LivingEntity>();
            if (livingEntity != null && !livingEntity.dead && livingEntity == targetEntity)
            {
                var pos = transform.position;
                pos.y += 1f;
                var hitPoint = collision.contacts[0].point;
                var hitNormal = pos - collision.contacts[0].point;
                livingEntity.OnDamage(damage, hitPoint, hitNormal.normalized);
                lastAttackTime = Time.time;
            }
        }
    }

    private void AfterDie()
    {
        gameObject.SetActive(false);
        EnemySpawner.ReturnEnemy(gameObject, type);
    }
}
