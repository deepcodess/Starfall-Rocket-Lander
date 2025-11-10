using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static float musicTime;


    private AudioSource musicAudioSource;
    
    private void Awake()
    {
        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.time = musicTime;
    }

    private void Update()
    {
        musicTime = musicAudioSource.time;
    }
}
