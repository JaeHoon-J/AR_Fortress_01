using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisillEffect : MonoBehaviour
{
    //데미지 처리후 턴전환까지 남기고 사라진다
    protected int otherActNum;
    protected bool ismasterEffect;
    protected float time;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > 2)//이펙트 오브젝트 생성되고 2초있다가 파괴
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
 
    }

    private void OnDestroy()
    {
        //이펙트가 사라질때 판정을위한 마스터 이펙트인지 내가 마스터인지 확인후
        if (ismasterEffect && PhotonManager.Instance.isMaster) 
        {
            //턴을 넘긴다
            PhotonManager.Instance.Call_ChangeMasterClient();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(CompareTag("User") && PhotonManager.Instance.isMaster)
        {
            //트리거가 유저인걸 확인하면? 현재 방장이면 데미지 판정후 오브젝트 파괴
            otherActNum = other.GetComponent<Player>().actnum; //엑터 넘버를 가져온다
            //다른사용자들에게 내가 맟춘 대상의 엑터 넘버와 나의 데미지를 매개변수로 전달
            //이후는 각자 알아서 계산하게 한다
            PhotonManager.Instance.Call_CheckDamage(otherActNum, PhotonManager.Instance.myDmg);
        }
    }
    public void IsMasterEffect(bool ismaster)
    {
        ismasterEffect = ismaster;
    }
}
