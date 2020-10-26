using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo 
{
    //기본적인 사용자의 정보 필요에 따라 나중에 더 추가될수도 있음
    public string playfabId;
    public string nickName;
    public bool firstLogin;
    public string selectedWeapon;//조립된 무기 이름
    public string selectedBody;//조립 몸통 이름
    public string selectedLeg;//조립된 다리 이름

    public UserInfo()
    {
        playfabId = null;
        nickName = null;
        firstLogin = true;
        selectedWeapon = null;
        selectedBody = null;
        selectedLeg = null;
    }
}




