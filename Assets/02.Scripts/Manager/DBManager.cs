using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json;

/// <summary>
/// 플레이팹(DB)에 관련된 메소드들 데이터를 저장하고 받을때 사용할 메소드
/// </summary>
public class DBManager : MonoBehaviour
{
    protected static DBManager instance = null;
    //protected DataManager dataManager;
    //protected PhotonManager.Instance PhotonManager.Instance;
    private void Awake()
    {
        instance = this;
    }
    public static DBManager GetInstance()
    {
        if(instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = "DBManager";
            instance = obj.AddComponent<DBManager>();
        }
        return instance;
    }
    private void Start()
    {
        //데이타매니저 가져와야함
        //dataManager = DataManager.GetInstance();
        //포톤 매니저도 가져와야함
        //PhotonManager.Instance = PhotonManager.Instance.GetInstance();

        if (DataManager.Instance.imgArr.Length == 0)
        {
            //DataManager.Instance
            DataManager.Instance.imgArr = Resources.LoadAll<Sprite>("04.Img");
            DataManager.Instance.weaponPartsArr = Resources.LoadAll<GameObject>("01.Weapon");
            DataManager.Instance.bodyPartsArr = Resources.LoadAll<GameObject>("02.Body");
            DataManager.Instance.legPartsArr = Resources.LoadAll<GameObject>("03.Leg");
        }
    }
    public string email;//입력받은 이메일값 이메일 형식으로 @.com으로 적어
    public string password;//입력받은 비밀번호
    public string userName;//입력받은 사용자 이름
    public string nickName;//입력받은 닉네임
    public string buylastItem;//상점에서 방금 구입한 아이템 이름
    public bool modifyOk = false;
    public bool modifyFail = false;
    public int buyItemPice;//방금 구입한 아이템의 가격

