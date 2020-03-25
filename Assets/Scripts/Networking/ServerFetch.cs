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

  string serverAddress = "";

  public void Awake(){
    if (singletonInstance != null && singletonInstance != this){
        Destroy(this);
        return;
    } else {
        singletonInstance = this;
    }
    serverAddress = Resources.Load<TextAsset>("Data/serverAddress").text;
    //Debug.Log(serverAddress);
  }

  public void GetJsonObject(string request, Action<dynamic> callback)
  {
    StartCoroutine(GetRequest(serverAddress+request, callback));
  }

  private IEnumerator GetRequest(string uri,  Action<dynamic> callback)
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
                dynamic jsonObject = JsonConvert.DeserializeObject(webRequest.downloadHandler.text);
                callback(jsonObject);
            }
        }
    }
}