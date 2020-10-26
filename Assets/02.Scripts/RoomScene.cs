using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RoomScene : MonoBehaviour
{
    public void StartGame()
    {
        PhotonManager.Instance.StartGame();
    }
    public void ToLobby()
    {
        PhotonManager.Instance.LeaveRoom();
    }

    [SerializeField]
    protected Text nickName0;
    [SerializeField]
    protected Text nickName1;
    [SerializeField]
    protected Text nickName2;
    [SerializeField]
    protected Text nickName3;
    [SerializeField]
    protected Transform[] userRobotPos = new Transform[4];
    [SerializeField]
    protected UserRobotInSlot[] userRobotInSlot = new UserRobotInSlot[4];

    protected DataManager dtm;
    protected PhotonManager photonManager;
    
    private void Awake()
    {

    }
    private void Start()
    {
        photonManager = PhotonManager.Instance;
        photonManager.sendMyInfo = ShowUserRobotAndName;
        Debug.Log("sendMyInfo 등록");
        photonManager.deleteMyInfo = DeleteSlotInfo;
        Debug.Log("deleteMyInfo 등록");
        photonManager.firstAnchor = false;
        dtm = DataManager.Instance;
        //룸으로 씬이동을하고 RPC로 나의 정보를 다른사용자에게 넘긴다
        photonManager.Call_SendMyInfoInWaitingRoom(photonManager.slotOrder, dtm.userinfo.nickName, dtm.userinfo.selectedLeg,
            dtm.userinfo.selectedBody, dtm.userinfo.selectedWeapon);

    }
    #region 입장시 호출되게할 함수 나의 정보를 다른사람에게 뿌려준다
    /// <summary>
    /// 입장순서 닉네임 사용중인 부품이름에 따라 대기화면 슬롯위치에 로봇 생성
    /// </summary>
    /// <param name="admissionOrder"></param>
    /// <param name="nickName"></param>
    /// <param name="legName"></param>
    /// <param name="bodyName"></param>
    /// <param name="wepoanName"></param>
    public void ShowUserRobotAndName(int admissionOrder, string nickName, string legName, string bodyName, string weaponName)
    {
        Debug.Log("슬롯에 정보 입력중");
        switch(admissionOrder)
        {
            case 0:
                if(string.IsNullOrEmpty(nickName0.text))
                {
                    Debug.Log("0번 슬롯 정보 입력");
                    nickName0.text = nickName;
                    FindRobotPart(admissionOrder, legName, bodyName, weaponName);
                }
                break;
            case 1:
                if (string.IsNullOrEmpty(nickName1.text))
                {
                    Debug.Log("1번 슬롯 정보 입력");
                    nickName1.text = nickName;
                    FindRobotPart(admissionOrder, legName, bodyName, weaponName);
                }
                break;
            case 2:
                if (string.IsNullOrEmpty(nickName2.text))
                {
                    Debug.Log("2번 슬롯 정보 입력");
                    nickName2.text = nickName;
                    FindRobotPart(admissionOrder, legName, bodyName, weaponName);
                }
                break;
            case 3:
                if (string.IsNullOrEmpty(nickName3.text))
                {
                    Debug.Log("3번 슬롯 정보 입력");
                    nickName3.text = nickName;
                    FindRobotPart(admissionOrder, legName, bodyName, weaponName);
                }
                break;
        }
    }
    public void FindRobotPart(int admissionOrder, string legname, string bodyname, string weaponname)
    {
        for (int i = 0; i < DataManager.Instance.legPartsArr.Length; i++)
        {
            if (DataManager.Instance.legPartsArr[i].name == legname)
            {
                userRobotInSlot[admissionOrder].leg = Instantiate(DataManager.Instance.legPartsArr[i], userRobotPos[admissionOrder].position, 
                    Quaternion.identity, userRobotPos[admissionOrder]);
            }
        }
        //몸체 정보 찾아서 생성
        for (int i = 0; i < DataManager.Instance.bodyPartsArr.Length; i++)
        {
            if (DataManager.Instance.bodyPartsArr[i].name == bodyname)
            {
                userRobotInSlot[admissionOrder].body = Instantiate(DataManager.Instance.bodyPartsArr[i],
                    userRobotInSlot[admissionOrder].leg.GetComponent<LegParts>().bodyPos.transform.position,
                    Quaternion.identity, userRobotPos[admissionOrder]);
            }
        }
        //무기 정보 찾아서 생성
        for (int i = 0; i < DataManager.Instance.weaponPartsArr.Length; i++)
        {
            if (DataManager.Instance.weaponPartsArr[i].name == weaponname)
            {
                userRobotInSlot[admissionOrder].weapon = Instantiate(DataManager.Instance.weaponPartsArr[i],
                    userRobotInSlot[admissionOrder].body.GetComponent<BodyParts>().weaponPos.transform.position,
                   Quaternion.identity, userRobotPos[admissionOrder]);
            }
        }
    }
    #endregion

    #region 퇴장시에 내 입장순서에 맞게 해당 슬롯의 정보를 비우고 룸을 나가게 한다
    public void DeleteSlotInfo(int admissionOrder)
    {
        switch(admissionOrder)
        {
            case 0:
                nickName0.text = null;
                Destroy(userRobotInSlot[admissionOrder].leg);
                Destroy(userRobotInSlot[admissionOrder].body);
                Destroy(userRobotInSlot[admissionOrder].weapon);
                break;
            case 1:
                nickName1.text = null;
                Destroy(userRobotInSlot[admissionOrder].leg);
                Destroy(userRobotInSlot[admissionOrder].body);
                Destroy(userRobotInSlot[admissionOrder].weapon);
                break;
            case 2:
                nickName2.text = null;
                Destroy(userRobotInSlot[admissionOrder].leg);
                Destroy(userRobotInSlot[admissionOrder].body);
                Destroy(userRobotInSlot[admissionOrder].weapon);
                break;
            case 3:
                nickName3.text = null;
                Destroy(userRobotInSlot[admissionOrder].leg);
                Destroy(userRobotInSlot[admissionOrder].body);
                Destroy(userRobotInSlot[admissionOrder].weapon);
                break;
        }
    }
    #endregion

    [Serializable]
    protected struct UserRobotInSlot
    {

        public GameObject leg;

        public GameObject body;

        public GameObject weapon;
    }
}
