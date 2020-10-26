using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }
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
        DontDestroyOnLoad(this.gameObject);
    }
    static public SoundManager GetInstance()
    {
        if(instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundManager";
            instance = obj.AddComponent<SoundManager>();
        }
        return instance;
    }
    // Update is called once per frame
    void Update()
    {
    }
    /*
    나는 여기서 사운드 매니저를 만들 것 임.
    현재, 각 씬 캔버스에 Audio Source에 있는 것을 여기에서 모든 것을 처리할 것이다.
    그래서 어떤 값을 변경해서 음악이 변경할 수 있도록 할 것임.
    그럼 일단 오디오 소스들을 public으로 선언을 해서 여기서 모든 값들을 받는 것이 1번이다.
    두가지를 받을 것임. effect와 bgm 두가지! 
    그렇다면, audio source component를 두개를 넣고,
    진동도 여기서 처리를 해야될까?
    */

    protected bool bgmOk = true;
    protected bool effectOk = true;

    protected AudioClip bgmAudioSourceClip;
    protected AudioClip effectSourceClip1;
    protected AudioSource bgmSource;
    protected AudioSource effectSource;
    protected AudioSource walkSource;
    public float bgmSourceVolume;
    public float effectSourceVolume;

    //이제 bgm 변수들 넣자.
    [SerializeField]
    protected AudioClip introLoginSceneClip;
    [SerializeField]
    protected AudioClip lobbySceneClip;
    [SerializeField]
    protected AudioClip makeAndReadyRoomSceneClip;
    [SerializeField]
    protected AudioClip playSceneClip;
    [SerializeField]
    protected AudioClip resultTimeClip;
    [SerializeField]
    protected AudioClip storeSceneClip;
    [SerializeField]
    protected AudioClip labSceneClip;

    //여기는 effect 관련 clip을 넣자.
    //[SerializeField]
    //protected AudioClip

    [SerializeField]
    protected AudioClip walkClip;
    private void Start()
    {
        bgmSource = AddAudio(true, false, 0.75f);//bgm audio source 추가
        effectSource = AddAudio(false, false, 1);//effect audio source 추가
        walkSource = AddAudio(true, true, 1f);
        SetBgmClip("intro");//bgm 시작
        //walkSession(walkClip);
        bgmSourceVolume = bgmSource.volume;
        effectSourceVolume = effectSource.volume;
    }
    //얘는 audio source를 추가해주고, 루프, 시작하자마자 시작, 볼륨에 대해서 설정해 주면서 컴포넌트를 만듦.
    protected AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        //newAudio.clip = clip; 
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }
    //만약에 bgm을 끄거나 하면 처리 할 수 있도록 만들자.
    public void SetONOFFBgmFromOption(bool onOff)
    {
        bgmOk = onOff;
        if(bgmOk)
        {
            bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
        }
    }
    public void SetBgmVolumeFromOption(float value)
    {
        bgmSource.volume = value;
    }
    //이번은 effect를 처리하려고 만들자.
    public void SetONOFFEffectFromOption(bool onoff)
    {
        effectOk = onoff;
    }
    public void SetEffectVolumeFromOption(float value)
    {
        effectSource.volume = value;
    }
    //으음.. 배경음악 부터 처리를 하자 배경음악 넣고 틀자!
    protected void BgmSession(AudioClip audio)
    {
        bgmSource.clip = audio;
        if (!bgmOk)
        {
            return;
        }
        bgmSource.Play();
    }
    //배경음악 넣기 위한 변수에 넣는 함수.
    public void SetBgmClip(string checkBgm)
    {
        
        switch(checkBgm)
        {
            case "intro":
                BgmSession(introLoginSceneClip);
                break;
            case "lobby":
                BgmSession(lobbySceneClip);
                break;
            case "store":
                BgmSession(storeSceneClip);
                break;
            case "lab":
                BgmSession(labSceneClip);
                break;
            case "makeRoom":
                BgmSession(makeAndReadyRoomSceneClip);
                break;
            case "play":
                BgmSession(playSceneClip);
                break;
            case "result":
                BgmSession(resultTimeClip);
                break;
        }
    }
    //이펙트를 켜주는 함수. 이펙트 클립에 넣어서 재생한다. 넣지만 실행이 안되도록 만들기는 함.
    protected void EffectSession(AudioClip audio)
    {
        effectSource.clip = audio;
        if(!effectOk) return;
        effectSource.Play();
    }
    //effect를 넣는 함수.
    public void SetEffectClip(string checkBgm)
    {
        
        switch(checkBgm)
        {
            case "intro":
                EffectSession(introLoginSceneClip);
                break;
            case "lobby":
                EffectSession(lobbySceneClip);
                break;
            case "store":
                EffectSession(storeSceneClip);
                break;
            case "lab":
                EffectSession(labSceneClip);
                break;
            case "makeRoom":
                EffectSession(makeAndReadyRoomSceneClip);
                break;
            case "play":
                EffectSession(playSceneClip);
                break;
            case "result":
                EffectSession(resultTimeClip);
                break;
        }
    }
    //이건 구상 하는 것.ㅎㅎ
    protected void walkSession(AudioClip audio)
    {
        walkSource.clip = audio;
        walkSource.Play();
    }
}
