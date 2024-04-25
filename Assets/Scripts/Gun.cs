using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gun : MonoBehaviour
{
    public enum State
    {
        Ready, // �߻� �غ��
        Empty, // źâ�� ��
    }
    public State state { get; private set; } // ���� ���� ����

    public Transform fireTransform; // �Ѿ��� �߻�� ��ġ
    public ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    public AudioClip shotClip;
    private LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������
    private AudioSource gunAudioPlayer; // �� �Ҹ� �����
    private float fireDistance = 50f; // �����Ÿ�
    private float lastFireTime; // ���� ���������� �߻��� ����
    private float timeBetFire = 0.1f;
    private float damage = 20f;

    private void Awake()
    {
        // ����� ������Ʈ���� ������ ��������
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
        // ���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        muzzleFlashEffect.Play();

        gunAudioPlayer.PlayOneShot(shotClip);

        // 0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }
}
