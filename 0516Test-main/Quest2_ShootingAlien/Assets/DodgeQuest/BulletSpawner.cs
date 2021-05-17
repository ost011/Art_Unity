﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float spwanRateMin = 0.5f;
    public float spwanRateMax = 3f;

    private Transform target;
    private float spwanRate;
    private float timeAffterSpawn;

    //public float hp = 2500.0f;
    //int maxHp = 2500;
    //public Slider hpSlider;

    public AudioClip fireClip;
    AudioSource fireAudio;

    private MonsterCtrl monsterCtrl;

    // Start is called before the first frame update
    void Start()
    {
        timeAffterSpawn = 0f;
        spwanRate = Random.Range(spwanRateMin, spwanRateMax);
        target = FindObjectOfType<PlayerController>().transform;

        fireAudio = GetComponent<AudioSource>();
        monsterCtrl = GetComponent<MonsterCtrl>();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(monsterCtrl.IsDie);
        if(monsterCtrl.IsDie == false)
        {
            timeAffterSpawn += Time.deltaTime;

            if (timeAffterSpawn >= spwanRate)
            {
                timeAffterSpawn = 0f;

                if (!FindObjectOfType<GameManager>().isGameOver &&
                    Vector3.Distance(target.transform.position, transform.position) <= 10000.0f)
                {
                    GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                    bullet.transform.LookAt(target);
                    fireAudio.PlayOneShot(fireClip);
                }

                spwanRate = Random.Range(spwanRateMin, spwanRateMax);
            }
        }
       
        
        //hpSlider.value = (float)hp / (float)maxHp;

    }


    //public void GetDamage(float amount)
    //{
    //    hp -= amount;

    //    if(hp < 0)
    //    {
    //        //Bullet Spawner가 죽음 SetActive를 false로 설정
    //        gameObject.SetActive(false);

    //        //FindObjectOfType<GameManager>().GetScored(1);
    //    }
    //}


}
