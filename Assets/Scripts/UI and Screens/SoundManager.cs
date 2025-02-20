using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    public static SoundManager instacne;
    public Sound[] sounds;
    public enum SoundOptions
    {
        PlayerMove,
        PlayerAttack,
        PlayerGetHit,
        PlayerDies,
        BgSound

    }
    private void Awake()
    {
        if(instacne == null)
        {
            instacne = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            //s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        Debug.Log("Started playing: " + name);
            s.source.Play();
    }
    
}


