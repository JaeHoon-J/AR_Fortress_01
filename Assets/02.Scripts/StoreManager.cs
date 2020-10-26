using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab.ClientModels;

//StoreManager, StoreBehaviour, StoreUI
public class StoreManager : MonoBehaviour
{
    protected DataManager dataManager = null;
    protected DBManager dbManager = null;
    [SerializeField]
    protected GameObject rightBtnObj;
    [SerializeField]
    protected GameObject leftBtnObj;
    // PrefabPanel item1;
    [SerializeField]
    protected PrefabPanel instantiateItemPrefab;
    [SerializeField]
    protected Transform itemPrefabParentTransform;
    
    //이것은 버튼을 누르면 중복을 줄이기 위한 변수들
    public int allNumber=1;
    public int otherNumber=2;
    public int checkNumber = 0;// 중복 버튼 눌러서 리소스 줄이기 위해서 이것을 넣음!
    
    //내 돈 표시.
    //public GameObject mymoney;
    [SerializeField]
    protected Text userMoneyText;

    //스크롤바 value값 0으로 만들기
    [SerializeField]
    protected Scrollbar scrollbar;

    // Start is called before the first frame update
    void Start()
    {
        dataManager = DataManager.GetInstance();
        dbManager = DBManager.GetInstance();
        //델리게이트를 등록하는것??
        //NetWork.Get.onChangeMoneyDelegate는 매개변수가 int임 Setusermoney도 매개변수가 int임
        //매개변수가 int 인것들을 묶어서 한번에 처리하는것?
        //상점에 들어왓을때 스타트에서 NetWork.Get.onChangeMoneyDelegate에 SetUsermoney까지 연결을 해주는것?
        //상점에서 아이템을 사고 성공을하면 호출되는 콜백부분에서 NetWork.Get.onChangeMoneyDelegate를 호출?
        //NetWork.Get.onChangeMoneyDelegate 델리게이트는 NetWork의 BuyOk 콜백부분에서 
        //if (onChangeMoneyDelegate != null) onChangeMoneyDelegate(myMoney); 함 myMoney는 int형이고
        //BuyOk 콜백부분에서 델리게이트 실행될때 SetUsermoney가 이미 onChangeMoneyDelegate델리게이트에 등록되어 있으니?
        //현재 돈을 받아서 (int myMoney) SetUsermoney(현재돈을 받아서 스트링으로 텍스트로 표시) 가 한꺼번에 실행되는것?
        //그래서 아이템구매시 델리게이트에 SetUsermoney가 등록?연결? 되어 있으니 바뀐돈을 받아서 바로 돈이 바뀐것이 
        //스트링으로 바껴서 화면에 표시되는것인가?
        //SetUsermoney를 스태틱이나 싱글톤 스크립트에 작성후 콜백부분에서 호출하는것과 비슷한것인가? 
        //근데 UI에 표시를 해야하니 현재 UI가 연결된 스크립트에 있는 함수를 onChangeMoneyDelegate델리게이트로 등록후 
        //BuyOk 콜백부분에서 onChangeMoneyDelegate 델리게이트를 호출하는것인가?
        //onChangeMoneyDelegate를 호출하기위해선 무언가 연결되어 있어야하니 그래서 if문으로 null 체크 하는것인가?
        dbManager.onChangeMoneyDelegate += SetUsermoney;
        //NetWork.Get.GetMyMoney()는 현재 돈을 반환해준다
        //씬처음들어와서 내돈 표시해줘야하니 한번 표시해준다
        SetUsermoney(dbManager.GetMyMoney());
        OnOffButton();//맨 처음 scrollvalue가 0이기 때문에 오른쪽 버튼 0으루 만들어줘야지,
        scrollbar.onValueChanged.AddListener((value) => { OnOffButton(); });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickedRightBtn()
    {
        if(checkNumber ==1){scrollbar.value += 0.312f;}
        else{ scrollbar.value +=1;}
    }
    public void OnClikedLeftBtn()
    {
        if(checkNumber ==1){scrollbar.value -= 0.312f;}
        else{ scrollbar.value -=1;}
    }
    public void ResetScroll()
    {
        scrollbar.value = 0;
    }
    public void ChangingScrollValue()
    {
        
    }
    void OnOffButton()
    {
        if (scrollbar.value == 0)
        {
            leftBtnObj.SetActive(false);
            rightBtnObj.SetActive(true);
        }
        else if (scrollbar.value == 1)
        {
            leftBtnObj.SetActive(true);
            rightBtnObj.SetActive(false);
        }
        else
        {
            leftBtnObj.SetActive(true);
            rightBtnObj.SetActive(true);
        }
    }
    //매개변수로 돈을 넘겨주면 스트링으로 바꿔서 텍스트로 표시해준다
    public void SetUsermoney(int currentMoney)
    {
        //if (userMoneyText == null) userMoneyText = mymoney.GetComponentInChildren<Text>();
        userMoneyText.text = currentMoney.ToString();
    }
    //뒤로가기 버튼 넣음 됨!
    public void OnClickedBackButton()
    {
        SceneManager.LoadScene("03.Lobby");
    }

    public void UpdateItemPanel(List<CatalogItem> itemList) 
    {
         for(int i =0 ; i <itemList.Count; i++)
         {
            PrefabPanel item1 = Instantiate(instantiateItemPrefab , new Vector3(0,0,0), Quaternion.identity, itemPrefabParentTransform);
            CatalogItem networkItem = itemList[i];

            ItemData data = new ItemData();
            data.id = networkItem.ItemId;
            data.displayName = networkItem.DisplayName;
            data.descripition = networkItem.Description;
            data.price = (int)networkItem.VirtualCurrencyPrices["GD"];
            data.cost = data.price.ToString();

            item1.SetPrefabData(data);
            // item1.SetPrefabData(new ItemData(networkItem.ItemId, networkItem.DisplayName, networkItem.Description, networkItem.VirtualCurrencyPrices["GD"].ToString(),
            // networkItem.VirtualCurrencyPrices["GD"]));
         }
    }
   
    //자식들 없애는 곳!
    public void DestroyChildObj()
    {
        Debug.Log("자식 오브젝트 삭제 시작");
        for (int i =0; i < itemPrefabParentTransform.childCount; i++)
        {
            Destroy(itemPrefabParentTransform.GetChild(i).gameObject);
        }
    }
}