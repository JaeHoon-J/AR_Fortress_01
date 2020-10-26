using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class TestClass
{
    public int age;
    public string name;

    public TestClass()
    {
        age = 0;
        name = "NoName";
    }


    public TestClass(int num, string str)
    {
        age = num;
        name = str;
    }
}




public class GameManager : MonoBehaviour
{
    [Header("무기 상세정보 담은 리스트")]
    public List<WeaponInfo> weaponInfo_List = new List<WeaponInfo>();    //무기의 상세 정보를 담을 리스트

    [Header("몸통 상세정보 담은 리스트")]
    public List<BodyInfo> bodyInfo_List = new List<BodyInfo>();    //몸통의 상세 정보를 담을 리스트

    [Header("다리 상세정보 담은 리스트")]
    public List<LegInfo> legInfo_List = new List<LegInfo>();    //다리의 상세 정보를 담을 리스트

    [Header("보유한 아이템 리스트")]
    public List<string> ownedItem_List = new List<string>();  //보유 아이템 리스트

    [Header("인트로에서 무기 Prefab정보 담을 배열")]
    public GameObject[] weaponPartsArr ;

    [Header("인트로에서 몸통 Prefab정보 담을 배열")]
    public GameObject[] bodyPartsArr ;

    [Header("인트로에서 다리 Prefab정보 담을 배열")]
    public GameObject[] legPartsArr ;

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
    public bool istypeNoeSame = false ;//타입 불일치 여부

    [Header("메세지")]
    public string Msg;

