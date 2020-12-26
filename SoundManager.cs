using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// BGM은 아직 추가예정 -> Title, Ingame, 등등 으로 변경예정
public enum Player_SFX { Footstep1, Footstep2, Landing, Attack, None};
public enum BGM { BGM1, BGM2}

public class SoundManager : MonoBehaviour
{
    private static SoundManager sInstance = null;
    public static SoundManager Instance
    {
        get
        {
            if(sInstance == null)
            {
                GameObject newObj = new GameObject("_SoundManager");
                sInstance = newObj.AddComponent<SoundManager>();
            }
            return sInstance;
        }
    }

    // C++의 맵과 같은 기능을 가진 Dictionary사용.
    private Dictionary<Player_SFX, AudioClip> dPlayerEffectClips;
    private Dictionary<BGM, AudioClip> dBGMClips;
    private AudioSource playerSFX_SR;
    private AudioSource BGMSource;

    private float MasterVolume = 1.0f;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        dPlayerEffectClips = new Dictionary<Player_SFX, AudioClip>();
        dBGMClips = new Dictionary<BGM, AudioClip>();

        playerSFX_SR = GameObject.Find("Player").transform.Find("PlayerAudioSource").GetComponent<AudioSource>();
        BGMSource = Camera.main.transform.GetComponent<AudioSource>();

        this.LoadResoureFromResources();
    }

    private void Start()
    {
        // 임시로 시작과 동시에 BGM~
        this.PlayBGM(BGM.BGM1);
    }

    // Resources 폴더에서 가져온다.
    private void LoadResoureFromResources()
    {
        // BGM
        dBGMClips[BGM.BGM1] = Resources.Load<AudioClip>("Sound/BGM/BGM1");

        // Player SFX
        dPlayerEffectClips[Player_SFX.Footstep1] = Resources.Load<AudioClip>("Sound/Player/Player_Footstep_wav");
        dPlayerEffectClips[Player_SFX.Footstep2] = dPlayerEffectClips[Player_SFX.Footstep1];
        dPlayerEffectClips[Player_SFX.Landing] = Resources.Load<AudioClip>("Sound/Player/Player_Landing");
        dPlayerEffectClips[Player_SFX.Attack] = Resources.Load<AudioClip>("Sound/Player/Player_Wieldding_Sword");
    }

    // Public Functions ---------------------------------

    // Player SFX를 재생하기 위한 함수
    public void PlayPSFX(Player_SFX pSFX, float volume = 1.0f)
    {
        playerSFX_SR.clip = dPlayerEffectClips[pSFX];

        if (pSFX == Player_SFX.Footstep2)
            playerSFX_SR.pitch = 1.1f;
        else
            playerSFX_SR.pitch = (MasterVolume * 1.0f);

        playerSFX_SR.volume = volume;
        playerSFX_SR.loop = false;
        playerSFX_SR.Play();
    }

    // Player의 공격 사운드가 하나이기 때문에 pitch를 조정하여 다르게 들리도록 수정
    public void PlayAttackSound(int number)
    {
        playerSFX_SR.clip = dPlayerEffectClips[Player_SFX.Attack];
        playerSFX_SR.pitch = 1.0f + (0.1f * number);
        playerSFX_SR.loop = false;
        playerSFX_SR.Play();
    }

    // BGM 실행하기 위한 함수
    public void PlayBGM(BGM bgm, float volume = 1.0f)
    {
        BGMSource.volume = MasterVolume * volume;
        BGMSource.clip = dBGMClips[bgm];
        BGMSource.loop = true;

        BGMSource.Play();
    }
}
