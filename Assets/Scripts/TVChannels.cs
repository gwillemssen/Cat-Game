using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVChannels : MonoBehaviour
{

    [SerializeField] private List<VideoClip> TVClips = new List<VideoClip>();
    [SerializeField] private VideoPlayer videoPlayer;
    int Index = 0;

    // Start is called before the first frame update
    void Awake()
    {

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
       
            StartCoroutine(RunChangeChannel());
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator RunChangeChannel()
    {
        while (true)
        {
            videoPlayer.clip = TVClips[Index];
            videoPlayer.Play();
            if (Index < TVClips.Count-1) { Index++; }
            else { Index = 0; }
            yield return new WaitForSeconds((float)videoPlayer.clip.length);
        }
        
        

    }
}
