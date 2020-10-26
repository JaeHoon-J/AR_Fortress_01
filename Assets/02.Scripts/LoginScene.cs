using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LoginScene : MonoBehaviour
{
    protected LoginManager loginManager = null;
    public GameObject loginBtn;
    public GameObject loadingGauge;
    private void Start()
    {
        loginManager = LoginManager.GetInstance();
    }
    public void Login()
    {
        loginBtn.gameObject.SetActive(false);
        loadingGauge.gameObject.SetActive(true);
        //버튼 클리갛면 로그인 처리후 로비로 입장
#if GOOGLEGAMES
        loginManager.GoogleLogin(); //구글 로그인 되면 이거 사용한다.
        //NetWork.Get.JoinLobby();// DB 없이 arcore 테스트 해볼때만 사용한다
#endif

#if UNITY_EDITOR
        loginManager.TestLogin();//에디터에서 구글로그인 빼고 테스트용 커스텀 아이디로 플레이팹 로그인되게 해놓음
        //NetWork.Get.JoinLobby();
#endif
    }
}
