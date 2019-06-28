using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BosDamage : MonoBehaviour
{
    const string attackTag = "BULLET";
    //생명게이지
    float hp = 5000;
    //피격시 사용할 혈흔 효과
    GameObject bloodEffect;

    void Start()
    {
        //혈흔 효과 프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == attackTag)
        {
            //혈흔효과 생성하는 함수
            ShowBloodEffect(collision);
            //총알삭제
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

            //생명게이지 차감
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0f)
            {
                //적 캐릭터의 상태를 Die로 만듬
                GetComponent<BosAI>().state = BosAI.State.DIE;
                GameManager.instance.EndingScene();
                //SceneManager.LoadScene("Win");
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //공격 지점
        Vector3 pos = coll.contacts[0].point;
        //공격이 성공 했을때
        Vector3 _normal = coll.contacts[0].normal;
        //공격 성공시 회전값
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //혈흔효과 
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
