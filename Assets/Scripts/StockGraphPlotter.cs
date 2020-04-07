using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class StockGraphPlotter : MonoBehaviour
{
  public Text currentValue, lineLow, lineHigh, lineLeft, lineRight;
  public UILineRenderer line;

  public Vector2 rangeX, rangeY;

  public void ShowPlot(float[] values){
    //find min/max
    float min = values[0], max = values[0];

    int i=0;
    for(i=1; i<values.Length; i++){
      min = Mathf.Min(min, values[i]); max = Mathf.Max(max, values[i]);
    }
    var dist = max-min;
    //plot values
    Vector2[] points = new Vector2[values.Length];
    for(i=0; i<values.Length; i++){
      points[i] = new Vector2(
          Mathf.Lerp(rangeX.x, rangeX.y, (float)i/(values.Length-1)), 
          Mathf.Lerp(rangeY.x, rangeY.y, (values[i]-min)/dist));
    }
    line.Points = points;
    lineLow.text = min.ToString("0.0");
    lineHigh.text = max.ToString("0.0");
  }

}