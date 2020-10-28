using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionManager : MonoBehaviour
{
    [SerializeField]
    protected Toggle bgmOnOff;
    [SerializeField]
    protected Toggle effectOnOff;
    [SerializeField]
    protected Slider bgmVolume;
    [SerializeField]
    protected Slider effectVolume;
    [SerializeField]
    protected GameObject soundTab;
    [SerializeField]
    protected GameObject MadeByTab;

    protected SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.GetInstance();
        //여기에서 소리 크기들을 여기에 세팅하자. 아 멍청한짓 한 거 같지만 함수 만들기 귀찮.
        bgmVolume.value= SoundManager.Instance.bgmSourceVolume ;
        effectVolume.value = SoundManager.Instance.effectSourceVolume;
        OnClickedSoundTab();
        soundManager.SetEffectClip("scenestart");
    }

    // Update is called once per frame
    void Update()
    {    
    }
    public void OnClickedSoundTab()
    {
        soundManager.SetEffectClip("click");
        soundTab.SetActive(true);
        MadeByTab.SetActive(false);
    }
    public void OnClickedMadeByTab()
    {
        soundManager.SetEffectClip("click");
        soundTab.SetActive(false);
        MadeByTab.SetActive(true);
    }
    public void CheckBgmOnOff()
    {
        soundManager.SetEffectClip("click");
        SoundManager.Instance.SetONOFFBgmFromOption(bgmOnOff.isOn);
    }
    public void SetBgmVolume()
    {
        SoundManager.Instance.SetBgmVolumeFromOption(bgmVolume.value);
    }
    public void CheckEffectOnOff()
    {
        soundManager.SetEffectClip("click");
        SoundManager.Instance.SetONOFFEffectFromOption(effectOnOff.isOn);
    }
    public void SetEffectVolume()
    {
        SoundManager.Instance.SetEffectVolumeFromOption(effectVolume.value);
    }
}
