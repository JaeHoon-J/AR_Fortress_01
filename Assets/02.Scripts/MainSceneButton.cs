using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneButton : MonoBehaviour
{
    //protected DataManager.Instance DataManager.Instance;
    protected DBManager dbManager;
    public GameObject option;
    public GameObject exit;
    public GameObject nickName;
    protected SoundManager soundManager = null;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.GetInstance();
        soundManager.SetBgmClip("lobby");
        //dbManager = DBManager.GetInstance();
        //DataManager.Instance = DataManager.Instance.GetInstance();
        if(DataManager.Instance.userinfo.firstLogin == true)
        {
            nickName.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    { 
    }
    public void ExitBtn()
    {
        exit.gameObject.SetActive(true);
    }
    public void YesQuitBtn()
    {
#if UNITY_EDITIOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("종료를 시작합니다.");
        Application.Quit();
#endif
    }
    public void NoQuitBtn()
    {
        exit.gameObject.SetActive(false);
    }
    /// <summary>
    /// 조립실로 이동
    /// </summary>
    public void ToLab()
    {
        soundManager.SetBgmClip("lab");
        SceneManager.LoadScene("TestLabUI");
    }
    /// <summary>
    /// 상점으로 이동
    /// </summary>
    public void ToStore()
    {
        soundManager.SetBgmClip("store");
        SceneManager.LoadScene("TestStore");
    }
    /// <summary>
    /// 방이생성 씬으로 이동 조립된 로봇이 없으면 이동을 못하게 막는다.
    /// </summary>
    public void ToPlayGame()
    {
        //로봇 조립된 상태인지 체크하고
        if(DataManager.Instance.beforeLeg !=null && DataManager.Instance.beforeBody !=null && DataManager.Instance.beforeWeapon !=null)
        {
            SceneManager.LoadScene("04.MakeRoom");
            soundManager.SetBgmClip("makeRoom");
        }
        else
        {
            //선택이 안되어 있다면 씬이동막고 로봇 조립하라는 메세지
            DataManager.Instance.Msg = "Build a robot in LAB";
        }

    }
    public void ToOption()
    {
        option.gameObject.SetActive(true);
    }
    public void ToCloseOption()
    {
        option.gameObject.SetActive(false);
    }
}
