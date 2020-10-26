using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

public class DataManager : MonoBehaviour
{
    protected static DataManager instance = null;
    //--------------------------------임시 추가
    public static DataManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    //------------------------------------------------
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public static DataManager GetInstance()
    {
           if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "DataManager";
                instance = obj.AddComponent<DataManager>();
            }
            return instance;
    }


    public int myMoney;//현재 내가 가진돈
    public List<CatalogItem> itemList = new List<CatalogItem>();
    //무기 리스트
    public List<CatalogItem> weaponList = new List<CatalogItem>();
    //몸통 리스트
    public List<CatalogItem> bodyList = new List<CatalogItem>();
    //다리 리스트
    public List<CatalogItem> legList = new List<CatalogItem>();

    [Header("무기 상세정보 담은 리스트")]
    public List<WeaponInfo> weaponInfo_List = new List<WeaponInfo>();    //무기의 상세 정보를 담을 리스트

    [Header("몸통 상세정보 담은 리스트")]
    public List<BodyInfo> bodyInfo_List = new List<BodyInfo>();    //몸통의 상세 정보를 담을 리스트

    [Header("다리 상세정보 담은 리스트")]
    public List<LegInfo> legInfo_List = new List<LegInfo>();    //다리의 상세 정보를 담을 리스트

    [Header("보유한 아이템 리스트")]
    public List<string> ownedItem_List = new List<string>();  //보유 아이템 리스트

    [Header("인트로에서 무기 Prefab정보 담을 배열")]
    public GameObject[] weaponPartsArr;

    [Header("인트로에서 몸통 Prefab정보 담을 배열")]
    public GameObject[] bodyPartsArr;

    [Header("인트로에서 다리 Prefab정보 담을 배열")]
    public GameObject[] legPartsArr;

    [Header("상점에서 사용할 스프라이트 이미지")]
    public Sprite[] imgArr;

    [Header("랩실에서 조립할려고 현재 선택한 파츠")]
    public GameObject selectLeg;//선택한 다리
    public GameObject selectBody;//선택한 몸통
    public GameObject selectWeapon;//선택한 무기

    [Header("DB에 저장될 유저의 정보")]
    public UserInfo userinfo;//타이틀 데이터에 저장될 플레이어 정보

    [Header("현재 조립된 무기의 프리팹")]
    public GameObject weaponPrefab;
    public GameObject bodyPrefab;
    public GameObject legPrefab;

    //클릭하고 파츠의 교체가 될때마다 이전의 정보를 기억한다.(최종 사용할 프리팹 정보)
    [Header("이전에 선택한 파츠프리팹")]
    public GameObject beforeLeg; //이전 다리
    public GameObject beforeBody; //이전 몸체
    public GameObject beforeWeapon; //이전 무기

    //CurrentStats() 함수에서 현재 조립된 부품의 인덱스 정보를 기억한다.
    [Header("현재 조립된 부품의 인덱스 정보")]
    public int legindex;
    public int bodyindex;
    public int weaponindex;

    [Header("다리 능력치")]
    public int legtotalweight;
    public float legspeed;
    public int legamor;

    [Header("몸통 능력치")]
    public int bodyhp;
    public int bodyamor;
    public string bodytype;
    public int bodyweight;

    [Header("무기 능력치")]
    public int weaponweight;
    public string weapontype;
    public int weaponlange;
    public int weaponattack;

    [Header("조립가능 여부")]
    public bool isloadOver = false;//하중 초과여부
    public bool istypeNoeSame = false;//타입 불일치 여부
    [Header("이전 부품의 이름")]
    public string beforeLegName;
    public string beforeBodyName;
    public string beforeWeaponName;

    [Header("메세지")]
    public string Msg;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
