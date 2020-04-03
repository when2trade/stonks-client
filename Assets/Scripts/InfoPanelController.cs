using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour{
  private static InfoPanelController singletonInstance;
  public static InfoPanelController singleton { get { return singletonInstance; } }

  void Awake(){
    if (singletonInstance != null && singletonInstance != this){
      Destroy(this);
      return;
    } else {
      singletonInstance = this;
    }
  }

  public GameObject pointPanelPrefab, edgePanelPrefab;

  public Transform cameraAnchor;

  //define a pool of 2 point panels and 2 edge panels
  List<GameObject> pointPanelPool, edgePanelPool;

  Dictionary<PlotPoint, GameObject> pointPanelsInUse = new Dictionary<PlotPoint, GameObject>();
  Dictionary<PlotEdge, GameObject> edgePanelsInUse = new Dictionary<PlotEdge, GameObject>();

  void Start(){
    pointPanelPool = new List<GameObject>{
      Instantiate(pointPanelPrefab),
      Instantiate(pointPanelPrefab)};
    edgePanelPool = new List<GameObject>{
      Instantiate(edgePanelPrefab),
      Instantiate(edgePanelPrefab)};
  }

  void CloseAllPanels(){
    foreach(var key in new List<PlotPoint>(pointPanelsInUse.Keys)){ //new list is important to make a copy of .Keys, so we're not removing on the fly and triggering an exception!
      pointPanelsInUse[key].transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose");
      pointPanelPool.Add(pointPanelsInUse[key]);
      pointPanelsInUse.Remove(key);
      key.canvasOpen = false;
    }

    foreach(var key in new List<PlotEdge>(edgePanelsInUse.Keys)){
      edgePanelsInUse[key].transform.GetChild(0).GetComponent<Animation>().Play("RelationWorldCanvasClose");
      edgePanelPool.Add(edgePanelsInUse[key]);
      edgePanelsInUse.Remove(key);
      key.canvasOpen = false;
    }
  }

  private T Dequeue<T>(List<T> xs){
    var x = xs[0];
    xs.RemoveAt(0);
    return x;
  }

  public void OpenPointPanel(PlotPoint point){
    CloseAllPanels();
    //get an unused point panel, lock it on and play anim
    GameObject go = Dequeue(pointPanelPool);

    go.SetActive(true);
    go.GetComponent<SimpleLockOnto>().referenceTransform = point.transform;
    go.GetComponent<SimpleBillboardDampened>().referenceTransform = cameraAnchor;

    //set initial pos/rot
    go.transform.position = point.transform.position;
    go.GetComponent<SimpleBillboardDampened>().SnapToDesiredRotation();

    go.transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasOpen");

    go.transform.Find("CANVAS/MASK/NAME").GetComponent<Text>().text = Dataset.GetName(point.symbol);

    pointPanelsInUse.Add(point, go);
  }

  public void OpenEdgePanel(PlotEdge edge, Vector3 hitPos){
    CloseAllPanels();

    //get an unused edge panel, lock it on and play anim
    GameObject go = Dequeue(edgePanelPool);

    go.SetActive(true);

    var lockon = go.GetComponent<SimpleLockOnto>();
    lockon.referenceTransform = edge.transform;
    lockon.localOffset = Quaternion.Inverse(edge.transform.rotation)*(hitPos - edge.transform.position);
    lockon.Apply(); //necessary for SnapToDesiredRotation to work

    go.GetComponent<SimpleBillboardDampened>().referenceTransform = cameraAnchor;

    //set initial pos/rot
    go.transform.position = edge.transform.position;
    go.GetComponent<SimpleBillboardDampened>().SnapToDesiredRotation();

    go.transform.GetChild(0).GetComponent<Animation>().Play("RelationWorldCanvasOpen");

    go.transform.Find("CANVAS/MASK/NAME1").GetComponent<Text>().text = Dataset.GetName(edge.symbol1);
    go.transform.Find("CANVAS/MASK/NAME2").GetComponent<Text>().text = Dataset.GetName(edge.symbol2);

    float val = edge.value;
    go.transform.Find("CANVAS/MASK/TEXTNEG").gameObject.SetActive(val <= 0);
    go.transform.Find("CANVAS/MASK/TEXTPOS").gameObject.SetActive(val > 0);
    
    go.transform.Find("CANVAS/MASK/PERCENTAGE").GetComponent<Text>().text = Mathf.Abs(val*100).ToString("0.0") + "%";

    edgePanelsInUse.Add(edge, go);
  }

  public void ClosePointPanel(PlotPoint point){
    pointPanelsInUse[point].transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose");
    pointPanelPool.Add(pointPanelsInUse[point]);
    pointPanelsInUse.Remove(point);
    
  }

  public void CloseEdgePanel(PlotEdge edge){
    edgePanelsInUse[edge].transform.GetChild(0).GetComponent<Animation>().Play("RelationWorldCanvasClose");
    edgePanelPool.Add(edgePanelsInUse[edge]);
    edgePanelsInUse.Remove(edge);   
  }
}