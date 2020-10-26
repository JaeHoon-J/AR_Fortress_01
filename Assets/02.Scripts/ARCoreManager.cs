using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using GoogleARCore.Examples.CloudAnchors;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using System.Collections;


public class ARCoreManager: MonoBehaviourPunCallbacks
{
    
    //Camera ARCamera;
    //public GameObject centerObject;
    //public Text text;
    public GameObject firstTouch;
    public bool firstAnchor = false;
    private TrackableHit hit;
    public Anchor anchor;// 생성한 로컬 앵커
    public string anchorid;//생성된 클라우드 앵커 아이디값
    private AsyncTask<CloudAnchorResult> task;
    private GameObject obj;
    private Pose pos; //  생성할 위치
    private Pose firstpos;//방장이 처음 터치한 위치
    public int nowPlayerCnt;//현재 방에 입장한 플레이어수
    public float DistanceFromCenter; //중심점으로 부터의 거리
    public bool ismakeAnchor = false;
    public Text idreceiveCnt;
    public Text resolveCnt;
    public Text nowPlayer;
    [SerializeField]
    protected Text checkMasterText;
    [SerializeField]
    protected Text nowPower;

    private void Start()
    {
       
        DistanceFromCenter = 0.5f;
        //ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();

    }

    private void Update()
    {

        if(PhotonManager.Instance.isMaster == true)
        {
            idreceiveCnt.text = "앵커 아이디 전달받은 플레이어수: "+PhotonManager.Instance.receiveCnt.ToString();
            resolveCnt.text = "앵커 리졸브한 플레이어수: "+PhotonManager.Instance.readyCnt.ToString();
            nowPlayer.text = "현재 방에 입장한 플레이어수: " + PhotonManager.Instance.localPlayer.ToString();
            nowPower.text = "현재 게이지 : " + PhotonManager.Instance.lange.ToString();
            checkMasterText.text = "내 액터 넘버"+PhotonManager.Instance.myOrder.ToString()+"현재턴은"+PhotonManager.Instance.nowTurn.ToString();
        }
        else
        {
            checkMasterText.text = "내 엑터 넘버" + PhotonManager.Instance.myOrder.ToString() + " 현재턴은" + PhotonManager.Instance.nowTurn.ToString();
        }//지울 예정
        
        if (Input.touchCount==0)
        {
            return;
        }
        MakePosition();
    }
    public void MakePosition()
    {
        StartCoroutine(MakeCloudAnchorMaster());
    }
    /// <summary>
    /// 방장이 먼저 앵커를 만든다.
    /// 앵커를 호스팅한다.
    /// 다음 사람은 리솔브한다?
    /// </summary>

