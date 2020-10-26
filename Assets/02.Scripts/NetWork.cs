using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using GoogleARCore.Examples.CloudAnchors;
using System.Linq;
using PlayFab.GroupsModels;
#if GOOGLEGAMES
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class NetWork : MonoBehaviourPunCallbacks, IPunObservable
{

    private static NetWork m_Instance = null;
    public static NetWork Get { get { return m_Instance; } set { m_Instance = value; } }
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
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(gameObject);

    }
    #region PhotonNetwork
    [Header("포톤 관련 변수")]
    public int masterIndex;//마스터 클라이언트 인덱스
    public int[] actornums = new int[4];//방에 입장한 유저의 엑터 넘버를 저장할 배열
    public string[] userArr = new string[4]; //유저 이름을 저장하기위한 배열
    public bool[] userReady = new bool[4];//룸안에 유저가 게임 가능한 상태인지 여부 체크 false면 턴이 안돌아 온다(죽은 유저혹은 나간유저)
    public int myOrder = 99;// 내 입장 순서 체크
    public bool inIntro = true;//시작은 인트로부터 니까 true로
    public bool inLogin = false; //로그인 창 상태
    public bool inLobby = false;//로비 입장  상태
    public bool inRoom = false;//룸 입장 상태
    public bool isMaster = false;//방장인지 확인
    public bool finalCheck = false;//퍼미션 완료되면 true로 바뀜.
    public GameObject robtObj;//로봇객체가될 프리팹
    public GameObject testBot;//테스트용 로봇
    public int nowTurn; //현재 턴
    //public string anchorId;
    //public bool receiveId = false;//클라우드 앵커 아이디를 받았는지 체크
    public int readyCnt = 0;//방안에 몇명의 사용자가 클라우드 앵커를 생성 공유했는가 체크 방장제외최대 3까지 카운트
    public int receiveCnt;// 몇명이 클라우드 앵커 아이디를 전달받았는지 체크 방장제외최대 3까지 카운트
    public int localPlayer; //현재 방에 입장해 있는 유저의 수
    //public List<Anchor> anchorList = new List<Anchor>(); //플레이 씬에서 앵커 위치를 담을 리스트
    public List<AsyncTask<CloudAnchorResult>> hostingResultList = new List<AsyncTask<CloudAnchorResult>>();//클라우드 앵커 호스팅 된 결과가 담길 리스트
    public string anchorId;//앵커 아이디를 담을 변수
    private AsyncTask<CloudAnchorResult> task;//리졸브한 앵커의 결과를담을 변수
    public float z;//전방 움직임값
    public float x;//좌우 움직임값
    public bool isInput = false;//컨트롤러 값 입력되었는지 체크s
    public float rotateX;//x축 회전을 위한값
    public float rotateY;//y 축 회전을 위한값
    public bool isRotateUpDown = false;//x축 회전하고 있는지
    public bool isRotateLeftRight = false;//y축 회전하고 있는지
    //public string[] anchorIdArr;//앵커의 주소를 담을 배열
    private void Start()
    {
        if (inIntro == true)//인트로 상태에서 시작하고 연결안되있으면->처음 시작이면
        {
            inIntro = false;
            inLogin = true;
            inLobby = false;
            inRoom = false;
            //Connect();//연결하고
        }
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
    private void Update()
    {
        if (inRoom == true)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                isMaster = true;
            }
            else
            {
                isMaster = false;
            }
        }
        if (inRoom == true)
        {   //방에 입장했을때 현재 방의 유저수를 업데이트 시킨다.
            localPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
    /// <summary>
    /// 마스터 클라이언트 변경을 요구하기위함 
    /// 마스터 클라이언트를 변경하고 nowTurn을 현재 마스터클라이언트의엑터 넘버로 변경
    /// </summary>
    /// <param name="masterClientPlayer"></param>
    public void Call_ChangeMasterClient()
    {
       
        for(int i=0; i<PhotonNetwork.PlayerList.Length;i++)
        {
            //룸 사용자 배열에서 마스터 클라이언트가 몇번째 인덱스인지 찾는다
            if(PhotonNetwork.PlayerList[i].IsMasterClient==true)
            {
                masterIndex = i;
                Debug.Log("현재 마스터 클라이언트의 인덱스: " + i);
                Debug.Log("현재 마스터 클라이언트의 엑터 넘버" + PhotonNetwork.PlayerList[i].ActorNumber);
            }
        }
        if(masterIndex+1 ==PhotonNetwork.PlayerList.Length)
        {
            masterIndex = 0;
            ChangeMasterClient(PhotonNetwork.PlayerList[masterIndex]);
        }
        else
        {
            ChangeMasterClient(PhotonNetwork.PlayerList[masterIndex+1]);
        }
    }
    public void ChangeMasterClient(Photon.Realtime.Player masterClientPlayer)
    {
        if(inRoom ==true)
        {
        PhotonNetwork.SetMasterClient(masterClientPlayer);
            Debug.Log("새로운 마스터 클라이언트의 엑터 넘버"+masterClientPlayer.ActorNumber);
        }
    }
    public void Connect()
    {
        Debug.Log("마스터 서버 접속시도중.....");
        //마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }
    /// <summary>
    /// 마스터 서버 연결되면 호출됨
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 연결됨");

        Debug.Log("마스터 서버 연결상태 : " + inLogin);
        //로그인 화면으로 전환
        if (inLogin == true && finalCheck == true)
        {
            SceneManager.LoadScene("02.Login");
        }
    }
    /// <summary>
    /// 포톤 연결 끊겼을때
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김");
    }
    /// <summary>
    /// 호출하면 로비로
    /// </summary>
    public void JoinLobby()
    {
        Debug.Log("로비 입장시도");
        PhotonNetwork.JoinLobby();
    }
    /// <summary>
    /// 로비 입장 성공하면 호출됨
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("로비입장 성공");
        //룸에서 사용하는 변수들은 로비에 입장하면 전부 초기화 시켜준다 그래야 다시 룸에 들어갔을때 처음부터 사용할수 있다
        inIntro = false;
        inLogin = false;
        inLobby = true;
        inRoom = false;
        isMaster = false;
        //receiveId = false;
        receiveCnt = 0;
        myOrder = 99;
        readyCnt = 0;
        localPlayer = 0;
        Debug.Log(inLobby);
        //룸에 입장해서 사용할 배열들과 값들은 로비로 입장했을때 초기화가 되게한다.
        for (int i = 0; i < userArr.Length; i++)
        {
            userArr[i] = "empty";
            actornums[i] = -1;
        }
        if (inLobby == true)
        {
            SceneManager.LoadScene("03.Lobby");
        }
        //receiveId = false;
        receiveCnt = 0;
    }
    /// <summary>
    /// 방이름으로 방에 참가하기
    /// </summary>
    /// <param name="m_Roomname"></param>
    public void JoinRoom(string m_Roomname)
    {
        PhotonNetwork.JoinRoom(m_Roomname);
    }
    /// <summary>
    /// 방 이름 정하고 방생성하기
    /// </summary>
    /// <param name="m_Roomname"></param>
    public void CreateRoom(string m_Roomname)
    {
        //마스터 서버에 연결이 된상태에서만 방을 만들어야한다 연결상태인지 먼저 체크.
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("방 생성중...");
            //방옵션 세팅..
            RoomOptions roomOptions = new RoomOptions();
            //방을 리스트에 노출시킬지
            roomOptions.IsVisible = true;
            //공개방으로 설정
            roomOptions.IsOpen = true;
            //최대 입장인원
            roomOptions.MaxPlayers = 4;
            // 방 떠나면 방정보 날리기
            roomOptions.CleanupCacheOnLeave = true;
            //커스텀 프로퍼티 생성 엑터넘버를 저장할 배열
            roomOptions.CustomRoomProperties = new Hashtable() { { "roomMember", userArr }, { "actors", actornums }, { "userReady", userReady } };
            
            PhotonNetwork.CreateRoom(m_Roomname, roomOptions);

        }
        //마스터 서버에 접속이 안되있는경우 중간에 접속이 끝긴다든가 기타등등의 문제..
        else
        {
            Debug.Log("마스터 서버 연결 끝김 재접속중.....");
            //마스터 서버 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    /// <summary>
    /// 방입장에 실패했을때 호출됨.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패");
    }
    /// <summary>
    /// 방입장에 성공했을때 호출 방생성에 성공했을때도 호출
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 OR 생성 성공");
        inIntro = false;
        inLogin = false;
        inLobby = false;
        inRoom = true;
        Debug.Log(inRoom);
        //룸에 입장 성공하면 자신의 엑터 넘버 저장
        myOrder = PhotonNetwork.LocalPlayer.ActorNumber;
        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties; //현재 방의 커스텀 프로퍼티를 받아서 cp에 담음
        userArr = (string[])cp["roomMember"]; //룸의 커스텀 프로퍼티 roomMember를 useArr에 덮어씌운다
        actornums = (int[])cp["actors"];//룸의 커스텀 프로퍼티 actors를 배열 actornums에 덮어씌운다
        for (int i = 0; i < userArr.Length; i++)
        {
            //배열 크기만큼 반목문을 돌아서 배열에서 empty를찾고 내이름과 엑터 넘버 추가
            if (userArr[i] == "empty")
            {
                myOrder = i;
                userArr[i] = "testUser";//플레이어의 이름 Playfab에서 받아온 DisplayName을 넣을 예정
                actornums[i] = i;//나의 넘버 추가
                userReady[i] = true;
                break;
            }
        }
        //배열이 수정이 되었으니까 수정된 배열을 덮어씌우라는 명령을  RPC함수를 통해 룸안에 모든 사용자가 실행하게함
        photonView.RPC("UpdateRoomCustomProperty", RpcTarget.All, userArr, actornums, userReady);
        if (inRoom == true)
        {
            SceneManager.LoadScene("05.Room");
        }
    }
    /// <summary>
    /// 방에서 나가면 콜백으로 호출됨
    /// </summary>
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        print("OnLeftRoom");
        inRoom = false;
        SceneManager.LoadScene("03.Lobby");

    }
    //룸에 입장해서 커스텀 프로퍼티 변경을 요청하는 함수
    [PunRPC]
    public void UpdateRoomCustomProperty(string[] Arr, int[] actorarr, bool[] userready)
    {
        //RPC로 업데이트된 커스텀 프로퍼티를 다른유저에게도 뿌린다
        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties; //현재 방의 커스텀 프로퍼티를 받아서 CP에 담음
        cp["roomMember"] = Arr;//받은 매개변수로 현재 방의 커스텀 프로퍼티 수정
        cp["actors"] = actorarr;//받은 매개변수로 현재 방의 커스텀 프로퍼티 수정
        cp["userReady"] = userready;
        PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
        userArr = Arr;//매개변수로 받은 배열을 덮어쓰기
        actornums = actorarr;//매개변수로 받은 배열을 덮어쓰기
        userReady = userready;//매개변수로 받은 배열을 덮어쓰기
    }

    /// <summary>
    /// 룸에서 나감
    /// </summary>
    public void LeaveRoom()
    {
        Debug.Log("룸에서 퇴장중..");
        //방에서 퇴장하기 전에 cp["roomMember"] 과 cp["actors"] 에서 내정보 제거한다.
        for (int i = 0; i < userArr.Length; i++)
        {
            if (userArr[i] == name)
            {
                userArr[i] = "empty";
                actornums[i] = -1;
                userReady[i] = false;
            }
        }
        //변경된 배열을 방사용자에게 RPC로 변경하도록 명령하고
        photonView.RPC("UpdateRoomCustomProperty", RpcTarget.All, userArr, actornums, userReady);
        //퇴장
        PhotonNetwork.LeaveRoom();//방퇴장 ->로비로 돌아감
        //SceneManager.LoadScene("03.Lobby");
    }
    /// <summary>
    /// 방장이 시작을 누르면 다른 사용자와 함께 씬이동 autosynce를 사용할지 rpc로 사용할지 정해야함 아직 미정
    /// </summary>
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("06.PlayArcore");
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(z);
            stream.SendNext(x);
            stream.SendNext(myOrder);
            stream.SendNext(isInput);
            stream.SendNext(rotateX);
            stream.SendNext(rotateY);
            stream.SendNext(isRotateUpDown);
            stream.SendNext(isRotateLeftRight);
        }

        //클론이 통신을 받는 
        else
        {
            z = (float)stream.ReceiveNext();
            x = (float)stream.ReceiveNext();
            nowTurn = (int)stream.ReceiveNext();
            isInput = (bool)stream.ReceiveNext();
            rotateX = (float)stream.ReceiveNext();
            rotateY = (float)stream.ReceiveNext();
            isRotateUpDown = (bool)stream.ReceiveNext();
            isRotateLeftRight = (bool)stream.ReceiveNext();
        }
    }
    /// <summary>
    /// string 앵커 아이디를 룸안에 다른 사용자에게 보내기위한 rpc 함수를 호출함
    /// </summary>
    /// <param name="anchorid"></param>
    public void SendAnchorId(string anchorId)
    {
        photonView.RPC("RPC_SendAnchorId", RpcTarget.Others,anchorId);
    }
    /// <summary>
    /// 클라우드 앵커 아이디가 담긴 리스트 보내기
    /// </summary>
    /// <param name="anchorid"></param>
    [PunRPC]
    public void RPC_SendAnchorId(string Id)
    {
        Debug.Log("방장한테 앵커 아이디 받음");
        anchorId = Id;
        Debug.Log("앵커아이디 = " + Id);
        //receiveId = true;//앵커 아이디 보낸거 확인
        //아이디를 전달받고 마스터 클라이언트한테 받은것을 확인시켜주기위하여 receiveCnt 를 1증가하라는 명령을 내린다
        Call_ReceiveId();
    }
    /// <summary>
    /// 방장에게 receiveCnt 1증가하라고 시키는 rpc 함수 앵커 아이디를 전달 받앗을때 호출한다
    /// </summary>
    public void Call_ReceiveId()
    {
        photonView.RPC("ReceiveId", RpcTarget.MasterClient,myOrder);
    }
    [PunRPC]
    public void ReceiveId(int myorder)
    {
        //Debug.Log("방장한테 앵커아이디 받앗다고 알림");
        Debug.Log(myorder + "번째 플레이어가 앵커 아이디를 받았다고 알림");
        receiveCnt++;
    }
    /// <summary>
    /// 리졸브 한거를 체크해서 마스터 클라이언트한테 보내줌
    /// 마스터 클라이언트가 아닌 플레이어가 리졸브가 끝나면 호출함
    /// </summary>
    public void Call_CheckResolve(int myoder,int i)
    {
        //readyCnt++; 하는 함수
        photonView.RPC("CheckResolve", RpcTarget.MasterClient,myoder,i);
    }
    [PunRPC]
    public void CheckResolve(int myorder,int i)
    {
        Debug.Log(myorder+"번째플레이어가"+i + "번쨰 앵커 리졸브 했다고 알림");
        readyCnt++;
    }
    /// <summary>
    /// 방장이 아닌 사용자에게 리졸브를 하라고 rpc로 명령을 내린다
    /// </summary>
    /// <param name="id"></param>
    public void Call_ResolveRpc(int i)
    {
        //리졸브 명령은 방장말고 다른 사람만 하느거니까 알피씨 타겟 other
        //앵커 아이디는 NetWork 스크립트의 anchorId에 rpc로 받아 놓음
        photonView.RPC("ResolveRpc", RpcTarget.Others,i);
    }

    [PunRPC]
    public void ResolveRpc(int i)
    {
        StartCoroutine(ResolveCloudAnchor(i));
    }
    IEnumerator ResolveCloudAnchor(int i)
    {
        //Debug.Log(id);
        //task = XPSession.ResolveCloudAnchor(id);
        Debug.Log("전달 받은 앵커 아이디 = " + anchorId);
        Debug.Log(i+" 번째 앵커 리졸브중..");
        task = XPSession.ResolveCloudAnchor(anchorId);
        Debug.Log(task.Result.Response);
        yield return new WaitUntil(() => task.Result.Response == CloudServiceResponse.Success);
        yield return new WaitUntil(() => task.IsComplete == true);
        hostingResultList.Add(task);//결과를 담는다
        Debug.Log(task.Result.Response);
        Debug.Log(task.Result.Anchor);
        Debug.Log(task.Result.Anchor.CloudId);
        Debug.Log(" 전달받은 클라우드 앵커 위치 = " + task.Result.Anchor.transform.position);
        //다른사용자한테 클라우드 앵커를 생성했다고 알림
        Call_CheckResolve(myOrder,i);
        //yield return new WaitUntil(() => task.IsComplete);
        //obj = Instantiate(centerObject, task.Result.Anchor.transform.position, Quaternion.identity);//앵커위치에 생성하고
        //obj.transform.SetParent(task.Result.Anchor.transform);//부모로 설정
        //InstantePlayer(NetWork.Get.hostingResultList);
    }

    /// <summary>
    /// 플레이어를 각자 자기 위치에 생성하라고 명령을 내리는 rpc 함수 all로 호출해서 방장 자신도 포함해서 명령을 내린다
    /// </summary>
    public void Call_InstantePlayer()
    {
        //룸안의 사용자에게 자신의 로봇을 다른 사용자에게 생성하라는 RPC함수를 실행하게하는 함수를 RPC로 명령을 내림
        photonView.RPC("InstantePlayer", RpcTarget.All);
    }
    [PunRPC]
    public void InstantePlayer()
    {
        Debug.Log("나의 입장 순서는 = " + myOrder);
        Debug.Log("생성된 앵커의 숫자" + hostingResultList.Count);
        Call_MakeMyRobot(myOrder, GameManager.Get.beforeLeg.name, GameManager.Get.beforeBody.name, GameManager.Get.beforeWeapon.name);
        //Call_MakeTestBot(myOrder);
    }
    public void Call_MakeMyRobot(int actnum, string leg, string body, string weapon)
    {
        photonView.RPC("MakeMyRobot", RpcTarget.All, actnum, leg, body, weapon);
    }

    [PunRPC]
    public void MakeMyRobot(int actnum,string leg,string body,string weapon)
    {
        GameObject player = Instantiate(robtObj, hostingResultList[actnum + 1].Result.Anchor.transform.position, Quaternion.identity);
        player.GetComponent<Player>().legname = leg;
        player.GetComponent<Player>().bodyname = body;
        player.GetComponent<Player>().weaponname = weapon;
        player.GetComponent<Player>().actnum = actnum;
    }
    /// <summary>
    /// 테스트 봇 생성을 요구하는 알피씨 함수
    /// </summary>
    /// <param name="actnum"></param>
    public void Call_MakeTestBot(int actnum)
    {
        
        photonView.RPC("MakeTestBot", RpcTarget.All, actnum);
    }
    [PunRPC]
    public void MakeTestBot(int actnum)
    {
        Debug.Log(actnum + "번 유저의 테스트 봇을 생성하라고 요청옴");
        Instantiate(testBot, hostingResultList[actnum + 1].Result.Anchor.transform.position, Quaternion.identity);
    }
    
    #endregion





    #region Playfab
    [Header("플레이팹 변수")]
    public string email;//입력받은 이메일값 이메일 형식으로 @.com으로 적어
    public string password;//입력받은 비밀번호
    public string userName;//입력받은 사용자 이름
    public string nickName;//입력받은 닉네임
    public string buylastItem;//상점에서 방금 구입한 아이템 이름
    public bool modifyOk=false;
    public bool modifyFail = false;
    public int buyItemPice;//방금 구입한 아이템의 가격
    public int myMoney;//현재 내가 가진돈
    public string displayName;//플레이어 이름
    string playfabid;
    public List<CatalogItem> itemList = new List<CatalogItem>();
    //무기 리스트
    public List<CatalogItem> weaponList = new List<CatalogItem>();
    //몸통 리스트
    public List<CatalogItem> bodyList = new List<CatalogItem>();
    //다리 리스트
    public List<CatalogItem> legList = new List<CatalogItem>();
    /// <summary>
    /// 구글 로그인 하는 함수
    /// </summary>
    public void GoogleLogin()
    {
        print("GoogleLogin");
        Social.localUser.Authenticate((bool success, string msg) => {
            Debug.Log("Social.localUser.Authenticate : " +success);
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
                    playfabid = result.PlayFabId;
                    Debug.Log("로그인 성공");
                    GetInventory(); //로그인 성공하고 유저의 인벤토리 정보 바로 호출
                    Getdata(playfabid);//타이틀데이터 불러오기->타이틀데이터가 없으면 새로 생성해서 기본값 넣어주기
                    StartCoroutine(LoadItemList());// 상점 리스트 불러와서 정리하고 완료되면 씬전환
                    //JoinLobby();

                    if (onChangeMoneyDelegate != null) onChangeMoneyDelegate(myMoney);

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

    /// <summary>
    /// 회원가입
    /// </summary>
    public void Register()
    {
        var request = new PlayFab.ClientModels.RegisterPlayFabUserRequest { Email = email, Password = password, Username = userName, DisplayName = nickName };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegistFail);
    }

    private void OnRegistFail(PlayFabError obj)
    {
        Debug.Log("회원가입 실패");
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult obj)
    {
        Debug.Log("회원가입 성공");
        //회원가입성고앟면 생성된 플레이팹 고유의 아이디와 내가 입력한 이메일 패스워드 유저네임 닉네임을 클래스에 담기
        GameManager.Get.userinfo.playfabId = obj.PlayFabId;
        GameManager.Get.userinfo.nickName = nickName;
        SetData(GameManager.Get.userinfo);//유저의 정보를 세팅하는 값 설정하기
    }
    /// <summary>
    /// 커스텀 로그인 할떄 호출할것
    /// </summary>
    public void Login()
    {
        var request = new PlayFab.ClientModels.LoginWithEmailAddressRequest { Email = email, Password = password };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFail);
    }
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
        StartCoroutine(LoadItemList());
        //로그인 성공하면 앞으로 유저의 정보가 수정될수 있으니 유저의 정보를 불러와서 userinfo에 담아서 가지고 있는다
        //JoinLobby();
        GetInventory(); //로그인 성공하고 유저의 인벤토리 정보 바로 호출
        Getdata(obj.PlayFabId);
    }

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
        if(obj.Data.ContainsKey("Info"))
        {
            //가져올 정보가 있을때
            var yourObject = JsonUtility.FromJson<UserInfo>(obj.Data["Info"].Value);
            GameManager.Get.userinfo = yourObject;
            //선택된 부품 프리팹 가져오기
            GameManager.Get.SelecPrefabs();
        }
        else
        {
            SetData(GameManager.Get.userinfo);
            Debug.Log("유저 기본정보 세팅");
            //가져올 정보가 없을때 -> 기본 데이터 세팅한다 최초 로그인한 회원인 상태이다.
           
        }

        //키가 없다고 에러가 뜨면 -> 기본 정보 세팅
        
    }
    /// <summary>
    /// 가상화폐 얻기 한번 호출될때마다 100씩
    /// </summary>
    public void GetMoney(int amount)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = "GD" }, (result) =>
           {
               Debug.Log("돈 추가");
               myMoney = result.Balance;//현재 돈 변경
           },
        (error) => Debug.Log("돈 얻기 실패"));
    }
    /// <summary>
    /// 가상화폐 감소시키기 매개변수로 감소 시키고 싶은 양
    /// </summary>
    /// <param name="amount"></param>
    public void SubMoney(int amount)
    {
        PlayFabClientAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest { Amount = amount, VirtualCurrency = "GD" }, (result) =>
           {
               Debug.Log("돈 감소");
               myMoney = result.Balance;//현재 돈 변경
               if(myMoney <0)
               {
                   myMoney = 0;
               }
           },
        (error) => Debug.Log("돈 감소 실패"));
    }
    /// <summary>
    /// 인벤토리 정보 불러오기
    /// </summary>
    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new PlayFab.ClientModels.GetUserInventoryRequest(), (result) =>
        {
            Debug.Log("인벤토리 불러오기 성공");
            if(result.VirtualCurrency["GD"]<0)
            {
                myMoney = 0;
            }
            else
            {
            myMoney = result.VirtualCurrency["GD"]; //인벤토리 열면서 현재 돈 가져오기
            }
            //인벤토리 불러오기 성공하면 동작해야할것 작성....
            Debug.Log(result.VirtualCurrency);//가상화폐 종류별로  내가 가지고있는 잔액불러오기(배열) 
            for(int i=0; i<result.Inventory.Count;i++)
            {
                //인벤토리 리스트에 있는 아이템들의 각정보들
                var inventory = result.Inventory[i];
                GameManager.Get.ownedItem_List.Add(result.Inventory[i].DisplayName);
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

/// <summary>
/// 상점 불러오기 상점의 내용은 Playfab 홈페이지에서 설정할수 있다(아이템 가격 중첩여부 기간 등..)
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
            
            itemList = result.Catalog;
            //리스트 낮은 가격순으로 정렬
            //SortItemByPrice(itemList);
            for (int i=0; i<result.Catalog.Count;i++)
            {
                
                var catalog = result.Catalog[i];
                //태그로 비교한다.
                if(catalog.Tags[0] =="Weapon")
                {
                    weaponList.Add(catalog);//무기는 무기리스트에 추가
                    //아이템의 customData 불러오는 부분
                    //GetCustomIteminfo(catalog);
                }
                else if(catalog.Tags[0]=="LowerBody")
                {
                    legList.Add(catalog);//다리는 다리 리스트에 추가
                }
                else
                {
                    bodyList.Add(catalog);//그외에 것들은 몸통 리스트에 추가
                }
            }
            StartCoroutine("OrganizeItem_2");
        },
        (error) => Debug.Log("상점 불러오기 실패"));
    }
    /// <summary>
    /// 리스트에서 각 아이템의 상세 능력치 딕셔너리로 가져오기 + GameManager에 있는 아이템 상세정보리스트에 정보 넣어주기
    /// </summary>
    public void GetCustomIteminfo(CatalogItem item)
    {
        Debug.Log("아이템 세부 정보 받기");
        var iteminfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.CustomData);
       
        if (item.Tags[0]== "Weapon")
        {
            WeaponInfo info = new WeaponInfo();
            info.partName = item.DisplayName;//이름 넣기
            info.partType = item.Tags[0];//무기인지 몸인지 다리인지 널기
            info.weapontype = iteminfo["type"]; //무기가 어깨형인지 탑형인지 팔형인지
            info.attack = int.Parse(iteminfo["attack"]);
            info.weight = int.Parse(iteminfo["weight"]);
            info.lange = int.Parse(iteminfo["lange"]);
            GameManager.Get.weaponInfo_List.Add(info);
            Debug.Log("무기 정보 추가");
        }
        else if(item.Tags[0]== "LowerBody")
        {
            LegInfo info = new LegInfo();
            Debug.Log(item.DisplayName+"의 이동속도: " + iteminfo["speed"]);
            Debug.Log(item.DisplayName+"의 총 하중: " + iteminfo["totalweight"]);
            Debug.Log(item.DisplayName+"의 방어력: " + iteminfo["amor"]);
            info.partName = item.DisplayName;
            info.partType = item.Tags[0];
            info.totalweight = int.Parse(iteminfo["totalweight"]);
            info.speed = int.Parse(iteminfo["speed"]);
            info.amor = int.Parse(iteminfo["amor"]);
            GameManager.Get.legInfo_List.Add(info);
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
            GameManager.Get.bodyInfo_List.Add(info);
            Debug.Log("몸통 정보 추가");
        }
       
    }
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
    /// <summary>
    /// 상점에서 아이텝 구입
    /// </summary>
    /// <param name="storeName"></param> <=스토어 이름
    /// <param name="itemid"></param> <=아이템이 고유의 아이디 playfab 홈페이지에서 앙템 설정해줄때 생성됨 받아올수 아이템 정보를 받아올때 가져 올수 있음
    /// <param name="virtualCurrency"></param><=게임내 화폐의 종류 playfab 홈페이지에서 화폐 설정가능하다 
    /// <param name="price"></param><= 100원? 200원? 얼마나 게임내 화페를 사용할지 
    public void BuyItem(string storeName,string itemid, string virtualCurrency,int price)
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
    //델리게이트 
    public delegate void ChangeMoneyDelegate(int totalMoney);
    public ChangeMoneyDelegate onChangeMoneyDelegate;
    public int GetMyMoney() {
        return myMoney;
    }
    
    private void BuyOk(PurchaseItemResult obj)
    {
        Debug.Log("구입 성공");
        //GameManager의 보유 아이템 리스트(스트링타입)에 방금 구입한 아이템의 이름을 넣어준다
        GameManager.Get.ownedItem_List.Add(buylastItem);
        myMoney -= buyItemPice;//방금 구입한 아이템의 가격만큼 현재 가진돈을 빼준다
        if (onChangeMoneyDelegate != null) onChangeMoneyDelegate(myMoney);
    }
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
        GameManager.Get.userinfo.nickName = obj.DisplayName;
        GameManager.Get.userinfo.firstLogin = false;
        SetData(GameManager.Get.userinfo);
        Debug.Log("변경된 이름은 ; " + obj.DisplayName);
        modifyOk = true;

    }
    #endregion

    #region 상점아이템 받아오기 -> 아이템 가격순 정렬->아이템별 상세 정보 담아서 리스트 만들기
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
        SortItemByPrice(itemList); //전체아이템 리스트 가격 낮은 순으로 정렬
        SortItemByPrice(weaponList);//무기리스트 가격낮은 순으로 정렬
        SortItemByPrice(bodyList);//몸통 리스트 가격낮은 순으로 정렬
        SortItemByPrice(legList);//다리 리스트 가격낮은 순으로 정렬

        StartCoroutine("OrganizeItem_3");
    }
    /// <summary>
    /// 각 아이템별 상세 정보를 담는다.
    /// GameManaget 스크립트의 weaponInfo_List ,bodyInfo_List, legInfo_Listdp 에 정보를 담기 위함
    /// </summary>
    /// <returns></returns>
    public IEnumerator OrganizeItem_3()
    {
        yield return null;
        Debug.Log("OrganizeItem_3");
        //GetCustomIteminfo 함수는 리스트에 있는 아이템들의 이름 타입 커스텀 데이터를 받아와서
        //GameManager에 있는 무기.몸,다리 상세정보리스트에 정보를 담는다.
        for (int i = 0; i < weaponList.Count; i++)
        {
            GetCustomIteminfo(weaponList[i]);
        }
        for (int i = 0; i < bodyList.Count; i++)
        {
            GetCustomIteminfo(bodyList[i]);
        }
        for (int i = 0; i < legList.Count; i++)
        {
            GetCustomIteminfo(legList[i]);
        }
        yield return new WaitUntil (()=>GameManager.Get.legInfo_List.Count == 7);
        //무기 정보 리스트가 차면->기존에 조립된 것이 있다면 바로 정보를 찾아서 불러온다
        GameManager.Get.BeforeStats();
        StartCoroutine("MoveToLobby");
    }
    public IEnumerator MoveToLobby()
    {
        yield return null;
        JoinLobby();
    }

    #endregion
}




