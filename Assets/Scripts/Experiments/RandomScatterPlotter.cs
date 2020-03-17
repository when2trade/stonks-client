using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

/// <summary>
/// Makes a random 3D scatter plot, used in the ScatterIneractTest experiment.
/// </summary>
public class RandomScatterPlotter : MonoBehaviour
{
    public GameObject pointPrefab, edgePrefab;

    public int pointCount = 100;
    public Vector3 pointRange = Vector3.one;

    [Range(0,1)]
    public float edgeThreshold = 0.1f; //how liberally should we make edges?

    //edge colours
    public Color weakPositiveColor = Color.white, strongPositiveColor = Color.green, 
        weakNegativeColor = Color.white, strongNegativeColor = Color.red;

    public SectorColorScheme sectorColors;
    
    public Transform headTransform; //reference to head for billboarding

    public TextAsset tickers;

    void Start()
    {
        MakePlot();
    }

    /// <summary>
    /// Make a random spread of points and edges, as children of this object.s
    /// </summary>
    void MakePlot(){
        Vector3[] points = new Vector3[pointCount];
        for(int i=0;i<pointCount;i++){ //make 'pointCount'x random points in [-0.5,0.5]^3
            points[i] = new Vector3(
                Random.Range(-.5f,.5f),
                Random.Range(-.5f,.5f),
                Random.Range(-.5f,.5f));
        }

        List<(int,int)> edges = new List<(int, int)>();
        for(int i=0;i<pointCount;i++){
            for(int j=i+1; j<pointCount;j++){ //for each (symmetric!) point pair,
                if((points[i] - points[j]).magnitude < edgeThreshold){ //if points are close enough, add edge
                    edges.Add((i,j));
                }
            }
        }
        Debug.Log(edges.Count+" edges");

        string[] names = tickers.text.Split('\n');

        ScaleNegator scaler = GetComponent<ScaleNegator>();

        for(int i=0;i<pointCount;i++){
            //make and position points
            GameObject obj = Instantiate(pointPrefab, transform);
            obj.transform.localPosition = Vector3.Scale(points[i], pointRange);
            obj.GetComponent<SimpleBillboard>().targetTransform = headTransform;
            obj.GetComponent<SimpleDistanceHider>().referenceTransform = headTransform;

            //give random sector and company name
            obj.GetComponent<SpriteRenderer>().color = sectorColors.colors[
                Mathf.FloorToInt(Random.Range(0,sectorColors.colors.Length))];

            obj.GetComponentInChildren<TextMeshPro>().text = names[i % names.Length];

            scaler.objectsToScale.Add(obj.transform);
        }


        foreach((int,int) edge in edges){
            //make and position edges
            LineRenderer line = Instantiate(edgePrefab, transform).GetComponent<LineRenderer>();
            line.SetPositions(new Vector3[]{Vector3.Scale(points[edge.Item1], pointRange),
                Vector3.Scale(points[edge.Item2], pointRange)});

            if(Random.value > 0.5f) //colour green or red
                //linear interpolation between weak and strong correlation colours
                line.startColor = line.endColor = Color.Lerp(weakPositiveColor, strongPositiveColor, Random.value);
            else
                line.startColor = line.endColor = Color.Lerp(weakNegativeColor, strongNegativeColor, Random.value);
        }
    }

}
