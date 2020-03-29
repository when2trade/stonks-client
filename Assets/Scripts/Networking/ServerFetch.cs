using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System;

/// <summary>
/// Class to fetch data from server.
/// </summary>
public class ServerFetch : MonoBehaviour{

  private static ServerFetch singletonInstance;
  public static ServerFetch singleton { get { return singletonInstance; } }

  public static string serverAddress = "";

  void Awake(){
    if (singletonInstance != null && singletonInstance != this){
        Destroy(this);
        return;
    } else {
        singletonInstance = this;
    }
    serverAddress = Resources.Load<TextAsset>("Data/serverAddress").text;
    //Debug.Log(serverAddress);
  }

  public void GetJsonObject<T>(string request, Action<T> callback)
  {
    StartCoroutine(GetRequest<T>(request, callback));
  }

  public void GetStockData(string symbol,  Action<DataCandle> callback, string resolution = "D", int count=30){
    string url = "https://finnhub.io/api/v1/stock/candle?symbol="
     + symbol + "&resolution=" + resolution + "&count="+count+"&token="+Dataset.finnhubAPIKey;
    StartCoroutine(GetRequest<DataCandle>(url, callback));
  }

  private IEnumerator GetRequest<T>(string uri,  Action<T> callback)
  {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
              Debug.LogError(uri + ": Error: " + webRequest.error);
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                try{
                  T jsonObject = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                  callback(jsonObject);
                }
                catch(JsonReaderException e){
                    Debug.LogError(uri + ": JSON can't be parsed! " + webRequest.downloadHandler.text);
                }
            }
        }
    }
}