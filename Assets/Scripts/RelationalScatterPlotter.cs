using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Makes a 3D scatter plot from the input dataset.
/// </summary>
public class RelationalScatterPlotter : MonoBehaviour
{
    private static RelationalScatterPlotter singletonInstance;
    public static RelationalScatterPlotter singleton { get { return singletonInstance; } }
    void Awake(){
        if (singletonInstance != null && singletonInstance != this){
        Destroy(this);
        return;
        } else {
        singletonInstance = this;
        }
    }

    public GameObject pointPrefab, edgePrefab;

    public int pointCount = 100;
    public Vector3 plotScaler = Vector3.one;

    [Range(0,1)]
    public float minCorrelation = 0.1f; //how liberally should we make edges?
    [Range(0,1)]
    public float maxCorrelation = 0.1f; 

    public SectorColorScheme sectorColors;
    
    public Transform headTransform; //reference to head for billboarding

    Dictionary<string, List<GameObject>> edgesIncidentToSymbol = new Dictionary<string, List<GameObject>>();
    Dictionary<string, GameObject> symbolToObj = new Dictionary<string, GameObject>();
    List<GameObject> edgesVisible = new List<GameObject>();


    void Start()
    {
        MakePlot();
    }

    /// <summary>
    /// Make a random spread of points and edges, as children of this object.
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
            symbolToObj.Add(symbol, obj);

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
                    GameObject edgeObj = Instantiate(edgePrefab, transform);
                    edgeObj.transform.localPosition = (p1+p2)/2.0f;
                    edgeObj.transform.localRotation = Quaternion.LookRotation(vec, Vector3.up);
                    edgeObj.transform.localScale = new Vector3(1,1, vec.magnitude);

                    edgeObj.GetComponent<PlotEdge>().SetupEdge(symbol, symbol2, relation);

                    AddToIncidence(symbol, edgeObj);
                    AddToIncidence(symbol2, edgeObj);

                    edgeObj.SetActive(false);

                    totalEdges+=1;
                }
            }
        }
        Debug.Log(totalEdges+" total edges");
    }

    void AddToIncidence(string symbol, GameObject edge){
        if(!edgesIncidentToSymbol.ContainsKey(symbol))
            edgesIncidentToSymbol.Add(symbol, new List<GameObject>());
        edgesIncidentToSymbol[symbol].Add(edge);
    }

    public void ShowEdgesConnectedTo(string symbol){
        bool didOneAppear = false;
        if(edgesIncidentToSymbol.ContainsKey(symbol)){
            foreach(GameObject edge in edgesIncidentToSymbol[symbol]){
                if(!edge.activeSelf) didOneAppear = true;
                edge.SetActive(true);
                edgesVisible.Add(edge);
            }
        }
        if(didOneAppear)
            SFXController.singleton.PlaySfxEdge(true, headTransform.position); 
    }

    public GameObject GetPoint(string symbol){
        if(symbolToObj.ContainsKey(symbol)){
            return symbolToObj[symbol];
        }
        return null;
    }

    public void HideAllEdges(){
        foreach(GameObject edge in edgesVisible)
            edge.SetActive(false);
        edgesVisible = new List<GameObject>();     
        SFXController.singleton.PlaySfxEdge(false, headTransform.position);
    }
}
