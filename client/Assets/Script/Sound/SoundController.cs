using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

struct AudioSourceInfo
{
    public AudioSource audioSource;
    public soundconfig sc;

    public AudioSourceInfo(AudioSource audioSource, soundconfig sc)
    {
        this.audioSource = audioSource;
        this.sc = sc;
    }
}

public class SoundController : MonoBehaviour
{
    List<AudioSource> AS3d = null;
    List<AudioSourceInfo> playingAS = null;

    private void Awake()
    {
        SoundManager.Instance.Add(this);
    }
    private void OnDestroy()
    {
        StopAll();
        SoundManager.Instance.Remove(this);
    }
    public void Play(int soundId)
    {
        if (gameObject == null || gameObject.activeSelf == false)
            return;
        soundconfig sc = null;
        if (ConfigDataManager.Instance.soundconfigs.TryGetValue(soundId, out sc) == false)
        {
            Debug.Log($"not exist key({soundId}) in soundconfig");
            return;
        }
        string path = SoundManager.GetPath(sc.category) + sc.name;
        BundleManager.Instance.LoadAssetAsyn<AudioClip>(path, true, (ac) =>
        {
            if (gameObject == null || gameObject.activeSelf == false)
            {//对象被销毁或者隐藏不播放加载完成的音效
                return;
            }
            if (ac == null)
            {
                Debug.Log($"not exist soundclip : {path}");
                return;
            }
            if (sc.play_way == 1)
            {//停止同类别音效
                SoundManager.Instance.StopByCategory(sc.category);
            }
            AudioSource audioSource = null;
            if (sc.is3D)
            {
                if (AS3d != null && AS3d.Count > 0)
                {
                    audioSource = AS3d[0];
                    AS3d.RemoveAt(0);
                }
                else
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            else
            {
                audioSource = SoundManager.Instance.Take();
            }
            if (playingAS == null)
            {
                playingAS = new List<AudioSourceInfo>();
            }

            playingAS.Add(new AudioSourceInfo(audioSource, sc));
            audioSource.clip = ac;
            audioSource.playOnAwake = false;
            audioSource.loop = sc.isLoop;
            audioSource.mute = SoundManager.Instance.IsMute(sc.category);//是否静音
            audioSource.volume = CalculateVolume(sc.normal_volume, sc.max_volume, SoundManager.Instance.GetVolume(sc.category));
            audioSource.outputAudioMixerGroup = SoundManager.Instance.GetGroup(sc.category);
            if (sc.is3D)
            {
                audioSource.spatialBlend = 1;
                audioSource.minDistance = sc.min_dis;
                audioSource.maxDistance = sc.max_dis;
                audioSource.spread = 100;
            }
            else
            {
                audioSource.spatialBlend = 0;
            }

            audioSource.Play();
            if (sc.isLoop)
            {
                if (sc.time > 0.01f)
                {
                    StartCoroutine(SoundRecyc(audioSource, sc.time));
                }
            }
            else
            {
                StartCoroutine(SoundRecyc(audioSource, audioSource.clip.length));
            }
        });
    }
    IEnumerator SoundRecyc(AudioSource audioSource, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Stop(audioSource);
    }
    void Stop(AudioSource audioSource)
    {
        int index = playingAS.FindIndex(x => x.audioSource == audioSource);
        if(index < 0)
        {
            return;
        }
        AudioSourceInfo asInfo = playingAS[index];
        playingAS.RemoveAt(index);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.outputAudioMixerGroup = null;
        if (asInfo.sc.is3D)
        {
            if (AS3d == null)
                AS3d = new List<AudioSource>();
            AS3d.Add(audioSource);
        }
        else
        {
            SoundManager.Instance.Back(audioSource);
        }
    }
    public void Stop(int soundId)
    {
        if (playingAS == null)
            return;
        for (int i = playingAS.Count - 1; i >= 0; i--)
        {
            if (playingAS[i].sc.ID == soundId)
            {
                Stop(playingAS[i].audioSource);
            }
        }
    }
    public void StopAll()
    {
        if(playingAS != null)
        {
            for (int i = playingAS.Count - 1; i >= 0; i--)
            {
                Stop(playingAS[i].audioSource);
            }
        }
        StopAllCoroutines();
    }
    public void StopByCategory(int category)
    {
        if (playingAS == null)
            return;
        for (int i = playingAS.Count - 1; i >= 0; i--)
        {
            if (playingAS[i].sc.category == category)
            {
                Stop(playingAS[i].audioSource);
            }
        }
    }
    public void SetMuteByCategory(int category, bool isMute)
    {
        if (playingAS == null)
            return;
        for (int i = playingAS.Count - 1; i >= 0; i--)
        {
            if (playingAS[i].sc.category == category)
            {
                playingAS[i].audioSource.mute = isMute;
            }
        }
    }
    public void ModifyVolumeByCategory(int category, float volume)
    {
        if (playingAS == null)
            return;
        for (int i = playingAS.Count - 1; i >= 0; i--)
        {
            if (playingAS[i].sc.category == category)
            {
                playingAS[i].audioSource.volume = CalculateVolume(playingAS[i].sc.normal_volume, playingAS[i].sc.max_volume, volume);
            }
        }
    }
    public float CalculateVolume(float normal, float max, float volume)
    {
        if(volume <= 0.5f)
        {
            return (1 - (0.5f - volume) * 2) * normal;
        }
        else
        {
            return normal + (max - normal) * (volume - 0.5f) * 2;
        }
    }
}