    public string displayName;//플레이어 이름
    public string playfabid;
    //델리게이트 
    public delegate void ChangeMoneyDelegate(int totalMoney);
    public ChangeMoneyDelegate onChangeMoneyDelegate;

    
    #region 데이터를 저장하는것 SetData(UserInfo info) 
    /// <summary>
    /// 유저의 정보를 저장하기 변경사항이 있으면 Userinfo 클래스를 변경하고 매개변수로 변경된 클래스를 넣어서 호출하면
    /// 변경된 클래스가 저장이 된다
    /// </summary>
    /// <param name="info"></param>
    public void SetData(UserInfo info)
    {
        string json = JsonUtility.ToJson(info);
        var request = new PlayFab.ClientModels.UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                 { "Info", json },
            },
            Permission = PlayFab.ClientModels.UserDataPermission.Public //수정가능하게 퍼미션을 퍼블릭으로
        };
        PlayFabClientAPI.UpdateUserData(request, OnSetdateSuccess, OnSetdataFail);
    }

    private void OnSetdataFail(PlayFabError obj)
    {
        Debug.Log("데이터 저장 실패");
    }

    private void OnSetdateSuccess(PlayFab.ClientModels.UpdateUserDataResult obj)
    {
        Debug.Log("데이터 저장 성공");
    }
    #endregion

    #region 정보가져오기 Getdata(string myId)
    /// <summary>
    /// 사용자의 정보 불러오기
    /// </summary>
    public void Getdata(string myId)
    {
        //myId 를 이용해서 플레이팹에서 나의 정보를 불러올수 있다.
        var request = new PlayFab.ClientModels.GetUserDataRequest() { PlayFabId = myId };
        PlayFabClientAPI.GetUserData(request, OnGetdataSuccess, OnGetdataFail);
    }
    private void OnGetdataFail(PlayFabError obj)
    {
        Debug.Log("가져오기 실패");
    }
    private void OnGetdataSuccess(PlayFab.ClientModels.GetUserDataResult obj)
    {
        Debug.Log("가져오기 성공");
        if (obj.Data.ContainsKey("Info"))
        {
            //가져올 정보가 있을때
            var yourObject = JsonUtility.FromJson<UserInfo>(obj.Data["Info"].Value);
            DataManager.Instance.userinfo = yourObject;
            //선택된 부품 프리팹 가져오기
            SelecPrefabs();
        }
        else
        {
            //플레이팹 아이디가 존재해서 성공 콜백은 넘어왔지만 키값에 대응하는 정보가 없으면 처음 로그인 상요자이므로 기본정보 Set해주기
            SetData(DataManager.Instance.userinfo);
            Debug.Log("유저 기본정보 세팅");
            //가져올 정보가 없을때 -> 기본 데이터 세팅한다 최초 로그인한 회원인 상태이다.

        }
        //키가 없다고 에러가 뜨면 -> 기본 정보 세팅
    }
    #endregion

    #region 돈 얻기 GetMoney(int amount)
    /// <summary>
    /// amount 만큼 돈이 늘어남
    /// </summary>
    /// <param name="amount"></param>
    public void GetMoney(int amount)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = "GD" }, (result) =>
        {
            Debug.Log("돈 추가");
            DataManager.Instance.myMoney = result.Balance;//현재 돈 변경
        },
        (error) => Debug.Log("돈 얻기 실패"));
    }
    #endregion

    #region 인벤토리 정보 불러오기 GetInventory()
    /// <summary>
    /// 인벤토리 정보 불러오기
    /// </summary>
    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(), (result) =>
        {
            Debug.Log("인벤토리 불러오기 성공");
            if (result.VirtualCurrency["GD"] < 0)
            {
                DataManager.Instance.myMoney = 0;
            }
            else
            {
                DataManager.Instance.myMoney = result.VirtualCurrency["GD"]; //인벤토리 열면서 현재 돈 가져오기
            }
            //인벤토리 불러오기 성공하면 동작해야할것 작성....
            Debug.Log(result.VirtualCurrency);//가상화폐 종류별로  내가 가지고있는 잔액불러오기(배열) 
            for (int i = 0; i < result.Inventory.Count; i++)
            {
                //인벤토리 리스트에 있는 아이템들의 각정보들
                var inventory = result.Inventory[i];
                DataManager.Instance.ownedItem_List.Add(result.Inventory[i].DisplayName);
                /*  Debug.Log(inventory.DisplayName);
                  Debug.Log(inventory.ItemId);
                  Debug.Log(inventory.ItemInstanceId);
                  Debug.Log(inventory.UnitCurrency);
                  Debug.Log(inventory.CustomData);
                  Debug.Log(inventory.PurchaseDate);*/
            }
        },
        (error) => Debug.Log("인벤토리 불러오기 실패"));
    }
    #endregion

    #region 상점 목록 불러오기 GetCatalogItem(string storeName)
    /// <summary>
    /// 상점 불러오기 상점의 내용은 Playfab 홈페이지에서 설정할수 있다(아이템 가격 중첩여부 기간 등..)
    /// 현재는 DB에 상점이 Store 만 있으므로 매개변수로 Store를 입력하면된다
    /// itemList에 정체 리스트가 담긴다
    /// </summary>
    /// <param name="storeName"></param>
    public void GetCatalogItem(string storeName)
    {
        //CatalogVersion 은 홈페이지에서 내가 만드 CatalogVersion 의 이름이다
        //CatalogVersion 은 string으로 언제는 변경될수 있음
        PlayFabClientAPI.GetCatalogItems(new PlayFab.ClientModels.GetCatalogItemsRequest() { CatalogVersion = storeName }, (result) =>
        {
            Debug.Log("상점 불러오기 성공");

            DataManager.Instance.itemList = result.Catalog;
            //리스트 낮은 가격순으로 정렬
            //SortItemByPrice(itemList);
            for (int i = 0; i < result.Catalog.Count; i++)
            {

                var catalog = result.Catalog[i];
                //태그로 비교한다.
                if (catalog.Tags[0] == "Weapon")
                {
                    DataManager.Instance.weaponList.Add(catalog);//무기는 무기리스트에 추가
                    //아이템의 customData 불러오는 부분
                    //GetCustomIteminfo(catalog);
                }
                else if (catalog.Tags[0] == "LowerBody")
                {
                    DataManager.Instance.legList.Add(catalog);//다리는 다리 리스트에 추가
                }
                else
                {
                    DataManager.Instance.bodyList.Add(catalog);//그외에 것들은 몸통 리스트에 추가
                }
            }
            StartCoroutine("OrganizeItem_2");
        },
        (error) => Debug.Log("상점 불러오기 실패"));
    }
    #endregion

    #region OrganizeItem_3()에서 호출됨 아이템의 상세한 각 능력치를 받아온다 
    /// <summary>
    /// 리스트에서 각 아이템의 상세 능력치 딕셔너리로 가져오기 + dataManager에 있는 아이템 상세정보리스트에 정보 넣어주기
    /// </summary>
    public void GetCustomIteminfo(CatalogItem item)
    {
        Debug.Log("아이템 세부 정보 받기");
        var iteminfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.CustomData);

        if (item.Tags[0] == "Weapon")
        {
            WeaponInfo info = new WeaponInfo();
            info.partName = item.DisplayName;//이름 넣기
            info.partType = item.Tags[0];//무기인지 몸인지 다리인지 널기
            info.weapontype = iteminfo["type"]; //무기가 어깨형인지 탑형인지 팔형인지
            info.attack = int.Parse(iteminfo["attack"]);
            info.weight = int.Parse(iteminfo["weight"]);
            info.lange = int.Parse(iteminfo["lange"]);
            DataManager.Instance.weaponInfo_List.Add(info);
            Debug.Log("무기 정보 추가");
        }
        else if (item.Tags[0] == "LowerBody")
        {
            LegInfo info = new LegInfo();
            Debug.Log(item.DisplayName + "의 이동속도: " + iteminfo["speed"]);
            Debug.Log(item.DisplayName + "의 총 하중: " + iteminfo["totalweight"]);
            Debug.Log(item.DisplayName + "의 방어력: " + iteminfo["amor"]);
            info.partName = item.DisplayName;
            info.partType = item.Tags[0];
            info.totalweight = int.Parse(iteminfo["totalweight"]);
            info.speed = int.Parse(iteminfo["speed"]);
            info.amor = int.Parse(iteminfo["amor"]);
            DataManager.Instance.legInfo_List.Add(info);
            Debug.Log("다리 정보 추가");

        }
        else
        {
            BodyInfo info = new BodyInfo();
            info.partName = item.DisplayName;//이름 넣기
            info.partType = item.Tags[0]; //무기인지 몸인지 다리인지 넣기
            info.bodytype = iteminfo["type"];//몸통이 어깨인지 탑형인지 팔형인지
            info.weight = int.Parse(iteminfo["weight"]);//무게
            info.hp = int.Parse(iteminfo["hp"]);//체력
            info.amor = int.Parse(iteminfo["amor"]);//방어력
            DataManager.Instance.bodyInfo_List.Add(info);
            Debug.Log("몸통 정보 추가");
        }
    }
    #endregion

    #region OrganizeItem_2()에서 호출됨 아이템 리스트 낮은순으로 정렬하기 
    /// <summary>
    /// 아이템 리스트를 받아와서 가격에 따라 낮은 가격부터 정렬하기
    /// </summary>
    /// <param name="list"></param>
    public void SortItemByPrice(List<CatalogItem> list)
    {
        //#if UNITY_EDITOR
        //        Debug.Log("---정렬전 리스트---");
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            Debug.Log(list[i].VirtualCurrencyPrices["GD"]);
        //        }
        //        Debug.Log("------");
        //#endif
        list.Sort(delegate (CatalogItem A, CatalogItem B)
        {

            if (A.VirtualCurrencyPrices["GD"] > B.VirtualCurrencyPrices["GD"]) return 1;
            else if (A.VirtualCurrencyPrices["GD"] < B.VirtualCurrencyPrices["GD"]) return -1;
            return 0;
        });
        //#if UNITY_EDITOR
        //        Debug.Log("---정렬후---");
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            Debug.Log(list[i].VirtualCurrencyPrices["GD"]);
        //        }
        //        Debug.Log("------");
        //#endif
    }
    #endregion

    #region 아이템 구매시 호출 상점 씬에서 호출한다 BuyItem(string storeName, string itemid, string virtualCurrency, int price)
    /// <summary>
    /// 상점에서 아이텝 구입
    /// </summary>
    /// <param name="storeName"></param> <=스토어 이름
    /// <param name="itemid"></param> <=아이템이 고유의 아이디 playfab 홈페이지에서 앙템 설정해줄때 생성됨 받아올수 아이템 정보를 받아올때 가져 올수 있음
    /// <param name="virtualCurrency"></param><=게임내 화폐의 종류 playfab 홈페이지에서 화폐 설정가능하다 
    /// <param name="price"></param><= 100원? 200원? 얼마나 게임내 화페를 사용할지 
    public void BuyItem(string storeName, string itemid, string virtualCurrency, int price)
    {
        buylastItem = itemid;
        buyItemPice = price;
        var request = new PurchaseItemRequest() { CatalogVersion = storeName, ItemId = itemid, VirtualCurrency = virtualCurrency, Price = price };
        PlayFabClientAPI.PurchaseItem(request, BuyOk, BuyFail);
    }

    private void BuyFail(PlayFabError obj)
    {
        Debug.Log("구입 실패");
    }
    private void BuyOk(PurchaseItemResult obj)
    {
        Debug.Log("구입 성공");
        //dataManager의 보유 아이템 리스트(스트링타입)에 방금 구입한 아이템의 이름을 넣어준다
        DataManager.Instance.ownedItem_List.Add(buylastItem);
        DataManager.Instance.myMoney -= buyItemPice;//방금 구입한 아이템의 가격만큼 현재 가진돈을 빼준다
        if (onChangeMoneyDelegate != null) onChangeMoneyDelegate(DataManager.Instance.myMoney);
    }
    #endregion

    #region 현재 돈을 리턴해줌 GetMyMoney()
    public int GetMyMoney()
    {
        Debug.Log("돈 리턴");
        return DataManager.Instance.myMoney;
    }
    #endregion

    #region 닉네임을 입력받아 업데이트 시킨다 첫로그인 시에만 호출되게한다 ModifyDisplayName(string nickname) 로비신에서 호출한다
    /// <summary>
    /// 플레이어 디스플레이 네임 수정하기 첫로그인 시에만 수정창을 한번띄워주게한다
    /// </summary>
    /// <param name="nickname"></param>
    public void ModifyDisplayName(string nickname)
    {
        displayName = nickname;
        var request = new UpdateUserTitleDisplayNameRequest() { DisplayName = nickname };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, UpdatenameSuccess, GetprofileFail);
    }

    private void GetprofileFail(PlayFabError obj)
    {
        Debug.Log("닉네임 변경 실패");
        modifyFail = true;
    }

    private void UpdatenameSuccess(UpdateUserTitleDisplayNameResult obj)
    {
        Debug.Log("닉네임 변경 성공");
        DataManager.Instance.userinfo.nickName = obj.DisplayName;
        DataManager.Instance.userinfo.firstLogin = false;
        SetData(DataManager.Instance.userinfo);
        Debug.Log("변경된 이름은 ; " + obj.DisplayName);
        modifyOk = true;

    }
    #endregion

    #region 로그인 성공후에 호출되는부분들 상점아이템 받아오기 -> 아이템 가격순 정렬->아이템별 상세 정보 담아서 리스트 만들기
    public IEnumerator LoadItemList()
    {
        Debug.Log("LoadItemList");
        yield return StartCoroutine("OrganizeItem_1");
    }
    /// <summary>
    /// 상점 목록을 불러와 태그별로 구분해서 3개의 리스트에 나눠서 담는다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OrganizeItem_1()
    {
        yield return null;
        Debug.Log("OrganizeItem_1");
        GetCatalogItem("Store");

    }
    /// <summary>
    /// 부위별로 정리된 리스트 3개를 가격에 따라 낮은순서대로 정렬하기
    /// </summary>
    /// <returns></returns>
    public IEnumerator OrganizeItem_2()
    {
        yield return null;
        Debug.Log("OrganizeItem_2");
        //List<CatalogItem> list 형태 리스트가 매개변수로 사용됨
        SortItemByPrice(DataManager.Instance.itemList); //전체아이템 리스트 가격 낮은 순으로 정렬
        SortItemByPrice(DataManager.Instance.weaponList);//무기리스트 가격낮은 순으로 정렬
        SortItemByPrice(DataManager.Instance.bodyList);//몸통 리스트 가격낮은 순으로 정렬
        SortItemByPrice(DataManager.Instance.legList);//다리 리스트 가격낮은 순으로 정렬

        StartCoroutine("OrganizeItem_3");
    }
    /// <summary>
    /// 각 아이템별 상세 정보를 담는다.
    /// dataManager 스크립트의 weaponInfo_List ,bodyInfo_List, legInfo_Listdp 에 정보를 담기 위함
    /// </summary>
    /// <returns></returns>
    public IEnumerator OrganizeItem_3()
    {
        yield return null;
        Debug.Log("OrganizeItem_3");
        //GetCustomIteminfo 함수는 리스트에 있는 아이템들의 이름 타입 커스텀 데이터를 받아와서
        //dataManager에 있는 무기.몸,다리 상세정보리스트에 정보를 담는다.
        for (int i = 0; i < DataManager.Instance.weaponList.Count; i++)
        {
            GetCustomIteminfo(DataManager.Instance.weaponList[i]);
        }
        for (int i = 0; i < DataManager.Instance.bodyList.Count; i++)
        {
            GetCustomIteminfo(DataManager.Instance.bodyList[i]);
        }
        for (int i = 0; i < DataManager.Instance.legList.Count; i++)
        {
            GetCustomIteminfo(DataManager.Instance.legList[i]);
        }
        yield return new WaitUntil(() => DataManager.Instance.legInfo_List.Count == 7);
        //무기 정보 리스트가 차면->기존에 조립된 것이 있다면 바로 정보를 찾아서 불러온다
        BeforeStats();
        StartCoroutine("MoveToLobby");
    }
    public IEnumerator MoveToLobby()
    {
        yield return null;
        PhotonManager.Instance.JoinLobby();
    }
    #endregion

    #region DB에서 저장된 정보로 프리팹을 찾는다 로그인성공시 호출 SelecPrefabs()
    /// <summary>
    /// DB에서 이름 받아와서 현재 조립된 프리팹을 찾아서 담아 놓는다
    /// 로그인후에 호출
    /// </summary>
    public void SelecPrefabs()
    {
        if (DataManager.Instance.userinfo.selectedLeg != null)
        {
            //다리 프리팹이 담긴 배열에서
            for (int i = 0; i < DataManager.Instance.legPartsArr.Length; i++)
            {
                //DB에 저장된것과 이름이 같은 프리팹을찾아서
                if (DataManager.Instance.userinfo.selectedLeg == DataManager.Instance.legPartsArr[i].name)
                {
                    //프리팹을 찾아 놓는다
                    DataManager.Instance.legPrefab = DataManager.Instance.legPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    DataManager.Instance.beforeLeg = DataManager.Instance.legPrefab;
                }
            }
        }
        if (DataManager.Instance.userinfo.selectedBody != null)
        {
            for (int i = 0; i < DataManager.Instance.bodyPartsArr.Length; i++)
            {
                if (DataManager.Instance.userinfo.selectedBody == DataManager.Instance.bodyPartsArr[i].name)
                {
                    DataManager.Instance.bodyPrefab = DataManager.Instance.bodyPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    DataManager.Instance.beforeBody = DataManager.Instance.bodyPrefab;
                }
            }
        }
        if (DataManager.Instance.userinfo.selectedWeapon != null)
        {
            for (int i = 0; i < DataManager.Instance.weaponPartsArr.Length; i++)
            {
                if (DataManager.Instance.userinfo.selectedWeapon == DataManager.Instance.weaponPartsArr[i].name)
                {
                    DataManager.Instance.weaponPrefab = DataManager.Instance.weaponPartsArr[i];
                    //방금 불러온 정보를 이전정보로 저장
                    DataManager.Instance.beforeWeapon = DataManager.Instance.weaponPrefab;
                }
            }
        }
    }
    #endregion

    #region 아이템 능력치가 담긴 리스트에서 내가 저장한 아이템의 능력치를 가져오기위해 인덱스 찾기 부위별로 BeforeStats()
    /// <summary>
    /// 저장된 부품의 인덱스 찾기
    /// </summary>
    public void BeforeStats()
    {
        Debug.Log("능력치 가져오기");
        if (DataManager.Instance.beforeLeg != null)
        {
            Debug.Log("다리 찾기 시작");
            for (int i = 0; i < DataManager.Instance.legInfo_List.Count; i++)
            {
                if (DataManager.Instance.beforeLeg.name == DataManager.Instance.legInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.legindex = i;
                }
            }

        }
        else
        {
            Debug.Log("legPrefab is null");
        }

        if (DataManager.Instance.beforeBody != null)
        {
            Debug.Log("몸통 찾기 시작");
            for (int i = 0; i < DataManager.Instance.bodyInfo_List.Count; i++)
            {
                if (DataManager.Instance.beforeBody.name == DataManager.Instance.bodyInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.bodyindex = i;
                }
            }

        }
        else
        {
            Debug.Log("bodyPrefab is null");
        }

        if (DataManager.Instance.beforeWeapon != null)
        {
            Debug.Log("무기 찾기 시작");
            for (int i = 0; i < DataManager.Instance.weaponInfo_List.Count; i++)
            {
                if (DataManager.Instance.beforeWeapon.name == DataManager.Instance.weaponInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.weaponindex = i;
                }
            }
        }
        else
        {
            Debug.Log("weaponPrefab is null");
        }
        Stats();
    }
    #endregion

    #region 조립실에서 현재 내가 선택한 부픔의 인덱스를 찾아 정보를 가져온다 CurrentStats()
    /// <summary>
    /// 선택된 부품에 따른 현재 능력치 표기
    /// </summary>
    public void CurrentStats()
    {
        Debug.Log("능력치 가져오기");
        if (DataManager.Instance.legPrefab != null)
        {
            Debug.Log("다리 찾기 시작");
            for (int i = 0; i < DataManager.Instance.legInfo_List.Count; i++)
            {
                if (DataManager.Instance.legPrefab.name == DataManager.Instance.legInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.legindex = i;
                }
            }

        }
        else
        {
            Debug.Log("legPrefab us null");
        }

        if (DataManager.Instance.bodyPrefab != null)
        {
            Debug.Log("몸통 찾기 시작");
            for (int i = 0; i < DataManager.Instance.bodyInfo_List.Count; i++)
            {
                if (DataManager.Instance.bodyPrefab.name == DataManager.Instance.bodyInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.bodyindex = i;
                }
            }

        }
        else
        {
            Debug.Log("bodyPrefab is null");
        }

        if (DataManager.Instance.weaponPrefab != null)
        {
            Debug.Log("무기 찾기 시작");
            for (int i = 0; i < DataManager.Instance.weaponInfo_List.Count; i++)
            {
                if (DataManager.Instance.weaponPrefab.name == DataManager.Instance.weaponInfo_List[i].partName)
                {
                    Debug.Log("인덱스 찾음");
                    DataManager.Instance.weaponindex = i;
                }
            }
        }
        else
        {
            Debug.Log("weaponPrefab is null");
        }
        Stats();
    }
    #endregion

    #region 능력치 표시 Stats() 조립실에서 현재 선택한 부품 혹은 이전에 저장된 정보가 있는경우 그정보를 입력시킨다
    /// <summary>
    /// 능력치 표시 beforePrfab 기준 이전에 저장된부품이나 현재 슬롯에서 선택한 부품이 있으면 능력치를 찾아서 넣어준다
    /// </summary>
    public void Stats()
    {
        if (DataManager.Instance.beforeLeg != null || DataManager.Instance.legPrefab != null)
        {
            DataManager.Instance.legtotalweight = DataManager.Instance.legInfo_List[DataManager.Instance.legindex].totalweight;
            DataManager.Instance.legspeed = DataManager.Instance.legInfo_List[DataManager.Instance.legindex].speed;
            DataManager.Instance.legamor = DataManager.Instance.legInfo_List[DataManager.Instance.legindex].amor;
        }
        if (DataManager.Instance.beforeBody != null || DataManager.Instance.bodyPrefab != null)
        {
            DataManager.Instance.bodyhp = DataManager.Instance.bodyInfo_List[DataManager.Instance.bodyindex].hp;
            DataManager.Instance.bodyamor = DataManager.Instance.bodyInfo_List[DataManager.Instance.bodyindex].amor;
            DataManager.Instance.bodytype = DataManager.Instance.bodyInfo_List[DataManager.Instance.bodyindex].bodytype;
            DataManager.Instance.bodyweight = DataManager.Instance.bodyInfo_List[DataManager.Instance.bodyindex].weight;
        }
        if (DataManager.Instance.beforeWeapon != null || DataManager.Instance.weaponPrefab != null)
        {
            DataManager.Instance.weaponweight = DataManager.Instance.weaponInfo_List[DataManager.Instance.weaponindex].weight;
            DataManager.Instance.weapontype = DataManager.Instance.weaponInfo_List[DataManager.Instance.weaponindex].weapontype;
            DataManager.Instance.weaponlange = DataManager.Instance.weaponInfo_List[DataManager.Instance.weaponindex].lange;
            DataManager.Instance.weaponattack = DataManager.Instance.weaponInfo_List[DataManager.Instance.weaponindex].attack;
        }

        if ((DataManager.Instance.beforeLeg != null && DataManager.Instance.beforeBody != null && DataManager.Instance.beforeWeapon != null) || 
            (DataManager.Instance.legPrefab != null && DataManager.Instance.bodyPrefab != null && DataManager.Instance.weaponPrefab != null))
        {
            if (DataManager.Instance.weaponweight + DataManager.Instance.bodyweight > DataManager.Instance.legtotalweight)
            {
                DataManager.Instance.isloadOver = true;
            }
            else
            {
                DataManager.Instance.isloadOver = false;
            }
        }

        if (DataManager.Instance.beforeBody != null && DataManager.Instance.beforeWeapon != null)
        {
            if (DataManager.Instance.weapontype.Equals(DataManager.Instance.bodytype))
            {
                DataManager.Instance.istypeNoeSame = false;
            }
            else
            {
                DataManager.Instance.istypeNoeSame = true;
            }
        }
    }
    #endregion

    #region 조립실 입장하고 호출 이전에 DB에 저장한 프리팹정보를 가져온다 RecallBeforePrefab()
    /// <summary>
    /// 저장되지 않은 상태로 조립실을 나갔을경우를 대비해 이전 프리팹 정보를 다시 가져와서 선택된 무기로 바꿔준다.
    /// </summary>
    public void RecallBeforePrefab()
    {
        DataManager.Instance.legPrefab = DataManager.Instance.beforeLeg;
        DataManager.Instance.bodyPrefab = DataManager.Instance.beforeBody;
        DataManager.Instance.weaponPrefab = DataManager.Instance.beforeWeapon;
    }
    #endregion

    #region 조립실의 ConfirmAssemble()에서 호출 부품 선택후 DB에 저장하면 프리팹을 현재 선택된 프리팹으로 바꿔준다 ChangeBeforePrefab()
    /// <summary>
    /// 저장되었을경우 조립실에서 선택해서 보여주던 legPrefab, bodyPrefab, weaponPrefab을 이전 정보로 변경한다.
    /// </summary>
    public void ChangeBeforePrefab()
    {
        DataManager.Instance.beforeLeg = DataManager.Instance.legPrefab;
        DataManager.Instance.beforeBody = DataManager.Instance.bodyPrefab;
        DataManager.Instance.beforeWeapon = DataManager.Instance.weaponPrefab;
    }
    #endregion
}
