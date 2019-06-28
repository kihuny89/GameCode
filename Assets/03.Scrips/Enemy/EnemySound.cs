using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{

    AudioSource gameAudio;
    public AudioClip attack;
    public AudioClip idle;
    public AudioClip walk;
    public AudioClip die;

    void Awake()
    {
        gameAudio = GetComponent<AudioSource>();   
    }

    public void AttackMonster()
    {
        gameAudio.PlayOneShot(attack);
    }

    public void WalkMonster()
    {
        gameAudio.PlayOneShot(walk);
    }
    
    public void IdleMonster()
    {
        gameAudio.PlayOneShot(idle);
    }

    public void DieMonster()
    {
        gameAudio.PlayOneShot(die);
    }
}
