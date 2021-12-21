using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound_Channel
{
    BGM = 0, Effect, UI, Ambient, Voice
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager Instance = null;

    public Sound SoundPrefab = null;

    private Dictionary<Sound_Channel, List<Sound>> Sounds = null;
    private Dictionary<Sound_Channel, Queue<Sound>> SoundPool = null;
    private Dictionary<Sound_Channel, float> SoundVolume = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (!ReferenceEquals(null, Instance))
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //  Sounds 초기화
        if (null == Sounds)
            Sounds = new Dictionary<Sound_Channel, List<Sound>>();

        //  Pool 초기화
        if (null == SoundPool)
            SoundPool = new Dictionary<Sound_Channel, Queue<Sound>>();

        if (null == SoundVolume)
            SoundVolume = new Dictionary<Sound_Channel, float>();

        Add_SoundPool(Sound_Channel.BGM, 1);
        Add_SoundPool(Sound_Channel.Effect, 10);
        Add_SoundPool(Sound_Channel.UI, 10);
        Add_SoundPool(Sound_Channel.Ambient, 10);
        Add_SoundPool(Sound_Channel.Voice, 10);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float Get_Volume(Sound_Channel channel)
    {
        return SoundVolume[channel];
    }

    public void Set_Volume(Sound_Channel channel, float volume)
    {
        if (Sound_Channel.Ambient == channel)
            SoundVolume[channel] = volume * 0.1f;
        else
            SoundVolume[channel] = volume;

        foreach (var sound in Sounds[channel])
        {
            if (sound.gameObject.activeSelf)
                sound.Set_Volume(volume);
        }
    }

    void Add_SoundPool(Sound_Channel channel, int count)
    {
        if (!Sounds.ContainsKey(channel))
            Sounds.Add(channel, new List<Sound>());

        if (!SoundPool.ContainsKey(channel))
            SoundPool.Add(channel, new Queue<Sound>());

        if (!SoundVolume.ContainsKey(channel))
        {
            if (Sound_Channel.Ambient == channel)
                SoundVolume.Add(channel, 0.05f);
            else
                SoundVolume.Add(channel, 0.5f);
        }

        Sound sound = null;
        for (int i = 0; i < count; ++i)
        {
            sound = Instantiate(SoundPrefab, transform);
            sound.gameObject.SetActive(false);

            Sounds[channel].Add(sound);
            SoundPool[channel].Enqueue(sound);
        }
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="position">월드 위치</param>
    /// <param name="audioClip">클립</param>
    public void Play(Sound_Channel channel, Vector3 position, AudioClip audioClip)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        var pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, position, audioClip);

    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="parent">소리가 따라다닐 오브젝트</param>
    /// <param name="audioClip">클립</param>
    public void Play(Sound_Channel channel, GameObject parent, AudioClip audioClip)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        var pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, parent, audioClip);
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="position">월드 위치</param>
    /// <param name="audioClip">클립</param>
    /// <param name="pulledSound">실행한 소리 객체</param>
    public void Play(Sound_Channel channel, Vector3 position, AudioClip audioClip, out Sound pulledSound)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, position, audioClip);
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="parent">소리가 따라다닐 오브젝트</param>
    /// <param name="audioClip">클립</param>
    /// <param name="pulledSound">실행한 소리 객체</param>
    public void Play(Sound_Channel channel, GameObject parent, AudioClip audioClip, out Sound pulledSound)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, parent, audioClip);
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="position">월드 위치</param>
    /// <param name="audioClip">클립</param>
    /// <param name="loop">반복 여부</param>
    /// <param name="lifeTime">실행 시간, 0 이면 무한 반복</param>
    public void Play(Sound_Channel channel, Vector3 position, AudioClip audioClip, bool loop, float lifeTime = 0f)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        var pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, position, audioClip, loop, lifeTime);
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="parent">소리가 따라다닐 오브젝트</param>
    /// <param name="audioClip">클립</param>
    /// <param name="loop">반복 여부</param>
    /// <param name="lifeTime">실행 시간, 0 이면 무한 반복</param>
    public void Play(Sound_Channel channel, GameObject parent, AudioClip audioClip, bool loop, float lifeTime = 0f)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        var pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, parent, audioClip, loop, lifeTime);
    }


    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="position">월드 위치</param>
    /// <param name="audioClip">클립</param>
    /// <param name="pulledSound">실행한 소리 객체</param>
    /// <param name="loop">반복 여부</param>
    /// <param name="lifeTime">실행 시간, 0 이면 무한 반복</param>
    public void Play(Sound_Channel channel, Vector3 position, AudioClip audioClip, out Sound pulledSound, bool loop, float lifeTime = 0f)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, position, audioClip, loop, lifeTime);
    }

    /// <summary>
    /// 오디오 클립 플레이
    /// </summary>
    /// <param name="channel">실행할 채널</param>
    /// <param name="parent">소리가 따라다닐 오브젝트</param>
    /// <param name="audioClip">클립</param>
    /// <param name="pulledSound">실행한 소리 객체</param>
    /// <param name="loop">반복 여부</param>
    /// <param name="lifeTime">실행 시간, 0 이면 무한 반복</param>
    public void Play(Sound_Channel channel, GameObject parent, AudioClip audioClip, out Sound pulledSound, bool loop, float lifeTime = 0f)
    {
        if (0 == SoundPool[channel].Count)
            Add_SoundPool(channel, 10);

        pulledSound = SoundPool[channel].Dequeue();
        pulledSound.gameObject.SetActive(true);
        pulledSound.Play(channel, parent, audioClip, loop, lifeTime);
    }

    public void Play_CommanderSound(Sound_Channel channel, GameObject gameObject, Camp camp, ComSoundType soundType, int index, bool loop, float lifeTime = 0f)
    {
        var audios = SceneStarter.Instance.soundElements.ComSoundDic[camp][soundType];

        if (audios.Count <= index || index < 0)
        {
            Debug.Log("index is out of range - index : " + index + ", ComSoundDic[" + camp + "][" + soundType + "].Count : " + audios.Count);
            return;
        }

        Play(channel, gameObject, audios[index], loop, lifeTime);
    }

    public void Play_LobbySound(Sound_Channel channel, GameObject gameObject, LobbySoundType soundType, int index, bool loop, float lifeTime = 0f)
    {
        var audios = SceneStarter.Instance.soundElements.LobbySoundDic[soundType];

        if (audios.Count <= index || index < 0)
        {
            Debug.Log("index is out of range - index : " + index + ", LobbySoundDic[" + soundType + "].Count : " + audios.Count);
            return;
        }

        Play(channel, gameObject, audios[index], loop, lifeTime);
    }

    public void Stop(Sound_Channel channel)
    {
        if (Sound_Channel.BGM == channel)
        {
            Stop_BGM();
            return;
        }

        foreach (var sound in Sounds[channel])
        {
            if (sound.gameObject.activeSelf)
                sound.Stop();
        }
    }

    public void Pause(Sound_Channel channel)
    {
        foreach (var sound in Sounds[channel])
        {
            if (sound.gameObject.activeSelf)
                sound.Pause();
        }
    }

    public void Resume(Sound_Channel channel)
    {
        foreach (var sound in Sounds[channel])
        {
            if (sound.gameObject.activeSelf)
                sound.Play();
        }
    }

    public bool Play_BGM(AudioClip audioClip)
    {
        if (0 == SoundPool[Sound_Channel.BGM].Count)
        {
            Debug.Log("BGM Queue Count : " + SoundPool[Sound_Channel.BGM].Count);
            return false;
        }

        if (null == Camera.main)
        {
            Debug.Log("camera.main is null");
        }
        else
            Play(Sound_Channel.BGM, Camera.main.gameObject, audioClip, true);

        return true;
    }

    public void Stop_BGM()
    {
        var sound = Sounds[Sound_Channel.BGM][0];
        if (!sound.isActiveAndEnabled)
        {
            Debug.Log("BGM is already stopped.");
            return;
        }

        sound.Stop();
        sound.gameObject.SetActive(false);

        SoundPool[Sound_Channel.BGM].Enqueue(sound);
    }

    public bool Push_Sound(Sound_Channel channel, Sound sound)
    {
        sound.gameObject.SetActive(false);
        SoundPool[channel].Enqueue(sound);
        return true;
    }
}
