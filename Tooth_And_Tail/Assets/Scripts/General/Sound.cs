using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public GameObject Parent = null;
    public AudioSource audioSource = null;

    public Sound_Channel Channel = Sound_Channel.BGM;

    private bool haveParent = false;
    private bool loop = false;
    private float lifeCount = 0f;
    private float lifeTime = 0f;
    private float volume = 1f;


    public void Play()
    {
        if (!ReferenceEquals(null, audioSource.clip))
            audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Stop()
    {
        audioSource.Stop();

        SoundManager.Instance.Push_Sound(Channel, this);
        lifeCount = 0f;
    }

    private bool Play(Sound_Channel channel, AudioClip audioClip)
    {
        volume = 1f;

        Channel = channel;

        Set_Audio(Channel);

        audioSource.clip = audioClip;

        Set_Volume(volume);

        audioSource.Play();

        return true;
    }

    public bool Play(Sound_Channel channel, Vector3 position, AudioClip audioClip)
    {
        Set_DefaultLoop(channel, audioClip);

        Play(channel, audioClip);

        haveParent = false;

        Parent = null;

        transform.position = position;

        return true;
    }

    public bool Play(Sound_Channel channel, GameObject parent, AudioClip audioClip)
    {
        Set_DefaultLoop(channel, audioClip);

        Play(channel, audioClip);

        haveParent = true;

        Parent = parent;

        Following_Parent();

        return true;
    }

    public bool Play(Sound_Channel channel, Vector3 position, AudioClip audioClip, bool loop, float lifeTime = 0f)
    {
        this.loop = loop;

        lifeCount = 0f;

        Play(channel, audioClip);

        if (0f != lifeTime)
            this.lifeTime = lifeTime;

        haveParent = false;

        transform.position = position;

        Following_Parent();

        return true;
    }

    public bool Play(Sound_Channel channel, GameObject parent, AudioClip audioClip, bool loop, float lifeTime = 0f)
    {
        this.loop = loop;

        lifeCount = 0f;

        Play(channel, audioClip);

        if (0f != lifeTime)
            this.lifeTime = lifeTime;

        haveParent = true;

        Parent = parent;

        Following_Parent();

        return true;
    }

    public void Set_Volume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume * SoundManager.Instance.Get_Volume(Channel);
    }

    public void Following_Parent()
    {
        if (null != Parent)
            transform.position = Parent.transform.position;
    }

    private void Set_DefaultLoop(Sound_Channel channel, AudioClip audioClip)
    {
        switch (channel)
        {
            case Sound_Channel.BGM:
                loop = true;
                break;
            case Sound_Channel.Effect:
            case Sound_Channel.Ambient:
            case Sound_Channel.Voice:
                loop = false;
                break;
        }

        lifeTime = audioClip.length;
    }

    private void Set_Audio(Sound_Channel channel)
    {
        audioSource.loop = loop;
        switch (channel)
        {
            case Sound_Channel.BGM:
                audioSource.spatialBlend = 0f;
                audioSource.minDistance = 1f;
                audioSource.maxDistance = 500f;
                audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                break;
            case Sound_Channel.Effect:
            case Sound_Channel.Ambient:
            case Sound_Channel.Voice:
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = 10f;
                audioSource.maxDistance = 12.2f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                break;
            case Sound_Channel.UI:
                audioSource.spatialBlend = 0f;
                audioSource.minDistance = 1f;
                audioSource.maxDistance = 500f;
                audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                break;
            default:
                break;
        }
    }

    private void Playing()
    {
        if (!loop || lifeTime != 0f)
        {
            lifeCount += Time.deltaTime;
        }

        if (!loop)
        {
            if (lifeCount >= lifeTime && !audioSource.isPlaying)
                Stop();
        }
        else if (lifeTime != 0f)
        {
            if (lifeCount >= lifeTime)
                Stop();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (haveParent)
        {
            if (null == Parent || !Parent.activeSelf)
            {
                Stop();
                return;
            }

            Following_Parent();
        }

        Playing();
    }
}
