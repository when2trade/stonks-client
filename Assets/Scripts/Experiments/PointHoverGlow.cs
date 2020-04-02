using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHoverGlow : MonoBehaviour
{
    public Color baseColor = Color.red;
    public Color hoverColor = Color.white;
    public int frames = 10;

    SpriteRenderer spr;
    LineRenderer line;
    Coroutine coroutine;
    
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        if(spr) baseColor =  spr.color;
        if(line) baseColor =  line.startColor;
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
            Color c = Color.Lerp(start,end, i/(float)frames);
            if(spr) spr.color = c;
            if(line) line.startColor = line.endColor = c;
            yield return null;
        }
        if(spr) spr.color = end;
        if(line) line.startColor = line.endColor = end;
    }

}
