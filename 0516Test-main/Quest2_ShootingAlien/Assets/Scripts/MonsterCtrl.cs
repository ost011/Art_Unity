﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterCtrl : MonoBehaviour {
    //몬스터의 상태 정보가 있는 Enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die };
    //몬스터의 현재 상태 정보를 저장할 Enum변수
    public MonsterState monsterState = MonsterState.idle;

    //속도 향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform monsterTr;
    private Transform playerTr;
    private UnityEngine.AI.NavMeshAgent nvAgent;
    private Animator animator;
    public float hp = 800.0f;
    int maxHp = 800;
    public Slider hpSlider;
    //public GameObject gameoverTxt;
    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 2.01f;
    
    //몬스터의 사망 여부
    private bool isDie = false;
    
    public bool IsDie
    {
        get
        {
            return isDie;
        }
    }
      
    //몬스터 생명 변수
    //private int hp = 100;
    
  
    
    void Start () {
        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상인 Player의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent 컴포넌트 할당
        nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponent<Animator>();
        //추적 대상의 위치를 설정하면 바로 추적 시작
        //nvAgent.destination = playerTr.position;
        //nvAgent.isStopped = false;
        //nvAgent.destination = playerTr.position;
    }


    
    
    void Update()
    {
        hpSlider.value = (float)hp / (float)maxHp;
        //일정한 간격으로 몬스터의 행동 상태를 체크하는 코루틴 함수 실행
        StartCoroutine(this.CheckMonsterState());

        //몬스터의 상태에 따라 동작하는 루틴을 실행하는 코루틴 함수 실행
        StartCoroutine(this.MonsterAction());        
    }
    
       
    //일정한 간격으로 몬스터의 행동 상태를 체크하고 monsterState값 변경
    IEnumerator CheckMonsterState()
    {
        while(!isDie)
        {
            //0.2초 동안 기다렸다가 다음으로 넘어감
            yield return new WaitForSeconds(0.2f);
            
            //몬스터와 플레이어 사이의 거리 측정
            float dist = Vector3.Distance(playerTr.position , monsterTr.position);
            
            if (dist <= attackDist && !FindObjectOfType<GameManager>().isGameOver) //공격거리 범위 이내로 들어왔는지 확인
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist) //추적거리 범위 이내로 들어왔는지 확인
            {   
                monsterState = MonsterState.trace; //몬스터의 상태를 추적으로 설정
            }
            else
            {
                monsterState = MonsterState.idle; //몬스터의 상태를 idle모드로 설정
            }
        }
    }
    
    //몬스터의 상태값에 따라 적절한 동작을 수행하는 함수
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState){
                //idle 상태
                case MonsterState.idle:
                    //추적 중지
                    nvAgent.isStopped = true;
                    //Animator의 IsTrace 변수를 false로 설정
                    animator.SetBool("IsTrace", false);
                    break;
                
                //추적 상태
                case MonsterState.trace:
                    //추적 대상의 위치를 넘겨줌
                    nvAgent.destination = playerTr.position;
                    //추적을 재시작
                    nvAgent.isStopped = false;
                    
                    //Animator의 IsAttack 변수를 false로 설정
                    animator.SetBool("IsAttack", false);                    
                    //Animator의 IsTrace 변수값을 true로 설정
                    animator.SetBool("IsTrace", true);
                    break;
                
                //공격 상태
                case MonsterState.attack:
                    //추적 중지
                    nvAgent.isStopped = true;
                    //IsAttack을 true로 설정해 attack State로 전이
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }
    
    
    public void GetDamage(float amounnt)
    {
        if (isDie == true) return;

        hp -= (int) (amounnt / 2.0f); // 외계인은 데미지를 절반으로 줄이는 특수 능력
        animator.SetTrigger("IsHit");

        if (hp <= 0)
            MonsterDie();
    }


    //몬스터 사망 시 처리 루틴
    //IEnumerator MonsterDie()
    //{
    //    if(isDie == true)
    //    {
    //        yield return null;
    //    }
    //    StopAllCoroutines();
    //    isDie = true;
    //    monsterState = MonsterState.die;
    //    nvAgent.isStopped = true;
    //    animator.SetTrigger("IsDie");

    //    //문제시 자삭, IsDie anim 0.2초 후에 비활성화
    //    yield return new WaitForSeconds(0.2f);
    //    gameObject.SetActive(false);

    //    //몬스터에 추가된 Collider를 비활성화
    //    gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
    //    foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
    //    {
    //        coll.enabled = false;
    //    }
    //}
    void MonsterDie()
    {

        if (isDie == true) return;

        Destroy(gameObject, 3f);
        //모든 코루틴을 정지
        StopAllCoroutines();
        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //hpSlider를 지우고 싶음. 허나 안됨.
        //hpSlider = null;

        //GameManager gm = GameObject.FindObjectOfType<GameManager>();
        //gm.BossDead();

        //GameManager alien = FindObjectOfType<GameManager>();
        //alien.
        //BossDead -> 보스 죽었을때 띄울 Text
        //gameoverTxt.SetActive(true);
        FindObjectOfType<MenuUI>().BossDie();
        //3초 후에 setactive(false)


        //몬스터에 추가된 Collider를 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }
        //Invoke("disappear3Sec", 3f);

    }
    void disappear3Sec()
    {
        gameObject.SetActive(false);
    }







}
