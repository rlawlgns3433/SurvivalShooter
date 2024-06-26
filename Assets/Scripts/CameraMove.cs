using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    private Vector3 offset = Vector3.zero;
    private float speed = 15f;

    private void Start()
    {
        offset = target.position - transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position - offset, speed * Time.deltaTime);
    }
}