    private static GameManager m_Instance = null;
    public static GameManager Get { get { return m_Instance; } set { m_Instance = value; } }
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    void Start()
    {
        imgArr = Resources.LoadAll<Sprite>("04.Img");
        Debug.Log("스타트함수");
        weaponPartsArr = Resources.LoadAll<GameObject>("01.Weapon");
        bodyPartsArr = Resources.LoadAll<GameObject>("02.Body");
        legPartsArr = Resources.LoadAll<GameObject>("03.Leg");
        
    }

    
    void Update()
    {
        
    }
    /// <summary>
    /// DB에서 이름 받아와서 현재 조립된 프리팹을 찾아서 담아 놓는다
    /// 로그인후에 호출
    /// </summary>
    public void SelecPrefabs()
    {
        if(userinfo.selectedLeg !=null)
        {
            //다리 프리팹이 담긴 배열에서
            for(int i=0; i<legPartsArr.Length;i++)
            {
                //DB에 저장된것과 이름이 같은 프리팹을찾아서
              if(  userinfo.selectedLeg == legPartsArr[i].name)
                {
                    //프리팹을 찾아 놓는다
                    legPrefab = legPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    beforeLeg = legPrefab;
                }
            }
        }
        if(userinfo.selectedBody !=null)
        {
            for(int i=0; i<bodyPartsArr.Length;i++)
            {
                if(userinfo.selectedBody == bodyPartsArr[i].name)
                {
                    bodyPrefab = bodyPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    beforeBody = bodyPrefab;
                }
            }
        }
        if(userinfo.selectedWeapon !=null)
        {
            for(int i=0; i<weaponPartsArr.Length;i++)
            {
                if(userinfo.selectedWeapon == weaponPartsArr[i].name)
                {
                    weaponPrefab = weaponPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    beforeWeapon = weaponPrefab;
                }
            }
        }
    }
    /// <summary>
    /// 저장된 부품의 인덱스 찾기
    /// </summary>
    public void BeforeStats()
    {
        Debug.Log("능력치 가져오기");
        if (beforeLeg != null)
        {
            Debug.Log("다리 찾기 시작");
            for (int i = 0; i < legInfo_List.Count; i++)
            {
                if (beforeLeg.name == legInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    legindex = i;
                }
            }

        }
        else
        {
            Debug.Log("legPrefab is null");
        }

        if (beforeBody != null)
        {
            Debug.Log("몸통 찾기 시작");
            for (int i = 0; i < bodyInfo_List.Count; i++)
            {
                if (beforeBody.name == bodyInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    bodyindex = i;
                }
            }

        }
        else
        {
            Debug.Log("bodyPrefab is null");
        }

        if (beforeWeapon != null)
        {
            Debug.Log("무기 찾기 시작");
            for (int i = 0; i < weaponInfo_List.Count; i++)
            {
                if (beforeWeapon.name == weaponInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    weaponindex = i;
                }
            }
        }
        else
        {
            Debug.Log("weaponPrefab is null");
        }
        Stats();
    }
    /// <summary>
    /// 선택된 부품에 따른 현재 능력치 표기
    /// </summary>
    public void CurrentStats()
    {
        Debug.Log("능력치 가져오기");
        if (legPrefab != null)
        {
            Debug.Log("다리 찾기 시작");
            for (int i = 0; i < legInfo_List.Count; i++)
            {
                if (legPrefab.name == legInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    legindex = i;
                }
            }

        }
        else
        {
            Debug.Log("legPrefab us null");
        }

        if (bodyPrefab != null)
        {
            Debug.Log("몸통 찾기 시작");
            for (int i = 0; i < bodyInfo_List.Count; i++)
            {
                if (bodyPrefab.name == bodyInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    bodyindex = i;
                }
            }

        }
        else
        {
            Debug.Log("bodyPrefab is null");
        }

        if (weaponPrefab != null)
        {
            Debug.Log("무기 찾기 시작");
            for (int i = 0; i < weaponInfo_List.Count; i++)
            {
                if (weaponPrefab.name == weaponInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    weaponindex = i;
                }
            }
        }
        else
        {
            Debug.Log("weaponPrefab is null");
        }
        Stats();
    }
    /// <summary>
    /// 능력치 표시 beforePrfab 기준 이전에 저장된부품이나 현재 슬롯에서 선택한 부품이 있으면 능력치를 찾아서 넣어준다
    /// </summary>
    public void Stats()
    {
        if (beforeLeg != null|| legPrefab != null)
        {
            legtotalweight = legInfo_List[legindex].totalweight;
            legspeed = legInfo_List[legindex].speed;
            legamor = legInfo_List[legindex].amor;
        }
        if (beforeBody != null|| bodyPrefab !=null)
        {
            bodyhp = bodyInfo_List[bodyindex].hp;
            bodyamor = bodyInfo_List[bodyindex].amor;
            bodytype = bodyInfo_List[bodyindex].bodytype;
            bodyweight = bodyInfo_List[bodyindex].weight;
        }
        if (beforeWeapon != null || weaponPrefab !=null)
        {
            weaponweight = weaponInfo_List[weaponindex].weight;
            weapontype = weaponInfo_List[weaponindex].weapontype;
            weaponlange = weaponInfo_List[weaponindex].lange;
            weaponattack = weaponInfo_List[weaponindex].attack;
        }

        if ((beforeLeg != null && beforeBody != null && beforeWeapon != null)||(legPrefab!=null && bodyPrefab !=null && weaponPrefab !=null))
        {
            if (weaponweight + bodyweight > legtotalweight)
            {
                isloadOver = true;
            }
            else
            {
                isloadOver = false;
            }
        }

        if (beforeBody != null && beforeWeapon != null)
        {
            if (weapontype.Equals(bodytype))
            {
                istypeNoeSame = false;
            }
            else
            {
                istypeNoeSame = true;
            }
        }
    }
    /// <summary>
    /// 저장되지 않은 상태로 조립실을 나갔을경우를 대비해 이전 프리팹 정보를 다시 가져와서 선택된 무기로 바꿔준다.
    /// </summary>
    public void RecallBeforePrefab()
    {
        legPrefab = beforeLeg;
        bodyPrefab = beforeBody;
        weaponPrefab = beforeWeapon;
    }
    /// <summary>
    /// 저장되었을경우 조립실에서 선택해서 보여주던 legPrefab, bodyPrefab, weaponPrefab을 이전 정보로 변경한다.
    /// </summary>
    public void ChangeBeforePrefab()
    {
        beforeLeg = legPrefab;
        beforeBody = bodyPrefab;
        beforeWeapon = weaponPrefab;
    }
}

