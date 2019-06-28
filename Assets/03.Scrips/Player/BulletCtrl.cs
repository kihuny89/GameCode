using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //총알의 공격력
    public int damage = 20;
    //총알의 속도
    public float speed = 3500f;
    //컴포넌트를 저장
    Transform tr;
    Rigidbody rb;
    TrailRenderer trail;

  
    private void Awake()
    {
        //컴포넌트 할당
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }
    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }
    private void OnDisable()
    {
        //재활용된 총알의 초기화
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }
}
