using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Makes a 3D scatter plot from the input dataset.
/// TODO remove random point/edge data!
/// </summary>
public class RelationalScatterPlotter : MonoBehaviour
{
    public GameObject pointPrefab, edgePrefab;

    public int pointCount = 100;
    public Vector3 pointRange = Vector3.one;

    [Range(0,1)]
    public float edgeThreshold = 0.1f; //how liberally should we make edges?

    public SectorColorScheme sectorColors;
    
    public Transform headTransform; //reference to head for billboarding

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

        ScaleNegator scaler = GetComponent<ScaleNegator>();

        var syms = new List<string>();

        int p=0;
        foreach(var symbolCatPair in Dataset.categoryMap){
            //make and position points
            GameObject obj = Instantiate(pointPrefab, transform);
            obj.transform.localPosition = Vector3.Scale(points[p], pointRange);

            obj.GetComponent<PlotPoint>().SetupPoint(headTransform, symbolCatPair.Key);

            //give random sector and company name
            obj.GetComponent<SpriteRenderer>().color = sectorColors.ColorOfCategory(symbolCatPair.Value);

            scaler.objectsToScale.Add(obj.transform);

            syms.Add(symbolCatPair.Key);
            
            p+=1;
            if(p==pointCount) break;
        }
        Debug.Log(syms.Count);
        
        foreach((int,int) edge in edges){
            Vector3 p1 = Vector3.Scale(points[edge.Item1], pointRange), 
              p2 = Vector3.Scale(points[edge.Item2], pointRange);
            Vector3 vec = (p2-p1);

            //make and position edges
            Transform obj = Instantiate(edgePrefab, transform).transform;
            obj.localPosition = (p1+p2)/2.0f;
            obj.localRotation = Quaternion.LookRotation(vec, Vector3.up);
            obj.localScale = new Vector3(1,1, vec.magnitude);

            obj.GetComponent<PlotEdge>().SetupEdge(syms[edge.Item1], syms[edge.Item2], Random.value*2f-1f);
        }
    }

}
