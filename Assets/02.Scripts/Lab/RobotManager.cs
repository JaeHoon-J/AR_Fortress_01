using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    protected DBManager dbManager;
    public GameObject pivot;
    public GameObject assembleComplete;
    
    /// <summary>
    ///  GameManager의 Userinfo형태를 저장한다
    /// </summary>
    public void ConfirmAssemble()
    {
        if(DataManager.Instance.isloadOver == true)
        {
            DataManager.Instance.Msg = "하중이 초과했습니다";
        }
        else if(DataManager.Instance.istypeNoeSame == true)
        {
            DataManager.Instance.Msg = "무기와 몸체의 타입이 일치하지 않습니다.";
        }
        else
        {
            DataManager.Instance.beforeLegName = DataManager.Instance.userinfo.selectedLeg;
            DataManager.Instance.beforeBodyName = DataManager.Instance.userinfo.selectedBody;
            DataManager.Instance.beforeWeaponName = DataManager.Instance.userinfo.selectedWeapon;
            dbManager.SetData(DataManager.Instance.userinfo);
            dbManager.ChangeBeforePrefab();
            DataManager.Instance.Msg = "저장되었습니다.";
            assembleComplete.gameObject.SetActive(true);
            StartCoroutine("SetActiveFalse");
        }
    }
    IEnumerator SetActiveFalse()
    {
        yield return new WaitForSeconds(0.5f);
        assembleComplete.gameObject.SetActive(false);
    }
    private void Start()
    {
        //DataManager.Instance
        dbManager = DBManager.GetInstance();
        dbManager.RecallBeforePrefab(); //이전 프리팹 정보를 가져오는것
        dbManager.BeforeStats(); //이전프리팹 정보로 능력치를 찾아온다.
        dbManager.Stats(); //능력치를 표시해준다

        DataManager.Instance.beforeLegName = DataManager.Instance.userinfo.selectedLeg;
        DataManager.Instance.beforeBodyName = DataManager.Instance.userinfo.selectedBody;
        DataManager.Instance.beforeWeaponName = DataManager.Instance.userinfo.selectedWeapon;
        if (DataManager.Instance.legPrefab !=null)
        {
            //이전 프리팹 정보로 다리를 만들어서 생성시킨다.
            DataManager.Instance.selectLeg = Instantiate(DataManager.Instance.beforeLeg, pivot.transform.position,
                Quaternion.identity);
            //이전 몸통 프리팹을 생성한다.
            DataManager.Instance.selectBody = Instantiate(DataManager.Instance.beforeBody,
                DataManager.Instance.selectLeg.GetComponent<LegParts>().bodyPos.position,
               Quaternion.identity);
            //이전 무기 프리팹을 생성한다.
            DataManager.Instance.selectWeapon = Instantiate(DataManager.Instance.beforeWeapon,
                DataManager.Instance.selectBody.GetComponent<BodyParts>().weaponPos.position,
                Quaternion.identity);
        }
    }
}
