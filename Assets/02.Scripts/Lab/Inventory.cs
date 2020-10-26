using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();

    public GameObject currItem;
    protected DataManager dataManager;
    //토글을 위한 변수
    public GameObject legBtn ;
    public GameObject bodyBtn ;
    public GameObject weaponBtn ;
    public Sprite highlightImage;
    public Sprite normalImage;
    
    //시작시 레그 인벤토리가 켜져있으므로 Leg 메뉴를 Selected 상태로 시작. 
    public bool buttonDown;

    void Start()
    {
        dataManager = DataManager.Instance;
        //시작할때 curritem에 다리 인벤토리를 담아둠 - 인벤토리 켠상태로 시작시 curritem에 안담겨서 아래코드 대로할시 버그생김
        //[09.28] 피드백 -  시작할떄 default 인벤토리 [먼저 조립되야하는 다리부분을] 켜놔야 좋을것 같다고 해서 수정
        weaponBtn.GetComponent<Button>().onClick.AddListener(() => { PanelButton("weapon"); });
        bodyBtn.GetComponent<Button>().onClick.AddListener(() => { PanelButton("body"); });
        legBtn.GetComponent<Button>().onClick.AddListener(() => { PanelButton("leg"); });
        PanelButton("leg");
    }
/*    public void SelectedButton()
    {
        //레그 인벤토리 의 selected 가켜지게
        buttonDown = true;
    }*/
    void Update()
    {
    }
    // Leg, Weapon , body 해당 메뉴마다 Onclick 시 해당 인벤토리 활성화 해놓음
    // inventory[0] = Weapon , [1] = Body [2] = leg   curritem = 현재 선택된 인벤토리
    /*public void Inven0Open()
    {
        if(currItem != null)
        {
            currItem.SetActive(false);
            currItem = null;
        }
        inventory[0].SetActive(true);
        currItem = inventory[0] ;
        checkImage(0);
    }
    public void Inven1Open()
    {
        if (currItem != null)
        {
            currItem.SetActive(false);
            currItem = null;
        }
        inventory[1].SetActive(true);
        currItem = inventory[1];
        checkImage(1);
    }
    public void Inven2Open()
    {
        if (currItem != null)
        {
            currItem.SetActive(false);
            currItem = null;
        }
        inventory[2].SetActive(true);
        currItem = inventory[2];
        checkImage(2);
    }*/
    public void Inven0Close()
    {
        inventory[0].SetActive(false);
    }
    public void Inven1Close()
    {
        inventory[1].SetActive(false);
    }
    public void Inven2Close()
    {
        inventory[2].SetActive(false);
    }
    //좌 상단 Back Key  클릭시 로비씬으로 이동
    public void BackToMainScene()
    {
        //나가기 전에 변경사항이 있을때 저장을 안했으면 저장전 상태도 되돌린다
        if (dataManager.userinfo.selectedLeg != dataManager.beforeLegName)
        {
            dataManager.userinfo.selectedLeg = dataManager.beforeLegName;
        }
        if (dataManager.userinfo.selectedBody != dataManager.beforeBodyName)
        {
            dataManager.userinfo.selectedBody = dataManager.beforeBodyName;
        }
        if (dataManager.userinfo.selectedWeapon != dataManager.beforeWeaponName)
        {
            dataManager.userinfo.selectedWeapon = dataManager.beforeWeaponName;
        }
        SceneManager.LoadScene("03.Lobby");
    }
    /*
    //여기는 이제 토글 넣어주려고 하는 부분
    void checkImage(int k)
    {
        switch(k)
        {
            case 2 :
                weaponBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
                bodyBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
                legBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
            break;

            case 1 :
                weaponBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
                bodyBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
                legBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
            break;
            
            case 0 :
                weaponBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
                bodyBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
                legBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
            break;
        }
    }*/
    
    void PanelButton(string checkButton)
    {
        if (currItem != null)
        {
            Debug.Log("ㅇㅇㅇ");
            currItem.SetActive(false);
            currItem = null;
        }
        if (checkButton == "weapon")
        {
            weaponBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
            inventory[0].SetActive(true);
            currItem = inventory[0];
        }
        else if(checkButton != "weapon")
        {
            weaponBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;

        }
        if (checkButton == "body")
        {
            bodyBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
            inventory[1].SetActive(true);
            currItem = inventory[1];
        }
        else if (checkButton != "body")
        {
            bodyBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;

        }
        if (checkButton == "leg")
        {
            legBtn.GetComponent<UnityEngine.UI.Image>().sprite = highlightImage;
            inventory[2].SetActive(true);
            currItem = inventory[2];
        }
        else if (checkButton != "leg")
        {
            legBtn.GetComponent<UnityEngine.UI.Image>().sprite = normalImage;
        }
    }
}
