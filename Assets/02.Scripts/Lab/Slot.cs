using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    protected DBManager dbManager;
    //이 스크립트는 Panel_Leg,Panel_Weapon,Panel_Body 의 PartsSlot 안의 Slot 안에 각각 붙어있음.
    //슬롯 테두리안의 로봇이미지 img
    public Image img;
    //선택시 큰화면으로 들고갈 이미지 저장 변수 
    public Image selectImage;
    //select 이미지 안에있는 각각 Spider , Ammogun 등 파츠별 실제 이름= DB에서 불러오기 위해 이름을 정확하게써야됨.
    public string itemName;
    //0 = 무기 1 = 바디  2 = 다리
    public int itemType;
    //텍스트를 정렬하기 위해 그룹으로 만듦. 텍스트루트에 GetChild(0~5) 하여 하위 항목에 하나씩 아이템 설명을 받아오면됨. 
    public GameObject textRoot;
    //WeaponList 의 카운트만큼 for문 돌려서 string네임과 같은걸찾음
    //잠금화면 이미지
    public Image lockitem;
    public bool activeSlot;



    public Transform objectPivot;
    void Start()
    {
        dbManager = DBManager.GetInstance();
        activeSlot = false;
        //인벤토리에 아이템이 들어와 있으면 아이템의 버튼 활성화 ( Default 상태의 잠금 해제)
        for (int i = 0; i < DataManager.Instance.ownedItem_List.Count; i++)
        {
            if (itemName == DataManager.Instance.ownedItem_List[i])
            {
                //버튼 활성화 시키기  활성화 되면 잠금이미지의 알파값 을 0으로 (투명하게)
                activeSlot = true;
                Color color = lockitem.color;
                color.a = 0;
                lockitem.color = color;
                //비활성화 일때 노말칼라가 빨강색인데 활성화되면 흰색으로 변경
                img.color = new Color(255, 255, 255);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 슬롯클릭하면 아이템의 상세정보를 표시해준다.
    /// </summary>
    public void GetSlotInfo()
    {
        //액티브 상태일때
        if (activeSlot == true)
        {
            selectImage.sprite = null;
            selectImage.sprite = img.sprite;
            //무기 리스트일때
            if (itemType == 0)
            {
                for (int i = 0; i < DataManager.Instance.weaponInfo_List.Count; i++)
                {
                    if (itemName == DataManager.Instance.weaponInfo_List[i].partName)
                    {
                        textRoot.transform.GetChild(0).GetComponent<Text>().text = "<color=#99ff99>Name : </color>" + DataManager.Instance.weaponInfo_List[i].partName;
                        textRoot.transform.GetChild(1).GetComponent<Text>().text = "<color=#80ff80>Type : </color>" + DataManager.Instance.weaponInfo_List[i].weapontype;
                        textRoot.transform.GetChild(2).GetComponent<Text>().text = "<color=#66ff66>Weight : </color>" + DataManager.Instance.weaponInfo_List[i].weight.ToString();
                        textRoot.transform.GetChild(3).GetComponent<Text>().text = "<color=#4dff4d>Attack : </color>" + DataManager.Instance.weaponInfo_List[i].attack.ToString();
                        textRoot.transform.GetChild(4).GetComponent<Text>().text = "<color=#33ff33>Range : </color>" + DataManager.Instance.weaponInfo_List[i].lange.ToString();
                    }


                }
            }
            //바디 일때
            else if (itemType == 1)
            {
                for (int i = 0; i < DataManager.Instance.bodyInfo_List.Count; i++)
                {
                    if (itemName == DataManager.Instance.bodyInfo_List[i].partName)
                    {
                        textRoot.transform.GetChild(0).GetComponent<Text>().text = "<color=#99ff99>Name : </color>" + DataManager.Instance.bodyInfo_List[i].partName;
                        textRoot.transform.GetChild(1).GetComponent<Text>().text = "<color=#80ff80>Type : </color>" + DataManager.Instance.bodyInfo_List[i].bodytype;
                        textRoot.transform.GetChild(2).GetComponent<Text>().text = "<color=#66ff66>Weight : </color>" + DataManager.Instance.bodyInfo_List[i].weight.ToString();
                        textRoot.transform.GetChild(3).GetComponent<Text>().text = "<color=#4dff4d>HP : </color>" + DataManager.Instance.bodyInfo_List[i].hp.ToString();
                        textRoot.transform.GetChild(4).GetComponent<Text>().text = "<color=#33ff33>Armor : </color>" + DataManager.Instance.bodyInfo_List[i].amor.ToString();
                    }
                }
            }
            //다리 일때
            else
            {
                for (int i = 0; i < DataManager.Instance.legInfo_List.Count; i++)
                {
                    if (itemName == DataManager.Instance.legInfo_List[i].partName)
                    {
                        textRoot.transform.GetChild(0).GetComponent<Text>().text = "<color=#99ff99>Name : </color>" + DataManager.Instance.legInfo_List[i].partName;
                        textRoot.transform.GetChild(1).GetComponent<Text>().text = "<color=#80ff80>Load : </color>" + DataManager.Instance.legInfo_List[i].totalweight.ToString();
                        textRoot.transform.GetChild(2).GetComponent<Text>().text = "<color=#66ff66>Speed : </color>" + DataManager.Instance.legInfo_List[i].speed.ToString();
                    }
                }
            }
        Assembly(itemName,itemType);
       

        }
        //string itemName으로 입력한 이름이   게임매니저의 아이템List와 일치하면 아이템 생성
    }
/// <summary>
/// 슬롯을 클릭하면 아이템을 실제로 생성해서 조립되는 모습을 보여준다.
/// </summary>
/// <param name="itemName"></param>
/// <param name="itemType"></param>
    
    public void Assembly(string itemName,int itemType)
    {
        //다리면 실행함 
        if(itemType ==2 )
        {
            //이미 생성된 다리가 있으면 파괴한다.
            if (DataManager.Instance.selectLeg != null)
            {
                Destroy(DataManager.Instance.selectLeg);
            }
            for(int i=0; i < DataManager.Instance.legPartsArr.Length; i++)
            {
                //이름과 일치하는 부품을 찾는다
                if(itemName == DataManager.Instance.legPartsArr[i].name)
                {
                    //다리를 생성한다.
                   DataManager.Instance.selectLeg =Instantiate(DataManager.Instance.legPartsArr[i], objectPivot.transform.position, Quaternion.identity);
                    //선택한 다리의 이름을 저장한다 DB에 올릴 이름
                    DataManager.Instance.userinfo.selectedLeg = DataManager.Instance.legPartsArr[i].name;
                    //변경된 프리팹도 기억한다.
                    DataManager.Instance.legPrefab = DataManager.Instance.legPartsArr[i];
                    //다리를 생성했는데 몸통이 이미 있다면?
                    if (DataManager.Instance.selectBody !=null)
                    {
                        //몸통의 위치는 다리의 높이에 따라 다시 설정한다.
                        DataManager.Instance.selectBody.transform.position = DataManager.Instance.selectLeg.GetComponent<LegParts>().bodyPos.transform.position;
                        //만약 선택된 무기도 있다면?
                        if(DataManager.Instance.selectWeapon !=null)
                        {
                            //무기의 위치는 다시 조정된 몸통의 위치에 따라 새로 조정한다.
                            DataManager.Instance.selectWeapon.transform.position = DataManager.Instance.selectBody.GetComponent<BodyParts>().weaponPos.transform.position;
                        }
                    }
                }
            }
        }
        else if(itemType ==1)
        {
            //몸통 만 선택시 예외처리
            if (DataManager.Instance.selectLeg == null)
            {
                Debug.Log("Please choose the Leg parts first.");
            }
            else
            {
                if (DataManager.Instance.selectBody != null)
                {
                    Destroy(DataManager.Instance.selectBody);
                }
                for (int i = 0; i < DataManager.Instance.bodyPartsArr.Length; i++)
                {
                    //이름 과 일치하는 몸통을 찾는다.
                    if (itemName == DataManager.Instance.bodyPartsArr[i].name)
                    {
                        //몸통을 생성한다
                        DataManager.Instance.selectBody = Instantiate(DataManager.Instance.bodyPartsArr[i], DataManager.Instance.selectLeg.GetComponent<LegParts>().bodyPos.transform.position, Quaternion.identity);
                        //선택한 몸통의 이름을 담아놓는다 DB 저장용
                        DataManager.Instance.userinfo.selectedBody = DataManager.Instance.bodyPartsArr[i].name;
                        //변경된 프리팹을 기억한다.
                        DataManager.Instance.bodyPrefab = DataManager.Instance.bodyPartsArr[i];
                        //몸통을 생성했는데 이미 생성된 무기가 있다면?
                        if (DataManager.Instance.selectWeapon != null)
                        {
                            //새로 생긴 몸통의 위치에 따라 무기의 위치도 재조정한다.
                            DataManager.Instance.selectWeapon.transform.position = DataManager.Instance.selectBody.GetComponent<BodyParts>().weaponPos.transform.position;
                        }
                    }
                }
            }
        }
        else
        {
            //무기만 선택시 예외처리
            if (DataManager.Instance.selectBody == null)
            {
                Debug.Log("Please choose the Body parts first.");
            }
            else
            {
                if (DataManager.Instance.selectWeapon != null)
                {
                    Destroy(DataManager.Instance.selectWeapon);
                }
                for (int i = 0; i < DataManager.Instance.weaponPartsArr.Length; i++)
                {
                    if (itemName == DataManager.Instance.weaponPartsArr[i].name)
                    {
                        DataManager.Instance.selectWeapon = Instantiate(DataManager.Instance.weaponPartsArr[i], DataManager.Instance.selectBody.GetComponent<BodyParts>().weaponPos.transform.position, Quaternion.identity);
                        //선택한 무기의 이름을 담아놓은다 DB 저장용
                        DataManager.Instance.userinfo.selectedWeapon = DataManager.Instance.weaponPartsArr[i].name;
                        //변경된 프리팹으로 기억한다.
                        DataManager.Instance.weaponPrefab = DataManager.Instance.weaponPartsArr[i];
                    }
                }
            }
        }
        dbManager.CurrentStats();
        dbManager.Stats();
    }

}