    public IEnumerator MakeCloudAnchorMaster()
    {
        Touch touch = Input.GetTouch(0);
        TrackableHitFlags racastFilter = TrackableHitFlags.PlaneWithinPolygon; //평면만 검출
        if(PhotonManager.Instance.firstAnchor == false && PhotonManager.Instance.isMaster == true)//처음 앵커는 방장만 찍게 한다.
        {
            if (touch.phase == TouchPhase.Began && Frame.Raycast(touch.position.x, touch.position.y, racastFilter, out hit))
            {

                PhotonManager.Instance.Call_MakeFirstAnchor(); //다른사용자에게 도 firstAnchor의 값을 true로 바꿔줌
                //firstAnchor = true;//값을 true 로 해서 이함수가 2번 실행되지 않게 한다? 터치2번못하게 막는다
                firstpos = hit.Pose;
                pos = hit.Pose;//터치한 지점을 담는다.
                Instantiate(firstTouch, pos.position, Quaternion.identity);
                //anchor = hit.Trackable.CreateAnchor(hit.Pose);//터치한 지점에 앵커를 만든다.
                //localAnchor.Add(hit.Trackable.CreateAnchor(pos));
                //Debug.Log("중심점 설정");
                //유저수 +1 만큼 앵커를 만들고 싶음 유저수 +1만큼 반복문을 돈다
                nowPlayerCnt = PhotonManager.Instance.localPlayer;
                Debug.Log("현재 방안의 플레이어수 : " + nowPlayerCnt);
                for(int i=0; i<nowPlayerCnt+1;i++)
                {
                    ismakeAnchor = false;
                    PhotonManager.Instance.receiveCnt = 0;//몇명이 클라우드 앵커 아이디를 받았는가 0 으로 초기화 하고 시작
                    PhotonManager.Instance.readyCnt = 0; //몇명이 클라우드 앵커를 생성했는가 0으로 초기화 하고 시작
                    if (i == 0) //센터 위치
                    {
                        anchor = hit.Trackable.CreateAnchor(hit.Pose);//터치한 지점에 앵커를 만든다.
                        Debug.Log("중심점이 될 앵커 생성");
                    }
                    else if(i ==1) //1번 사용자의 위치
                    {
                        pos.position = firstpos.position+new Vector3(DistanceFromCenter, 0, 0);
                        anchor = hit.Trackable.CreateAnchor(pos); //1번 플레이어의 앵커를만든다.
                        Debug.Log("1번 플레이어의 위치 앵커 생성");
                    }
                    else if(i==2)//2번 사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(-DistanceFromCenter, 0, 0);
                        anchor = hit.Trackable.CreateAnchor(pos); //2번 플레이어의 앵커를만든다.
                        Debug.Log("2번 플레이어의 위치 앵커 생성");
                    }
                    else if(i==3)//3번 사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(0, 0, DistanceFromCenter);
                        anchor = hit.Trackable.CreateAnchor(pos); //3번 플레이어의 앵커를만든다.
                        Debug.Log("3번 플레이어의 위치 앵커 생성");
                    }
                    else //4번사용자의 위치
                    {
                        pos.position = firstpos.position + new Vector3(0, 0, -DistanceFromCenter);
                        anchor = hit.Trackable.CreateAnchor(pos); //4번 플레이어의 앵커를만든다.
                        Debug.Log("4번 플레이어의 위치 앵커 생성");
                    }
                    //생성한 앵커를 클라우드 앵커로 만들고 다른 플레이어가 앵커를 리졸브 할때까지 대기..
                    StartCoroutine(HostCloudAnchor(anchor));//클라우드 만들고 앵커 아이디를 보냄
                    yield return new WaitUntil(() => ismakeAnchor == true);//방장이 클라우드 앵커 생성할떄까지 대기
                    if (nowPlayerCnt > 1)
                    {
                        yield return new WaitUntil(() => PhotonManager.Instance.receiveCnt == PhotonManager.Instance.localPlayer - 1);//클라우드 앵커아이디가 모든 사용자에게 전달될때까지 대기
                                                                                                                //방장이 아닌 다름 사용자들에게 리졸브하라는 명령을 내림
                        PhotonManager.Instance.Call_ResolveRpc(i);
                        yield return new WaitUntil(() => PhotonManager.Instance.readyCnt == PhotonManager.Instance.localPlayer - 1); //방에 방장을 제외한 다른사용자가 리졸브 할때까지 대기
                    }
                    Debug.Log(i + "번째 반복문 끝남");
                }
                //반복문을 빠져나오면 모든 앵커가 잡히면 각자의 위치에 플레이어를 생성하라는 명령을 알피씨로 내리게함
                PhotonManager.Instance.Call_InstantePlayer();
                //Debug.Log("방장이 생성한 앵커의 위치 = " + anchor.transform.position);
                //GameObject obj = PhotonNetwork.Instantiate("TestBot", hit.Pose.position, Quaternion.identity);
                //obj = Instantiate(centerObject, anchor.transform.position, Quaternion.identity);
                //obj.transform.SetParent(anchor.transform);
                //Debug.Log("앵커에 자식이된 표시 오브젝트의 위치 = " + obj.transform.position);

            }
        }


    }
    //public void ButtonCreateCloudAnchor()
    //{
    //        if (PhotonManager.Instance.isMaster == true)//방장이면 앵커 만들고 다른사람들한테 클라우드 앵커 아이디 넘겨주기
    //        {
    //            //AsyncTask<CloudAnchorResult> task = XPSession.CreateCloudAnchor(anchor);
    //            if (firstAnchor == true && time > 3.0f)//방장은 첫앵커 생성하고 3초후에 버튼 누르면 호스팅 동작함
    //            {
    //                //StartCoroutine(HostCloudAnchor(anchor));
    //            }
    //        }
    //        else
    //        {
    //            if(PhotonManager.Instance.receiveId==true) //앵커 아이디를 받았으면
    //            {
    //            Debug.Log("방장아닌 사용자 앵커아이디 받음");
    //            //Debug.Log(PhotonManager.Instance.anchorId);
    //            //StartCoroutine(ResolveCloudAnchor(PhotonManager.Instance.anchorId));//코루틴 실행
    //            StartCoroutine(ResolveCloudAnchor(PhotonManager.Instance.anchorIdList));//코루틴 실행
    //            }
    //        }
        
    //}
    /// <summary>
    /// 방장이 생성된 앵커리스트를 보내서 호스팅
    /// 앵커 아이디를 생성해서 다른 유저에게 보냄
    /// </summary>
    /// <param name="anchorlist"></param>
    /// <returns></returns>
    IEnumerator HostCloudAnchor(Anchor anchor)
    {
        //anchor_ = AsyncTask < CloudAnchorResult > CreateCloudAnchor(anchor);
        //task = XPSession.CreateCloudAnchor(anchor);
        //Debug.Log(anchor);
        //Debug.Log(task);
        Debug.Log("응답 머기중.... ");
        task = XPSession.CreateCloudAnchor(anchor);
        yield return new WaitUntil(() => task.IsComplete == true);
        ismakeAnchor = true;
        //PhotonManager.Instance.CheckResolve();//readyCnt 1증가
        //PhotonManager.Instance.anchorIdList.Add(task.Result.Anchor.CloudId);//완료된 클라우드 앵커의 아이디를 리스트에 담는다.
        PhotonManager.Instance.hostingResultList.Add(task);//task를 담는다.
        Debug.Log(task.Result.Response);
        Debug.Log(task.Result.Anchor.CloudId);
        anchorid = task.Result.Anchor.CloudId;
        Debug.Log("클라우드 앵커 생성 상태:" + ismakeAnchor);
        //string id = task.Result.Anchor.CloudId;
        //Debug.Log(id);
        if (nowPlayerCnt > 1 && ismakeAnchor == true) //방장말고 다른 유저가 존재할때
        {
            Debug.Log("다른 사용자에게 앵커 아이디리스트 보내는중...");
            PhotonManager.Instance.SendAnchorId(anchorid);//rpc 가 실행되고 앵커아이디를 전달하고 나도 카운트가 1올라간다
        }
        //플레이어를 해당위치에 생성
        //hostingResultList 매개변수로
        //InstantePlayer(PhotonManager.Instance.hostingResultList);
    }
    /// <summary>
    /// 매개변수로 앵커 아이디 리스트를 받는다. 리솔브 한다.
    /// 방장아닌 참가자가 해야할것
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    //IEnumerator ResolveCloudAnchor(List<string> ids)
    //{
    //    //Debug.Log(id);
    //    //task = XPSession.ResolveCloudAnchor(id);
    //    Debug.Log("응답 머기중....방장한테 아이디 받는중");
    //    Debug.Log("전달 받은 앵커 아이디 숫자 = "+ids.Count);
    //    for (int i= 0; i < ids.Count;i++)
    //    {
    //        Debug.Log(i + "번째 앵커아이디 리졸브중..");
    //        task = XPSession.ResolveCloudAnchor(ids[i]);
    //        yield return new WaitUntil(() => task.IsComplete);
    //        PhotonManager.Instance.hostingResultList.Add(task);//결과를 담는다
    //        Debug.Log(task.Result.Response); 
    //        Debug.Log(task.Result.Anchor); 
    //        Debug.Log(task.Result.Anchor.CloudId);//NullReferenceExceoption
    //        Debug.Log(i+"번째 전달받은 클라우드 앵커 위치 = " + task.Result.Anchor.transform.position);
    //    }
    //    //yield return new WaitUntil(() => task.IsComplete);
    //    //obj = Instantiate(centerObject, task.Result.Anchor.transform.position, Quaternion.identity);//앵커위치에 생성하고
    //    //obj.transform.SetParent(task.Result.Anchor.transform);//부모로 설정
    //    //InstantePlayer(PhotonManager.Instance.hostingResultList);
    //}
    /// <summary>
    /// 앵커 기준으로 포톤 객체를 생성함(플레이할 객체)
    /// 리스트에 0번은 가운데 기준점이다. 리스트 0번위치로 모든 오브젝트가 회전에서 돌아보게할것임
    /// 0번 유저는 리스트1번앵커위치에 1번 유저는 리스트2번앵커위치에 ...
    /// </summary>
    //public void InstantePlayer(List<AsyncTask<CloudAnchorResult>> tasklist)
    //{
    //    Debug.Log("나의 입장 순서는 = "+PhotonManager.Instance.myOrder);
    //    if(PhotonManager.Instance.myOrder ==0)
    //    {
    //        Debug.Log("1번플레이어 생성");
    //        obj = PhotonNetwork.Instantiate("Player", tasklist[1].Result.Anchor.transform.position, Quaternion.identity);

    //    }
    //    else if(PhotonManager.Instance.myOrder == 1)
    //    {
    //        Debug.Log("2번플레이어 생성");
    //        obj = PhotonNetwork.Instantiate("Player", tasklist[2].Result.Anchor.transform.position, Quaternion.identity);

    //    }
    //    else if(PhotonManager.Instance.myOrder == 2)
    //    {
    //        Debug.Log("3번플레이어 생성");
    //        obj = PhotonNetwork.Instantiate("Player", tasklist[3].Result.Anchor.transform.position, Quaternion.identity);

    //    }
    //    else
    //    {
    //        Debug.Log("4번플레이어 생성");
    //        obj = PhotonNetwork.Instantiate("Player", tasklist[4].Result.Anchor.transform.position, Quaternion.identity);

    //    }
    //}
    public void TurnOff()
    {
        if (PhotonManager.Instance.isMaster == true)
        {
            Debug.Log("턴을 넘김");
            PhotonManager.Instance.Call_ChangeMasterClient();
        }
    }

}