using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHoverGlow : MonoBehaviour
{
    public Color baseColor = Color.red;
    public Color hoverColor = Color.white;
    public int frames = 10;

    SpriteRenderer spr;
    Coroutine coroutine;
    
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.color = baseColor;
    }

    public void HoverEnter(){
        if(coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Fade(baseColor, hoverColor));
    }
    public void HoverExit(){
        if(coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Fade(hoverColor, baseColor));
    }


    IEnumerator Fade(Color start, Color end) 
    {
        for (int i=0;i<frames;i++) 
        {
            spr.color = Color.Lerp(start,end, i/(float)frames);
            yield return null;
        }
        spr.color = end;
    }

}
