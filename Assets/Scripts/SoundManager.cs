using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    //crincle sound found here: http://freesound.org/people/volivieri/sounds/37171/

    public AudioClip crincleAudioClip;
    AudioSource crincle;


    void Awake()
    {
        crincle = AddAudio(crincleAudioClip);
    }

    AudioSource AddAudio( AudioClip audioClip)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
        return audioSource;
    }

    public void PlayCrincle()
    {
        crincle.Play();
    }
}
