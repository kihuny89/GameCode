using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//총알 발사와 재장전 오디오 클립을 저장할 구조체
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
    public AudioClip hit;
    public AudioClip footsteps;
    public AudioClip jumpsteps;
    public AudioClip jumpstate;
}

public class FireCtrl : MonoBehaviour
{
    //무기타입
    public enum WeaponType 
    {
        RIFLE = 0, SHOTGUN = 1
    }


    //주인공이 현재 들고 있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;
    //총알 프리팹
   // public GameObject bullet;
    //총알 발사 좌표
    public Transform firePos;
    //탄피 추출 파티클
    public ParticleSystem cartridge;
    //AudioSource 컴포넌트를 저장할 변수
    AudioSource _audio;

    //총구 화염 파티클
    ParticleSystem muzzleFlash;
    //오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;

    //발사 여부 판단
    bool isFire = false;
    //발사 간격
    float fireRate = 0.12f;
    //장전
    public bool reloading = false;
    //남은 총알
    int remainingBullet=30;
    //최대 총알
    int maxBullet = 30;
    //다음 발사 시간 저장
    float nextFire;

    //남은 총알수
    public Text magzineText;
    //재장전 시간
    public float reloadTime = 1.5f;


    //정조준
    bool isFineSightMode = false;

    //본래 포지션 값
    Vector3 originPos;

    PlayerCtrl ctrl;
    Damage damage;
    WeaponManager weapon;

    public GameObject bullet;
    public GameObject shotbullet;

    WeaponSway weaponSway;

    void shot()
    {
        if (weapon.index == 0)
        {
            currWeapon = WeaponType.RIFLE;
            
        }
        else if (weapon.index == 1)
        {
            currWeapon = WeaponType.SHOTGUN;
            //Instantiate<GameObject>(bullet, objectPools.transform);
        }
    }

   // Shack shake;
    void Start()
    {
        //FirePos 하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        //AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();
        ctrl = GetComponent<PlayerCtrl>();
        damage = GetComponent<Damage>();
        weapon = GetComponentInChildren<WeaponManager>();
        weaponSway = GetComponentInChildren<WeaponSway>();
    }


    void Update()
    {
        bool fire = currWeapon == WeaponType.RIFLE ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if ((!reloading && fire) && !ctrl.isRunning)
        {           
                if (Time.time>nextFire)
                {
                    --remainingBullet;
                    if(damage.currHp > 0)
                    {
                        Fire();
                    }
                    //남은 총알이 없을 경우 재장전
                    if (remainingBullet == 0)
                    {
                        StartCoroutine(Reloading());
                    }
                    //다음 총알 발사시간
                    nextFire = Time.time + fireRate;
                }            
        }

        else if (remainingBullet< maxBullet && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reloading());
        }        
        weaponSway.RecoilBack();
        shot();
        weaponSway.AimDownSights();
    }

    public IEnumerator Reloading()
    {
        reloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);
        //재장전 오디오의 길이
        GetComponent<PlayerCtrl>().Playergunreload();
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 1f);
        //각종 변수값의 초기화
        reloading = false;       
        remainingBullet = maxBullet;
        //남은 총알수 갱신
        UpdateBulletText();
    }

    public void UpdateBulletText()
    {
        //(남은 총알 수/ 최대 총알수) 
        magzineText.text = string.Format("<color=#ff000>{0}</color>/{1}", remainingBullet, maxBullet);
    }


     void Fire()
    {       
        //Bullet 프리팹
        if(currWeapon == WeaponType.RIFLE)
        {
            Instantiate(bullet, firePos.position, firePos.rotation);
        }
        else if(currWeapon == WeaponType.SHOTGUN)
        {
            Instantiate(shotbullet, firePos.position, firePos.rotation);
        }

        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        cartridge.Play();
        muzzleFlash.Play();
        FireSfx();
        //갱신
        UpdateBulletText();
        weaponSway.Recoil();
        if (!Input.GetButton("Fire2"))
        {
            ctrl.Fireshot();
        }
        else if (Input.GetButton("Fire2"))
        {
            //ctrl.Fireshoot();
        }
    }

   
    //적한테 공격 당했을 때 실행할 함수 
    public void HitDam()
    {
        AudioClip _sfx = playerSfx.hit;
        _audio.PlayOneShot(_sfx, 1f);
    }

    public void FootStep()
    {
        AudioClip _sfx = playerSfx.footsteps;
        _audio.PlayOneShot(_sfx, 1f);
    }
    public void JumpStep()
    {
        AudioClip _sfx = playerSfx.jumpsteps;
        _audio.PlayOneShot(_sfx, 1f);
    }
    public void JumpState()
    {
        AudioClip _sfx = playerSfx.jumpstate;
        _audio.PlayOneShot(_sfx, 1f);
    } 
    void FireSfx()
    {
        //현재 들고 있는 무기의 오디오 클립을 가져옴
        AudioClip _sfx = playerSfx.fire[(int)currWeapon];
        //사운드 발생
        _audio.PlayOneShot(_sfx, 1f);
    }


}
