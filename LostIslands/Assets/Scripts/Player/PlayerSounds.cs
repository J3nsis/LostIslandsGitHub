using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {


    public List<Clip> AudioClips = new List<Clip>();

    [Serializable]
    public struct Clip
    {
        public string name;
        public AudioClip audioClip;
    }
    [SerializeField]
    GameObject SoundPrefab;


    void Start () {
        //PlaySoundOnce("AxeChopping");
	}
	

    public void PlaySoundOnce(string name)
    {
        foreach (Clip clip in AudioClips)
        {
            if (clip.name == name)
            {
                GameObject Go = Instantiate(SoundPrefab);
                Go.transform.SetParent(transform);
                Go.GetComponent<AudioSource>().clip = clip.audioClip;
                Go.GetComponent<AudioSource>().Play();
                Destroy(Go, clip.audioClip.length);
            }
        }
    }
}
