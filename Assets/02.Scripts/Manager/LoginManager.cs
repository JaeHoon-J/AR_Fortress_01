using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;
#if GOOGLEGAMES
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
public class LoginManager : MonoBehaviour
{
    /*게임 오브젝트에 붙일 필요가 없을 때에는 monobehaviour를 상속 받을 필요가 없다.
     하지만 awake 만 어떻게 처리할 수 있다면 monobehaviour를 할 필요가 없을 수 있다.
     */
    protected static LoginManager instance =null;
    protected DataManager dataManager;
    protected DBManager dbManager;
    private void Awake()
    {
        instance = this;
#if GOOGLEGAMES
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    .AddOauthScope("profile")
    .RequestServerAuthCode(false)
    .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
#endif
    }

    public static LoginManager GetInstance()
    {
        if(instance == null)
        {
            GameObject obj = new GameObject(); //새로운 오브젝트
            obj.name = "LoginManager"; //오브젝트 이름 바꿔준다
           instance = obj.AddComponent<LoginManager>();//오브젝트에 스크립트를 붙이고 인스턴스에 넣는다
        }
        return instance;
    }
    private void Start()
    {
        //디비 메니져를 가져와야한다
        dbManager = DBManager.GetInstance();
        dataManager = DataManager.GetInstance();
    }

    #region 구글로그인 GoogleLogin()
    public void GoogleLogin()
    {
        print("GoogleLogin");
        Social.localUser.Authenticate((bool success, string msg) => {
            Debug.Log("Social.localUser.Authenticate : " + success);
            if (success)
            {
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                Debug.Log("Server Auth Code: " + serverAuthCode);

                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = serverAuthCode,
                    CreateAccount = true
                }, (result) =>
                {
                    //로그인 성공시 콜백 부분
                    dbManager.playfabid = result.PlayFabId;
                    Debug.Log("로그인 성공");
                    dbManager.GetInventory(); //로그인 성공하고 유저의 인벤토리 정보 바로 호출
                    dbManager.Getdata(dbManager.playfabid);//타이틀데이터 불러오기->타이틀데이터가 없으면 새로 생성해서 기본값 넣어주기
                    StartCoroutine(dbManager.LoadItemList());// 상점 리스트 불러와서 정리하고 완료되면 씬전환
                    //JoinLobby();

                    if (dbManager.onChangeMoneyDelegate != null) dbManager.onChangeMoneyDelegate(dataManager.myMoney);

                }, PlayFab_GoogleLogin_Error);
            }
            else
            {
                Debug.Log("Social.localUser.Authenticate : " + success);
                Debug.Log(msg);
            }

        });

    }

    private void PlayFab_GoogleLogin_Error(PlayFabError obj)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region 유니티 에디터 테스트 로그인용 TestLogin()
    /// <summary>
    /// 유니티 에디터에서 로그인되도록 테스트용으로 만들어 놓음
    /// </summary>
    public void TestLogin()
    {
        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFail);
    }
    /// <summary>
    /// 로그인 실패시 호출됨
    /// </summary>
    /// <param name="obj"></param>
    private void OnLoginFail(PlayFabError obj)
    {
        Debug.Log("로그인 실패");
    }
    /// <summary>
    /// 로그인 성공시 호출됨
    /// </summary>
    /// <param name="obj"></param>
    private void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log("로그인 성공");
        StartCoroutine(dbManager.LoadItemList());
        //로그인 성공하면 앞으로 유저의 정보가 수정될수 있으니 유저의 정보를 불러와서 userinfo에 담아서 가지고 있는다
        //JoinLobby();
        dbManager.GetInventory(); //로그인 성공하고 유저의 인벤토리 정보 바로 호출
        dbManager.Getdata(obj.PlayFabId);
    }
    #endregion
}
