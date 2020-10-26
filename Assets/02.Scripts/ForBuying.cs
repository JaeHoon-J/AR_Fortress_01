using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForBuying : MonoBehaviour
{
    public string itemId1;
    public int price1;
    public GameObject panel;
    public GameObject completeBuy;
    protected DBManager dbManager;
    // Start is called before the first frame update
    void Start()
    {
        dbManager = DBManager.GetInstance();
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void BuyBtn()
    {
        //구입을 위한 코드
        Debug.Log("이 아이템을 구매합니다.");
        dbManager.BuyItem("Store", itemId1 ,"GD", price1);
        //구입 후 중복 구매 방지를 위한 코드
        panel.GetComponent<PrefabPanel>().canBuy = false;
        panel.GetComponent<PrefabPanel>().cost1.text = "OWNED";
        panel.GetComponent<PrefabPanel>().buttonImage.gameObject.SetActive(false);
        //정상적으로 샀으면 창 꺼지게 만들기.\
        completeBuy.gameObject.SetActive(true);
        Destroy(this.gameObject,0.5f);
    }
    public void Close()
    {
        Destroy(this.gameObject,0.1f);
    }
}
