using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
      //edgePanelsInUse[key].transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose");
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
    go.GetComponent<SimpleBillboardDampened>().SnapToDesiredRotation(); //so it doesn't spin around on first hit
    go.transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasOpen");
    pointPanelsInUse.Add(point, go);
  }

  public void OpenEdgePanel(PlotEdge edge){
    CloseAllPanels();
    //get an unused edge panel, lock it on and play anim
  }

  public void ClosePointPanel(PlotPoint point){
    pointPanelsInUse[point].transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose");
    pointPanelPool.Add(pointPanelsInUse[point]);
    pointPanelsInUse.Remove(point);
    
  }

  public void CloseEdgePanel(PlotEdge edge){

  }
}