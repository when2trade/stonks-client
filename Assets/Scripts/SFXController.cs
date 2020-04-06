using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public AudioMixer mixer;

    public float dragNoiseLowpassFreqMin = 100, dragNoiseLowpassFreqScale = 1000;
    public float dragNoisePitchScale = 0.2f;
    public float dragNoiseBasePitch = 1.3f;
    [Range(0,1)]
    public float dragNoiseDamping = 0.9f;

    public float vibrateDuration = .1f;
    [Range(0,1)]
    public float vibrateFrequency = 1f;
    [Range(0,1)]
    public float vibrateAmplitude = .3f;

    AudioSource source;

    void Start(){
      source = GetComponent<AudioSource>();
      source.clip = sfxDrag;
      source.Play();
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
      var type = isLeft ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
      OVRInput.SetControllerVibration(vibrateFrequency, vibrateAmplitude, type);
      StartCoroutine(StopVibrate(type));
    }

    IEnumerator StopVibrate(OVRInput.Controller type){
      yield return new WaitForSeconds(vibrateDuration);
      OVRInput.SetControllerVibration(0,0, type);
    }

    float moveDamped = 0, scaleDamped = 0;
    public void UpdateDragNoise(float scaleVelocity, float moveVelocity){
        scaleDamped = Mathf.Lerp(scaleVelocity, scaleDamped, dragNoiseDamping);
        moveDamped = Mathf.Lerp(moveVelocity, moveDamped, dragNoiseDamping);
        //mixer.SetFloat("cutoffFreq", dragNoiseLowpassFreqMin +  moveDamped * dragNoiseLowpassFreqScale);
        source.volume = moveDamped * dragNoiseLowpassFreqScale * volumeDrag;
        source.pitch = dragNoiseBasePitch - scaleDamped * dragNoisePitchScale;    
    }
}