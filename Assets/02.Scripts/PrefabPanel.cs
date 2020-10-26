    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class PrefabPanel : MonoBehaviour
    {
    //protected DataManager dataManager = null;
    public Text title;
    public Text introduction;
    public GameObject buttonText;
    public Text cost1;
    public GameObject buttonImage;
    public Image image;
    public bool canBuy = true;

    public GameObject buy;
    GameObject blackCanvas;
    public GameObject noCoin;

    protected int price;
    protected string id;

    // Start is called before the first frame update
    void Start()
    {
        //dataManager = DataManager.GetInstance();
        // cost1 = buttonText.GetComponentInChildren<Text>();
        // title.text = displayname;
        // introduction.text = description;
        // cost1.text = itemcost;
        // for(int i=0; i<GameManager.Get.ownedItem_List.Count;i++)
        // {
        //     if(GameManager.Get.ownedItem_List[i] == displayname)
        //     {
        //         canBuy = false;
        //         cost1.text = "Owned";
        //         buttonImage.gameObject.SetActive(false);
        //     }
        // }
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void SetPrefabData(ItemData data) {
        if (data != null) {
            this.id = data.id;
            this.price = data.price;

            title.text = data.displayName;
            introduction.text = data.descripition;

            if (cost1 == null) cost1 = buttonText.GetComponentInChildren<Text>();
            cost1.text = data.cost;

            for(int j =0; j < DataManager.Instance.imgArr.Length; j++)
            {
                if(DataManager.Instance.imgArr[j].name == data.displayName)
                {
                    image.sprite = DataManager.Instance.imgArr[j];
                }
            }

            
            for(int i=0; i< DataManager.Instance.ownedItem_List.Count;i++)
            {
                if(DataManager.Instance.ownedItem_List[i] == data.displayName)
                {
                    canBuy = false;
                    cost1.text = "Owned";
                    buttonImage.gameObject.SetActive(false);
                }
            }
        }
    }
    public void Choice()
    {
        //아이템이 구입되어 있는 상태면 바로 함수 종료하고 아래부분 미실행
        if(canBuy == false) 
        {
            Debug.Log("이미 사신 아이템입니다.");
            return;
        }
        else if (DataManager.Instance.myMoney < int.Parse(cost1.text))
        {
            Debug.Log("돈이 부족합니다.");
            Instantiate(noCoin, new Vector3(0,0,0), Quaternion.identity);
            return;
        }
        Debug.Log("만들어지자!!!");
        //사는 창을 만들 때, 창에 정보도 넘겨준다.
        blackCanvas = Instantiate(buy, new Vector3(0,0,0), Quaternion.identity);
        blackCanvas.GetComponent<ForBuying>().itemId1 = id;
        blackCanvas.GetComponent<ForBuying>().price1 = price;
        blackCanvas.GetComponent<ForBuying>().panel = this.gameObject;
        blackCanvas.GetComponent<ForBuying>().completeBuy.gameObject.SetActive(false);
    }
    public void Close()
    {
        Destroy(blackCanvas, 0.1f);
    }

}

public class ItemData {
    public string displayName;
    public string descripition;
    public string cost;
    public int price;
    public string id;
    
    public ItemData(string id, string displayName, string descripition, string cost, int price) {
        this.id = id;
        this.displayName = displayName;
        this.cost = cost;
        this.price = price;
        this.descripition = descripition;
    }

    public ItemData() {}
}