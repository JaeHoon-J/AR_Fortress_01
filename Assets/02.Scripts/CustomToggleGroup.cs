
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CustomToggleGroup : ToggleGroup
{
    /*
        여기서 toggle 그룹의 모든 이벤트를 여기서 처리를 할 것
        음 상속받는 것이니 나머진 필요 없고 내가 할 것만 할 것임.
        하나를 누르면 다른 곳에서도 알아야 되잖아
        그렇다면 ㅇㅅㅇ 여기서 스위치문을 쓰자!
        스토어 매니저에서는 if else문을 쓰고 될 것 같음.
    */

    [SerializeField]
    protected Sprite highlightImage;
    [SerializeField]
    protected Sprite normalImage;
    [SerializeField]
    protected StoreManager sm;
    [SerializeField]
    protected Toggle allToggle;
    [SerializeField]
    protected Toggle weaponToggle;
    [SerializeField]
    protected Toggle bodyToggle;
    [SerializeField]
    protected Toggle legToggle;

    // Start is called before the first frame update
    protected override void Start()
    {
        allToggle.onValueChanged.AddListener((value) => { ForChangingPanel("all"); });
        weaponToggle.onValueChanged.AddListener((value) => { ForChangingPanel("weapon"); });
        bodyToggle.onValueChanged.AddListener((value) => { ForChangingPanel("body"); });
        legToggle.onValueChanged.AddListener((value) => { ForChangingPanel("leg"); });
        allToggle.isOn = true;
        //ForChangingPanel("all");
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void ForChangingPanel(string checkClicked)
    {
        Debug.Log("ㅇㅅㅇ");
        sm.DestroyChildObj();
        sm.ResetScroll();
        //if문 4개를 만들것임
        if (checkClicked == "all")
        {
            //sm.DestroyChildObj();
            //allToggle.image.sprite = highlightImage;
            sm.UpdateItemPanel(DataManager.Instance.itemList);
            //sm.ResetScroll();
        }
        //else if(checkClicked != "all")
        //{
        //    allToggle.image.sprite = normalImage;
        //}
        else if (checkClicked == "weapon")
        {
            //sm.DestroyChildObj();
            //weaponToggle.image.sprite = highlightImage;
            sm.UpdateItemPanel(DataManager.Instance.weaponList);
            //sm.ResetScroll();
        }
        //else if(checkClicked != "weapon")
        //{
        //    weaponToggle.image.sprite = normalImage;
        //}
        else if (checkClicked == "body")
        {
            //sm.DestroyChildObj();
            //bodyToggle.image.sprite = highlightImage;
            sm.UpdateItemPanel(DataManager.Instance.bodyList);
            //sm.ResetScroll();
        }
        //else if (checkClicked != "body")
        //{
        //    bodyToggle.image.sprite = normalImage;
        //}
        else if (checkClicked == "leg")
        {
            //sm.DestroyChildObj();
            //legToggle.image.sprite = highlightImage;
            sm.UpdateItemPanel(DataManager.Instance.legList);
            //sm.ResetScroll();
        }
        //else if (checkClicked != "leg")
        //{
        //    legToggle.image.sprite = normalImage;
        //}
        else
        {
            Debug.Log("음 4개 외의 값");
        }
        // allToggle.onValueChanged.RemoveAllListeners();
        // weaponToggle.onValueChanged.RemoveAllListeners();
        // bodyToggle.onValueChanged.RemoveAllListeners();
        // legToggle.onValueChanged.RemoveAllListeners();
    }
    /*public void RecognizeChangingToggleState()
    {
        // allToggle.onValueChanged.AddListener((value) => { ForChangingPanel("all"); });
        // weaponToggle.onValueChanged.AddListener((value) => { ForChangingPanel("weapon"); });
        // bodyToggle.onValueChanged.AddListener((value) => { ForChangingPanel("body"); });
        // legToggle.onValueChanged.AddListener((value) => { ForChangingPanel("leg"); });
    }*/
}
