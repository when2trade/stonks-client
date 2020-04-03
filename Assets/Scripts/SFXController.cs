using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Plays sound effects.
/// </summary>
public class SFXController : MonoBehaviour
{
    private static SFXController singletonInstance;
    public static SFXController singleton { get { return singletonInstance; } }
    void Awake(){
        if (singletonInstance != null && singletonInstance != this){
        Destroy(this);
        return;
        } else {
        singletonInstance = this;
        }
    }

    public AudioClip sfxHover, sfxClick, sfxEdgeShow, sfxEdgeHide, sfxDrag;
    public float volumeHover, volumeClick, volumeEdgeShow, volumeDrag;

    public float dragNoiseLowpassFreqMin = 100, dragNoiseLowpassFreqScale = 1000;
    public float dragNoisePitchScale = 0.2f;

    public float vibrateDuration = .1f;
    [Range(0,1)]
    public float vibrateFrequency = 1f;
    [Range(0,1)]
    public float vibrateAmplitude = .3f;

    AudioLowPassFilter lowpass;
    AudioSource source;

    float vibrateRemainL = 0, vibrateRemainR = 0;

    void Start(){
      source = GetComponent<AudioSource>();
      lowpass = GetComponent<AudioLowPassFilter>();
    }

    public void PlayHover(Vector3 pos){
      AudioSource.PlayClipAtPoint(sfxHover, pos, volumeHover);
    }
    public void PlayClick(Vector3 pos){
      AudioSource.PlayClipAtPoint(sfxClick, pos, volumeClick);
    }
    public void PlaySfxEdge(bool show, Vector3 pos){
      if(show) AudioSource.PlayClipAtPoint(sfxEdgeShow, pos, volumeEdgeShow);
      else AudioSource.PlayClipAtPoint(sfxEdgeHide, pos, volumeEdgeShow);
    }

    public void PlayVibrate(bool isLeft){
      if(isLeft) vibrateRemainL = vibrateDuration;
      else vibrateRemainR = vibrateDuration;
      OVRInput.SetControllerVibration(vibrateFrequency, vibrateAmplitude, isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }

    void Update(){
      if(vibrateRemainL > 0){
        vibrateRemainL -= Time.deltaTime;
        if(vibrateRemainL <=0 )
          OVRInput.SetControllerVibration(0,0, OVRInput.Controller.LTouch);
      }
      if(vibrateRemainR > 0){
        vibrateRemainR -= Time.deltaTime;
        if(vibrateRemainR <=0 )
          OVRInput.SetControllerVibration(0,0, OVRInput.Controller.RTouch);
      }
    }


    bool wasOn = false;
    public void UpdateDragNoise(bool on, float scaleVelocity, float moveVelocity){
      if(on){
        if(!wasOn){
          source.clip = sfxDrag;
          source.Play();
        }
        lowpass.cutoffFrequency =  dragNoiseLowpassFreqMin +  moveVelocity * dragNoiseLowpassFreqScale;    
        source.pitch = 1 - scaleVelocity * dragNoisePitchScale;    
      }
      else{
        if(wasOn) source.Stop();
      }
      wasOn = on;
    }
}