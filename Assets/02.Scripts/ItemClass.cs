using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
    //무기 오브젝트에 달릴 스트립트
    //무기의 관한 능력치와 정보들
    public string partName;//무기 이름
    public string partType;//무기 , 몸, 다리 중에 어느부위인지 구별하기위함
    public string weapontype; //어깨형 , 팔형, 탑형 인지 구분하기위함
    public int weight;//무게
    public int attack;//공격력
    public int lange;//사거리

}

[System.Serializable]
public class BodyInfo
{
    public string partName;//파츠 이름
    public string partType;//무기, 몸, 다리 어느부위인지 구별하기위함
    public string bodytype;//어깨형, 팔형, 탑형 인지 구분하기위함
    public int weight;//무게
    public int hp;//체력
    public int amor;//방어력

}

[System.Serializable]
public class LegInfo
{
    public string partName; //파츠 이름
    public string partType;//무기,몸 다리 어느부인인지 구별하기 위함
    public int totalweight;//총 견딜수 있는 하중량
    public int speed;//속도
    public int amor;//방어력

}
