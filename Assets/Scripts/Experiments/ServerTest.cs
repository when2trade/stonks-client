using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTest : MonoBehaviour
{
  void Start(){
    ServerFetch.singleton.GetJsonObject<DataCloud>(ServerFetch.serverAddress + "cloud", GetData);
    ServerFetch.singleton.GetStockData("MSFT", Interpret);
  }

  void GetData(DataCloud cloud){
    Debug.Log(cloud.ToString());
  }

  void Interpret(DataCandle candle){
    Debug.Log(candle.s);
  }
}