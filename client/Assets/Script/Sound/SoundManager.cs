using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

public enum SoundCategory
{
    BGM = 1,
    Battle = 2,
    UI = 3,
    Talk = 4,
    Other = 5
}
public class SoundCategoryInfo
{
    public int category;
    public float volume;
    public bool isMute;
    public string pause_name;
    public string volume_name;
    public string mute_name;
    public AudioMixerGroup group;
}
public class SoundManager : BehaviourSingleton<SoundManager>
{
    private AudioMixer audioMixer;

    private List<AudioSource> cacheAudioes = new List<AudioSource>(); //2d音源缓存
    private List<AudioSource> totalAudioes = new List<AudioSource>(); //2d音源
    private List<SoundController> soundCtrs = new List<SoundController>();//音效控制器

    static Dictionary<int, string> pathDic = new Dictionary<int, string>()
    {
        {(int)SoundCategory.UI, "Sound/UI_ab/" },
        {(int)SoundCategory.Talk, "Sound/Talk_ab_sg/" },
        {(int)SoundCategory.Other, "Sound/Other_ab/" },
        {(int)SoundCategory.BGM, "Sound/BGM_ab_sg/" },
        {(int)SoundCategory.Battle, "Sound/Battle_ab/" },
    };
    private Dictionary<int, SoundCategoryInfo> categoryInfoDic = new Dictionary<int, SoundCategoryInfo>();

    public void Init()
    {
        gameObject.AddComponent<AudioListener>();
        this.audioMixer = BundleManager.Instance.LoadAsset<AudioMixer>("Sound/Other_ab/AudioMixer.mixer");
        foreach (int category in Enum.GetValues(typeof(SoundCategory)))
        {
            string strName = Enum.GetName(typeof(SoundCategory), category);//获取名称
            SoundCategoryInfo ci = new SoundCategoryInfo();
            ci.category = category;
            ci.volume_name = strName + "_Volume";
            ci.mute_name = strName + "_Mute";
            ci.pause_name = "Volume";
            ci.volume = PlayerPrefs.GetFloat(ci.volume_name, 0.5f);
            ci.isMute = PlayerPrefs.GetInt(ci.mute_name, 0) == 1;
            ci.group = GetAudioMixerGroup("Master/" + strName);
            categoryInfoDic[category] = ci;
        }
    }
    AudioMixerGroup GetAudioMixerGroup(string name)
    {
        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(name);
            if (groups.Length > 0)
                return groups[0];
        }
        return null;
    }
    public void Add(SoundController sc)
    {
        if (soundCtrs.Contains(sc))
            return;
        soundCtrs.Add(sc);
    }
    public void Remove(SoundController sc)
    {
        soundCtrs.Remove(sc);
    }
    public AudioSource Take()
    {
        AudioSource audioSource = null;
        if (cacheAudioes.Count > 0)
        {
            audioSource = cacheAudioes[0];
            cacheAudioes.RemoveAt(0);
        }
        else
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            totalAudioes.Add(audioSource);
        }
        return audioSource;
    }
    public void Back(AudioSource audioSource)
    {
        if(totalAudioes.Contains(audioSource))
        {
            cacheAudioes.Add(audioSource);
        }
    }
    public static string GetPath(int category)
    {
        string path = null;
        if(pathDic.TryGetValue(category, out path) == false)
        {
            Debug.Log($"not exist SoundCategory({category}).");
        }
        return path;
    }
    public void StopByCategory(int category)
    {
        for(int i = 0; i < soundCtrs.Count; i++)
        {
            soundCtrs[i].StopByCategory(category);
        }
    }
    public AudioMixerGroup GetGroup(int category)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        return ci.group;
    }
    public void SetMute(int category, bool isMute)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        if(ci.isMute != isMute)
        {
            PlayerPrefs.SetInt(ci.mute_name, isMute ? 1 : 0);
            ci.isMute = isMute;
            for (int i = 0; i < soundCtrs.Count; i++)
            {
                soundCtrs[i].SetMuteByCategory(category, isMute);
            }
        }
    }
    public bool IsMute(int category)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        return ci.isMute;
    }
    public float GetVolume(int category)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        return ci.volume;
    }
    public void ModifyVolume(int category, float volume)
    {
        if(volume > 1)
        {
            volume = 1;
        }
        else if(volume < 0)
        {
            volume = 0;
        }
        SoundCategoryInfo ci = categoryInfoDic[category];
        PlayerPrefs.SetFloat(ci.volume_name, volume);
        ci.volume = volume;
        for (int i = 0; i < soundCtrs.Count; i++)
        {
            soundCtrs[i].ModifyVolumeByCategory(category, volume);
        }
    }
    public void Pause(int category)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        ci.group.audioMixer.SetFloat(ci.pause_name, -80);
    }
    public void Normal(int category)
    {
        SoundCategoryInfo ci = categoryInfoDic[category];
        ci.group.audioMixer.SetFloat(ci.pause_name, 0);
    }
}
