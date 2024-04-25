using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
    }
    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public AudioClip shotClip;
    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러
    private AudioSource gunAudioPlayer; // 총 소리 재생기
    private float fireDistance = 50f; // 사정거리
    private float lastFireTime; // 총을 마지막으로 발사한 시점
    private float timeBetFire = 0.1f;
    private float damage = 20f;

    private void Awake()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        if (!TryGetComponent(out bulletLineRenderer))
        {
            return;
        }

        if (!TryGetComponent(out gunAudioPlayer))
        {
            gunAudioPlayer.enabled = false;
            return;
        }

        bulletLineRenderer.enabled = false;
        bulletLineRenderer.positionCount = 2;
    }

    public void Fire()
    {
        if (state == State.Ready && Time.time > lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }

    private void Shot()
    {
        var hitPoint = Vector3.zero;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, fireDistance))
        {
            hitPoint = hitInfo.point;
            var damagable = hitInfo.collider.GetComponent<IDamageable>();
            damagable?.OnDamage(damage, hitPoint, hitInfo.normal);
        }
        else
        {
            hitPoint = fireTransform.position + fireTransform.forward * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPoint));

    }
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        // 라인 렌더러를 활성화하여 총알 궤적을 그린다
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        muzzleFlashEffect.Play();

        gunAudioPlayer.PlayOneShot(shotClip);

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }
}
