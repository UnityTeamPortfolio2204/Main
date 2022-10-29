using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundKey
{
    BGM, HIT, 
    DRAGON_BITE, DRAGON_CLAW, DRAGON_FLAME, DRAGON_ROAR, DRAGON_STEP, DRAGON_WING, DRAGON_DEAD,
    GIANT_STEP, GIANT_ATTACK,
    TROLL_STEP, TROLL_ATTACK, TROLL_SMASH, TROLL_SMASH_VOICE, TROLL_DAMAGED, TROLL_DEAD,
    METALON_WALK, METALON_STAB, METALON_STAB_VOICE, METALON_SMASH, METALON_SMASH_VOICE, METALON_DEAD
}

[System.Serializable]
public struct ClipInfo
{
    public SoundKey key;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{ 
    static public SoundManager instance;

    public List<ClipInfo> clipInfos;

    private Dictionary<SoundKey, AudioClip> clips = new Dictionary<SoundKey, AudioClip>();

    private List<AudioSource> fxAudios = new List<AudioSource>();

    [Header("FX Option")]
    [Range(0.0f, 1.0f)]
    public float fxVolume = 1.0f;
    public int audioPoolCount = 10;
    public int soundDistanceMin = 5;
    public int soundDistanceMax = 10;


    private void Awake()
    {
        instance = this;

        CreateAudio();
    }

    public void CreateAudio()
    {
        foreach(ClipInfo clipInfo in clipInfos)
        {
            clips.Add(clipInfo.key, clipInfo.clip);
        }

        CreateFXAudio();
    }

    public void CreateFXAudio()
    {
        for(int i = 0; i <audioPoolCount; i++)
        {
            GameObject obj = new GameObject("FXAudio_" + i);
            obj.transform.SetParent(transform);

            AudioSource fxAudio = obj.AddComponent<AudioSource>();
            fxAudio.playOnAwake = false;
            fxAudio.volume = fxVolume;
            fxAudio.spatialBlend = 0.8f;
            fxAudio.minDistance = soundDistanceMin;
            fxAudio.maxDistance = soundDistanceMax;
            fxAudio.rolloffMode = AudioRolloffMode.Logarithmic;
            fxAudio.spatialBlend = 1.0f;
            fxAudio.dopplerLevel = 5.0f;

            fxAudios.Add(fxAudio);

            obj.SetActive(false);
        }
    }

    public void PlaySFX(SoundKey key, Vector3 pos)
    {
        if (!clips.ContainsKey(key)) return;

        AudioSource playAudio = null;

        foreach (AudioSource audio in fxAudios)
        {
            if(!audio.gameObject.activeSelf)
            {
                playAudio = audio;
                break;
            }
        }

        if (playAudio == null) return;

        playAudio.gameObject.SetActive(true);
        playAudio.transform.position = pos;

        playAudio.PlayOneShot(clips[key]);

        StartCoroutine(AudioDisable(playAudio, clips[key].length));
    }

    private IEnumerator AudioDisable(AudioSource audio, float time)
    {
        yield return new WaitForSeconds(time);

        audio.gameObject.SetActive(false);
    }
}
