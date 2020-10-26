using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    //protected PhotonManager photonManager;
    public InputField roomName;//입력받을 필드
    private void Start()
    {
        
        //photonManager = PhotonManager.Instance();
    }
    /// <summary>
    /// 방생성
    /// </summary>
    public void CreateRoom()
    {
        PhotonManager.Instance.CreateRoom(roomName.text); //입력한 텍스트로 방생성
    }
    /// <summary>
    /// 방참가
    /// </summary>
    public void JoinRoom()
    {
        PhotonManager.Instance.JoinRoom(roomName.text);
    }
    //뒤로 로비씬으로 가기.
    public void ToBackToLobby()
    {
        SceneManager.LoadScene("03.Lobby");
    }
}
