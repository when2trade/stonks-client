using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTest : MonoBehaviour
{
  void Start(){
    ServerFetch.singleton.GetJsonObject<DataCloud>("cloud", GetData);
  }

  void GetData(DataCloud cloud){
    Debug.Log(cloud.ToString());
  }
}