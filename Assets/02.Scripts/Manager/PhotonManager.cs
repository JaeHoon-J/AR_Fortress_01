using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;

public class PhotonManager : MonoBehaviourPunCallbacks, IPunObservable
{
    protected static PhotonManager instance = null;
    //------------------------------------------
    public static PhotonManager Instance
    {
        get
        {
                return instance;
        }
    }
    //-------------------------------------------
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);
    }
    /*public static PhotonManager GetInstance()
    {
        Debug.Log("PhotonManager 인스턴스 가져오기");
        if(instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = "PhotonManager";
            obj.AddComponent<PhotonView>();
            instance = obj.AddComponent<PhotonManager>();
        }
        return instance;
    }*/
    DataManager dataManager;
    [Header("포톤 관련 변수")]
    public int masterIndex;//마스터 클라이언트 인덱스
    public int[] userArr = new int[4]; //유저 이름을 저장하기위한 배열
    public bool[] userReady = new bool[4];//룸안에 유저가 게임 가능한 상태인지 여부 체크 false면 턴이 안돌아 온다(죽은 유저혹은 나간유저)
    public int myOrder = 99;// 내 입장 순서 체크 포톤의 엑터 넘버를 부여함
    public bool inIntro = true;//시작은 인트로부터 니까 true로
    public bool inLogin = false; //로그인 창 상태
    public bool inLobby = false;//로비 입장  상태
    public bool inRoom = false;//룸 입장 상태
    public bool isMaster = false;//방장인지 확인
    public bool finalCheck = false;//퍼미션 완료되면 true로 바뀜.
    public GameObject robtObj;//로봇객체가될 프리팹
    //public GameObject testBot;//테스트용 로봇
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
    public float lange; //미사일 게이지
    public bool isFireCheck = false;
    public bool firstAnchor = false;//최초 앵커가 생성되었는지 체크
                                    //public string[] anchorIdArr;//앵커의 주소를 담을 배열
    public delegate void SendMyInfo(int admissionOrder, string nickName, string legName, string bodyName, string weaponName);
    public SendMyInfo sendMyInfo;
    public delegate void DeleteMyInfo(int admissionOrder);
    public DeleteMyInfo deleteMyInfo;
    public int slotOrder; //몇번째 슬롯에 내 정보가 표시되야 하는가?
    private void Start()
    {
        dataManager = DataManager.Instance;
        if (inIntro == true)//인트로 상태에서 시작하고 연결안되있으면->처음 시작이면
        {
            inIntro = false;
            inLogin = true;
            inLobby = false;
            inRoom = false;
            //Connect();//연결하고
        }

    }
    private void Update()
    {
        if (inRoom)
        {
            isMaster = PhotonNetwork.IsMasterClient;
            //방에 입장했을때 현재 방의 유저수를 업데이트 시킨다.
            localPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        }
}
    #region 마스터 클라이언트 교체 Call_ChangeMasterClient()
    /// <summary>
    /// 마스터 클라이언트 변경을 요구하기위함 
    /// 마스터 클라이언트를 변경하고 nowTurn을 현재 마스터클라이언트의엑터 넘버로 변경
    /// </summary>
    /// <param name="masterClientPlayer"></param>
    public void Call_ChangeMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)//메소드 호출자가 마스터 클라이언트면? 방장만 이 것을 들어오게 하려는 것임.
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                //룸 사용자 배열에서 마스터 클라이언트가 몇번째 인덱스인지 찾는다
                if (PhotonNetwork.PlayerList[i].IsMasterClient)
                {
                    masterIndex = i;
                    Debug.Log("현재 마스터 클라이언트의 인덱스: " + i);
                    Debug.Log("현재 마스터 클라이언트의 엑터 넘버" + PhotonNetwork.PlayerList[i].ActorNumber);
                }
            }
            if ((masterIndex + 1)== PhotonNetwork.PlayerList.Length)
            {
                masterIndex = 0;
                ChangeMasterClient(masterIndex);
            }
            else
            {
                ChangeMasterClient(masterIndex + 1);
            }
        }
    }
    /// <summary>
    /// index = 제어권을 가질 마스터 플레이어가 PlayerLisf에서 몇번째 인덱스인지의 값
    /// </summary>
    /// <param name="index"></param>
    public void ChangeMasterClient(int index)
    {
        if (inRoom == true)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[index]);
            Debug.Log("현재 마스터 플레이어의 인덱스 " + index);
            Debug.Log("새로운 마스터 클라이언트의 엑터 넘버" + PhotonNetwork.PlayerList[index].ActorNumber);
            //nowTurn을 방장의 엑터 넘버로 바꿔준다
            Call_ChangeTurn(PhotonNetwork.PlayerList[index].ActorNumber);
        }
    }
    #endregion

    #region 포톤연결시도 Connect()
    /// <summary>
    /// 포톤 연결을 시도하는 함수
    /// </summary>
    public void Connect()
    {
        Debug.Log("마스터 서버 접속시도중.....");
        //마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region Connect()에 대한 콜백 OnConnectedToMaster()
    /// <summary>
    /// 마스터 서버 연결되면 호출됨 
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 연결됨");

        Debug.Log("마스터 서버 연결상태 : " + inLogin);
        //퍼미션 체크되어 있고 로그인씬으로 로그인을 해도 되면?
        if (inLogin == true && finalCheck == true)
        {
            SceneManager.LoadScene("02.Login");
        }
    }
    #endregion

    #region 포톤연결이 끊겼을때 콜백 OnDisconnected(DisconnectCause cause)
    /// <summary>
    /// 포톤 연결 끊겼을때
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김");
        Debug.Log("마스터 서버 다시연결중....");
        Connect();
    }
    #endregion

    #region 로비로 입장을 시도한다 JoinLobby()
    /// <summary>
    /// 호출하면 로비로
    /// </summary>
    public void JoinLobby()
    {
        Debug.Log("로비 입장시도");
        PhotonNetwork.JoinLobby();
    }
    #endregion

    #region 로비 입장에 성공했을때 콜백 OnJoinedLobby()
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
            userArr[i] = 0;
        }
        if (inLobby == true)
        {
            SceneManager.LoadScene("03.Lobby");
        }
        //receiveId = false;
        receiveCnt = 0;
    }
    #endregion

    #region 방에이름찾아서 참가하기 JoinRoom(string m_Roomname)
    /// <summary>
    /// 방이름으로 방에 참가하기
    /// </summary>
    /// <param name="m_Roomname"></param>
    public void JoinRoom(string m_Roomname)
    {
        PhotonNetwork.JoinRoom(m_Roomname);
    }
    #endregion

    #region 방생성하기 CreateRoom(string m_Roomname)
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
            roomOptions.CustomRoomProperties = new Hashtable() { { "roomMember", userArr }, { "userReady", userReady } };

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
    #endregion

    #region 방입장 실패시 콜백됨 OnJoinRoomFailed(short returnCode, string message)
    /// <summary>
    /// 방입장에 실패했을때 호출됨.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패");
    }
    #endregion

    #region 방생성이나 방입장에 성공했을때 콜백됨 OnJoinedRoom()
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
        userArr = (int[])cp["roomMember"]; //룸의 커스텀 프로퍼티 roomMember를 useArr에 덮어씌운다

        for (int i = 0; i < userArr.Length; i++)
        {
            //배열 크기만큼 반목문을 돌아서 배열에서 0 를찾고 내이름과 엑터 넘버 추가
            if (userArr[i] == 0)
            {
                //myOrder = i;
                nowTurn = PhotonNetwork.PlayerList[0].ActorNumber;
                myOrder = PhotonNetwork.LocalPlayer.ActorNumber;//엑터넘버를 순서로
                userArr[i] = myOrder;//엑터넘버를 넣을 예정
                userReady[i] = true;
                slotOrder = i;
                break;
            }
        }
        //배열이 수정이 되었으니까 수정된 배열을 덮어씌우라는 명령을  RPC함수를 통해 룸안에 모든 사용자가 실행하게함
        photonView.RPC("UpdateRoomCustomProperty", RpcTarget.All, userArr, userReady);
        if (inRoom == true)
        {
            SceneManager.LoadScene("05.Room");
        }
       
    }
    #endregion

    #region OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) 다른 사용자가 룸에 들어왔을때 호출
    /// <summary>
    /// 다른 사용자가 룸에 들어왔을때 호출
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //새로운 플레이어가 룸에 입장할때마다 콜백되는 함수이다
        //새로운 유저가 방에 들어올때마다 내정보를 몇번 슬롯에 표시해줘야할지 쏴준다
        //다른 플레이어가 씬전화되서 자신의 정보를 쏴줄때까지 대기후 내 정보를 날린
        //Call_SendMyInfoInWaitingRoom(slotOrder, dataManager.userinfo.nickName, dataManager.userinfo.selectedLeg,
        //    dataManager.userinfo.selectedBody, dataManager.userinfo.selectedWeapon);
    }
    #endregion

    #region 방에서 나가기 LeaveRoom()는 PhotonNetwork.LeaveRoom()를 호출함
    /// <summary>
    /// 룸에서 나감
    /// </summary>
    public void LeaveRoom()
    {
        Debug.Log("룸에서 퇴장중..");
        firstAnchor = false;
        //방에서 퇴장하기 전에 cp["roomMember"] 과 cp["actors"] 에서 내정보 제거한다.
        for (int i = 0; i < userArr.Length; i++)
        {
            if (userArr[i] == myOrder)
            {
                userArr[i] = 0;
                userReady[i] = false;
            }
        }
        //변경된 배열을 방사용자에게 RPC로 변경하도록 명령하고
        photonView.RPC("UpdateRoomCustomProperty", RpcTarget.All, userArr, userReady);
        Call_DeleteMyInfoWaitingRoom(slotOrder);// 룸에서 나가면 대기방에서 슬롯의 정보도 비워준다
        //퇴장
        PhotonNetwork.LeaveRoom();//방퇴장 ->로비로 돌아감
        //SceneManager.LoadScene("03.Lobby");
    }
    #endregion

    #region 방에서 나가면 콜백됨 PhotonNetwork.LeaveRoom()호출데 대한 콜백 OnLeftRoom()
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
    #endregion

    #region 룸에 입장했을때 룸커스텀 프로퍼티 변경을위해 호출됨 OnJoinedRoom에서 호출되는 RPC
    //룸에 입장해서 커스텀 프로퍼티 변경을 요청하는 함수
    [PunRPC]
    public void UpdateRoomCustomProperty(int[] Arr,  bool[] userready)
    {
        //RPC로 업데이트된 커스텀 프로퍼티를 다른유저에게도 뿌린다
        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties; //현재 방의 커스텀 프로퍼티를 받아서 CP에 담음
        cp["roomMember"] = Arr;//받은 매개변수로 현재 방의 커스텀 프로퍼티 수정
        cp["userReady"] = userready;
        PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
        userArr = Arr;//매개변수로 받은 배열을 덮어쓰기
        userReady = userready;//매개변수로 받은 배열을 덮어쓰기
    }
    #endregion

    #region 게임 시작 StartGame() 동기화해서 씬을 전환한다
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
    #endregion

    #region  IPunObservable 인터페이스 구현부분
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(z);
            stream.SendNext(x);
            //stream.SendNext(myOrder);
            stream.SendNext(isInput);
            stream.SendNext(rotateX);
            stream.SendNext(rotateY);
            stream.SendNext(isRotateUpDown);
            stream.SendNext(isRotateLeftRight);
            //stream.SendNext(lange);
        }

        //클론이 통신을 받는 
        else
        {
            z = (float)stream.ReceiveNext();
            x = (float)stream.ReceiveNext();
            //nowTurn = (int)stream.ReceiveNext();
            isInput = (bool)stream.ReceiveNext();
            rotateX = (float)stream.ReceiveNext();
            rotateY = (float)stream.ReceiveNext();
            isRotateUpDown = (bool)stream.ReceiveNext();
            isRotateLeftRight = (bool)stream.ReceiveNext();
            //lange = (float)stream.ReceiveNext();
        }
    }
    #endregion

    #region 앵커아이디를 다른 사용자에게 보내기위한 RPC메소드를 호출하는 SendAnchorId(string anchorId)
    /// <summary>
    /// string 앵커 아이디를 룸안에 다른 사용자에게 보내기위한 rpc 함수를 호출함
    /// </summary>
    /// <param name="anchorid"></param>
    public void SendAnchorId(string anchorId)
    {
        photonView.RPC("RPC_SendAnchorId", RpcTarget.Others, anchorId);
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
    #endregion

    #region 앵커아이디를 전달받은 방장이 아닌 사용자가 호출하는 함수 Call_ReceiveId()
    /// <summary>
    /// 방장에게 receiveCnt 1증가하라고 시키는 rpc 함수 앵커 아이디를 전달 받앗을때 호출한다
    /// </summary>
    public void Call_ReceiveId()
    {
        photonView.RPC("ReceiveId", RpcTarget.MasterClient, myOrder);
    }
    [PunRPC]
    public void ReceiveId(int myorder)
    {
        //Debug.Log("방장한테 앵커아이디 받앗다고 알림");
        Debug.Log(myorder + "번째 플레이어가 앵커 아이디를 받았다고 알림");
        receiveCnt++;
    }
    #endregion

    #region 아이디로 앵커 리졸브해쓰면 방장한테 알려주기위한 RPC 메소드  Call_CheckResolve(int myoder, int i)
    /// <summary>
    /// 리졸브 한거를 체크해서 마스터 클라이언트한테 보내줌
    /// 마스터 클라이언트가 아닌 플레이어가 리졸브가 끝나면 호출함
    /// </summary>
    public void Call_CheckResolve(int myoder, int i)
    {
        //readyCnt++; 하는 함수
        photonView.RPC("CheckResolve", RpcTarget.MasterClient, myoder, i);
    }
    [PunRPC]
    public void CheckResolve(int myorder, int i)
    {
        Debug.Log(myorder + "번째플레이어가" + i + "번쨰 앵커 리졸브 했다고 알림");
        readyCnt++;
    }
    #endregion

    #region 방장이 다른사람들에게  리졸브 하라고 명령을 내리는 RPC Call_ResolveRpc(int i)
    /// <summary>
    /// 방장이 아닌 사용자에게 리졸브를 하라고 rpc로 명령을 내린다
    /// 매개변수 i는 현재 리졸브하는게 몇번째 앵커인지 앵커를 가리키는 인덱스
    /// </summary>
    /// <param name="id"></param>
    public void Call_ResolveRpc(int i)
    {
        //리졸브 명령은 방장말고 다른 사람만 하느거니까 알피씨 타겟 other
        //앵커 아이디는 NetWork 스크립트의 anchorId에 rpc로 받아 놓음
        photonView.RPC("ResolveRpc", RpcTarget.Others, i);
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
        Debug.Log(i + " 번째 앵커 리졸브중..");
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
        Call_CheckResolve(myOrder, i);
        //yield return new WaitUntil(() => task.IsComplete);
        //obj = Instantiate(centerObject, task.Result.Anchor.transform.position, Quaternion.identity);//앵커위치에 생성하고
        //obj.transform.SetParent(task.Result.Anchor.transform);//부모로 설정
        //InstantePlayer(NetWork.Get.hostingResultList);
    }
    #endregion

    #region 로븟을 생성하라고 방장이 명령을 내리는 RPC Call_InstantePlayer()
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
        //플레이 생성 명령이 방장으로 부터 떨어지면 내 로봇을 생성하고 이명령을 다른사용자에게도 내 로봇을 이위치에 생성해라 라고 명령을 전달
        Call_MakeMyRobot(myOrder, DataManager.Instance.beforeLeg.name, DataManager.Instance.beforeBody.name, DataManager.Instance.beforeWeapon.name);
        //Call_MakeTestBot(myOrder);
    }
    #endregion

    #region 자신의 로봇을 생성하면서 다른 사용자에게 해당앵커위치에 로봇을 생성하라고 명령을 내리는 RPC를 호출 InstantePlayer()부분에서 호출
    /// <summary>
    /// 나의 포톤 엑터넘버와 다리 몸 무기의 이름을 매개변수로 넘겨준다
    /// </summary>
    /// <param name="actnum"></param>
    /// <param name="leg"></param>
    /// <param name="body"></param>
    /// <param name="weapon"></param>
    protected int posnum;
    public void Call_MakeMyRobot(int actnum, string leg, string body, string weapon)
    {
        
        for(int i=0;i <PhotonNetwork.PlayerList.Length;i++)
        {
            if(PhotonNetwork.PlayerList[i].ActorNumber == actnum)
            {
                posnum = i;
            }
        }
        //actnum ->num
        //배열에서 내가 몇번째 인덱스인지 찾아서 매개변수로 넘김
        photonView.RPC("MakeMyRobot", RpcTarget.All, posnum, leg, body, weapon);
        
    }

    [PunRPC]
    public void MakeMyRobot(int actnum, string leg, string body, string weapon)
    {
        GameObject player = Instantiate(robtObj, hostingResultList[actnum + 1].Result.Anchor.transform.position, Quaternion.identity);
        player.GetComponent<Player>().legname = leg;
        player.GetComponent<Player>().bodyname = body;
        player.GetComponent<Player>().weaponname = weapon;
        player.GetComponent<Player>().actnum = PhotonNetwork.PlayerList[actnum].ActorNumber;
        Debug.Log((actnum+1)+"플레이어 로봇 엑터 넘버" + PhotonNetwork.PlayerList[actnum].ActorNumber);
    }
    #endregion

    #region Call_SendShootValue(bool check,float _lange) 마사일 발사시 나의 게이지 정보를 다른사용자에게 동기화
    /// <summary>
    /// 불값 파워게이지를 다른 사용자에게 보낸다
    /// </summary>
    /// <param name="check"></param>
    public void Call_SendShootValue(bool check,float _lange)
    {
        photonView.RPC("SendShootValue", RpcTarget.All, check,_lange);
    }
    [PunRPC]
    public void SendShootValue(bool check,float _lange)
    {
        isFireCheck = check;
        lange = _lange;
    }
    #endregion

    #region 중심앵커가 생겼음을 룸안 사용자들에게 알림 Call_MakeFirstAnchor()
    /// <summary>
    /// arcoreManager에서 firstAnchor 불값을 변경하기위한 rpc
    /// </summary>
    public void Call_MakeFirstAnchor()
    {
        photonView.RPC("MakeFirstAnchor", RpcTarget.All);
    }
    [PunRPC]
    public void MakeFirstAnchor()
    {
        firstAnchor = true;
    }
    #endregion

    #region Call_ChangeTurn(int num) 변경된 턴을 다른사용자들에게도 알려줌 Call_ChangeMasterClient()에서 마지막에 호출
    /// <summary>
    /// 턴을 변경했다는 것을 알리는 함수 턴변경시에 호출
    /// </summary>
    /// <param name="num"></param>
    public void Call_ChangeTurn(int num)
    {
        photonView.RPC("ChangeTurn", RpcTarget.All, num);
    }
    [PunRPC]
    public void ChangeTurn(int num)
    {
        //포톤 플레이어 배열에서 몇번 인덱스 플레이어의 턴인가
        nowTurn = num;//입장순서는 0번부터니까 엑터 넘버는 1부터시작이므로 1을빼준다
    }
    #endregion

    #region Call_SendMyInfoInWaitingRoom 델리게이트를 호출함 룸입장시 나의 정보를 다른사람에게도 룸 로비에서 보여주기위함
    /// <summary>
    /// 내정보를 룸입장시에 다른 사용자에게 보내는 RPC
    /// </summary>
    /// <param name="admissionOrder"></param>
    /// <param name="nickName"></param>
    /// <param name="legName"></param>
    /// <param name="bodyName"></param>
    /// <param name="weaponName"></param>
    public void Call_SendMyInfoInWaitingRoom(int admissionOrder, string nickName, string legName, string bodyName, string weaponName)
    {
        Debug.Log("Call_SendMyInfoInWaitingRoom RPC 호출");
        //내정보를 다른 사람들한테 뿌림
        photonView.RPC("SendMyInfoInWaitingRoom", RpcTarget.All, admissionOrder, nickName, legName, bodyName, weaponName);
        //내 정보는 뿌렸으니 너네 정보를 줘라
        Call_RequestOtherUsersInfo();
    }
    [PunRPC]
    public void SendMyInfoInWaitingRoom(int admissionOrder, string nickName, string legName, string bodyName, string weaponName)
    {
        Debug.Log(admissionOrder+"번째의 정보를 전달 받음");
        if(sendMyInfo !=null)
        {
        sendMyInfo(admissionOrder, nickName, legName, bodyName, weaponName);
        }
        else
        {
            Debug.Log("sendMyInfo ==null");
        }
       
    }
    /// <summary>
    /// 다른사람에게 요청을 보냄
    /// </summary>
    public void Call_RequestOtherUsersInfo()
    {
        Debug.Log("다른 사용자의 정보를 요청하였다");
        photonView.RPC("RequestOtherUsersInfo", RpcTarget.Others);
    }
    [PunRPC]
    public void RequestOtherUsersInfo()
    {
        Debug.Log("내정보를 보내달라는 요청이 왔다");
        photonView.RPC("RespondForRequest", RpcTarget.Others, slotOrder, dataManager.userinfo.nickName,
            dataManager.userinfo.selectedLeg, dataManager.userinfo.selectedBody, dataManager.userinfo.selectedWeapon);
    }
    //내정보를 요청 했으니 응답하고 날려준다
    [PunRPC]
    public void RespondForRequest(int admissionOrder, string nickName, string legName, string bodyName, string weaponName)
    {
        Debug.Log("요청에 응답해 내정보를 보낸다");
        if (sendMyInfo != null)
        {
            sendMyInfo(admissionOrder, nickName, legName, bodyName, weaponName);
        }
        else
        {
            Debug.Log("sendMyInfo ==null");
        }
    }
    #endregion

    #region Call_DeleteMyInfoWaitingRoom(int admissionOrder) 대기방에서 나갈때 대기방에서 내정보를 지우도록 하는것
    /// <summary>
    /// 룸에서 퇴장시에 나의 정보를 없에고 다른 사람의 화면에도 적용되도록 delegate 호출
    /// deleteMyInfo는 RoomScene에서 연결되어 있음 내용은 RoomScene.cs 120 번째 줄
    /// </summary>
    /// <param name="admissionOrder"></param>
    public void Call_DeleteMyInfoWaitingRoom(int admissionOrder)
    {
        photonView.RPC("DeleteMyInfoWaitingRoom", RpcTarget.All, admissionOrder);
    }
    [PunRPC]
    public void DeleteMyInfoWaitingRoom(int admissionOrder)
    {
        if(deleteMyInfo !=null)
        {
            deleteMyInfo(admissionOrder);
        }
        else
        {
            Debug.Log("deleteMyInfo ==null");
        }
    }
    #endregion
    //#region 테스트용 봇 생성
    ///// <summary>
    ///// 테스트 봇 생성을 요구하는 알피씨 함수
    ///// </summary>
    ///// <param name="actnum"></param>
    //public void Call_MakeTestBot(int actnum)
    //{

    //    photonView.RPC("MakeTestBot", RpcTarget.All, actnum);
    //}
    //[PunRPC]
    //public void MakeTestBot(int actnum)
    //{
    //    Debug.Log(actnum + "번 유저의 테스트 봇을 생성하라고 요청옴");
    //    Instantiate(testBot, hostingResultList[actnum + 1].Result.Anchor.transform.position, Quaternion.identity);
    //}
    //#endregion


}
