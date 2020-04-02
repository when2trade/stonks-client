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
    public Vector3 plotScaler = Vector3.one;

    [Range(0,1)]
    public float minCorrelation = 0.1f; //how liberally should we make edges?
    [Range(0,1)]
    public float maxCorrelation = 0.1f; 

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
        ScaleNegator scaler = GetComponent<ScaleNegator>();
        int totalEdges=0;
        
        foreach(var symbolIndexPair in Dataset.indexMap){
            string symbol = symbolIndexPair.Key;
            int index = symbolIndexPair.Value;
            Vector3 p1 = Vector3.Scale(Dataset.points[index], plotScaler);

            GameObject obj = Instantiate(pointPrefab, transform);
            obj.transform.localPosition = p1;

            obj.GetComponent<PlotPoint>().SetupPoint(headTransform, symbol);
            obj.GetComponent<SpriteRenderer>().color = sectorColors.ColorOfCategory(Dataset.categoryMap[symbol]);

            scaler.objectsToScale.Add(obj.transform);

            //add relevant edges to plot
            foreach(var symbolIndexPair2 in Dataset.indexMap){
                string symbol2 = symbolIndexPair2.Key;
                int index2 = symbolIndexPair2.Value;
                if(index2<=index) continue; //skip adding duplicate edges

                float relation = Dataset.GetRelation(symbol, symbol2);

                if(!float.IsNaN(relation) && Mathf.Abs(relation) > minCorrelation && Mathf.Abs(relation) < maxCorrelation){
                    //instantiate and position edges
                    Vector3 p2 = Vector3.Scale(Dataset.points[index2], plotScaler);
                    Vector3 vec = (p2-p1);
                    Transform edgeObj = Instantiate(edgePrefab, transform).transform;
                    edgeObj.localPosition = (p1+p2)/2.0f;
                    edgeObj.localRotation = Quaternion.LookRotation(vec, Vector3.up);
                    edgeObj.localScale = new Vector3(1,1, vec.magnitude);

                    edgeObj.GetComponent<PlotEdge>().SetupEdge(symbol, symbol2, relation);

                    totalEdges+=1;
                }
            }
        }
        Debug.Log(totalEdges+" total edges");
    }

}
