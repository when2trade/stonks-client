using UnityEngine;
using System.Collections.Generic;

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

  Dictionary<PlotPoint, GameObject> pointPanelsInUse = new Dictionary<PlotPoint, GameObject>(), 
    edgePanelsInUse = new Dictionary<PlotPoint, GameObject>();

  void Start(){
    pointPanelPool = new List<GameObject>{
      Instantiate(pointPanelPrefab),
      Instantiate(pointPanelPrefab)};
    edgePanelPool = new List<GameObject>{
      Instantiate(edgePanelPrefab),
      Instantiate(edgePanelPrefab)};
  }

  void CloseAllPanels(){
    foreach(var entry in pointPanelsInUse){
      entry.Value.transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose");
      pointPanelPool.Add(entry.Value);
      pointPanelsInUse.Remove(entry.Key);
    }

    foreach(var entry in edgePanelsInUse){
      //entry.Value.transform.GetChild(0).GetComponent<Animation>().Play("StockWorldCanvasClose"); //????
      edgePanelPool.Add(entry.Value);
      edgePanelsInUse.Remove(entry.Key);
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
    go.GetComponent<SimpleLockOnto>().referenceTransform = point.transform;
    go.GetComponent<SimpleBillboardDampened>().referenceTransform = cameraAnchor;
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