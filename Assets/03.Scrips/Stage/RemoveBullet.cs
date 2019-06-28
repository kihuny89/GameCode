using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    //충돌이 시작할 때 발생하는 이벤트
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            //스파크 프리팹을 저장할 변수
            ShowEffect(collision);
            //충돌한 게임오브젝트 삭제
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision coll)
    {
        //충돌지점의 정보추출
        ContactPoint contact = coll.contacts[0];
        //법선 벡터가 이루는 회전각도 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        //스파크 효과 생성
        Instantiate(sparkEffect, contact.point, rot);
    }
}
