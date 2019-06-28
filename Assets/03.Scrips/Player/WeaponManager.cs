using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 변경하고 다시 변경이 가능할 때까지 딜레이
    public float switchDelay = 1f;
    //무기의 게임 오브젝트 배열
    public GameObject[] weapon;


    //무기의 인덱스
    public int index = 0;
    //딜레이
    private bool isSwitching = false;

    FireCtrl firectrl;


    private void Start()
    {
        InitializeWeapon();
        firectrl = GetComponentInParent<FireCtrl>();
    }
    

    private void Update()
    {
        //마우스 스크롤 휠이 내려가고 딜레이가 아니면 인덱스를 올린다.
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
        {
            index++;
            if (index >= weapon.Length)
                index = 0;
            StartCoroutine(SwitchDelay(index));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
        {
            index--;
            if (index < 0)
                index = weapon.Length - 1;
            StartCoroutine(SwitchDelay(index));
        }

        //키보드 위쪽의 1~9까지의 키를 입력받아 각각에 해당하는 인덱스로 지정한다.
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKeyDown((KeyCode)i) && !isSwitching && weapon.Length > i - 49 && index != i - 49)
            {
                index = i - 49;
                StartCoroutine(SwitchDelay(index));
            }
        }
    }

    //게임이 시작될때 초기화하는 부분
    private void InitializeWeapon()
    {
        //0번 인덱스의무기만 가져온다.
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(false);
        }
        weapon[0].SetActive(true);
        index = 0;
    }

    //
    public IEnumerator SwitchDelay(int newIndex)
    {
        isSwitching = true;
        SwitchWeapons(newIndex);
        StartCoroutine(firectrl.Reloading());
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;
    }

    //입력받은 인덱스의 오브젝트를 활성화하고 나머지는 비활성화한다.
    private void SwitchWeapons(int newIndex)
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(false);
        }
        weapon[newIndex].SetActive(true);
    }

}
