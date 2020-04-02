using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Loads dataset files.
/// </summary>
public class Dataset : MonoBehaviour{
  private static Dataset singletonInstance;
  //public static Dataset singleton { get { return singletonInstance; } }

  public static Dictionary<string, string> nameMap, categoryMap;
  public static string finnhubAPIKey;

  public static Dictionary<string, int> indexMap;
  public static Dictionary<(int, int), float> correlationMap;
  public static Vector3[] points;

  void Awake(){
    if (singletonInstance != null && singletonInstance != this){
      Destroy(this);
      return;
    } else {
      singletonInstance = this;
    }

    nameMap = ParseTwoColumnTSV("Data/symbolToName");
    categoryMap = ParseTwoColumnTSV("Data/symbolToCategory");
    indexMap = ParseTwoColumnTSVI("Data/symbolToIndex");
    points = ParseVec3Array("Data/points");
    correlationMap = ParseCorrelations("Data/relations");
    finnhubAPIKey = Resources.Load<TextAsset>("Data/finnhubKey").text;
  }
  
  /// <summary>
  /// Returns the full name attributed with this symbol. If the symbol is not found, return the symbol.
  /// </summary>
  /// <param name="symbol"></param>
  public static string GetName(string symbol)
  {
    if(nameMap.ContainsKey(symbol)) return nameMap[symbol];
    else return symbol;
  }

  Dictionary<string, string> ParseTwoColumnTSV(string resourcePath){
    Dictionary<string, string> map = new Dictionary<string, string>();
    foreach(string entry in Resources.Load<TextAsset>(resourcePath).text.Split('\n')){
      string[] split = entry.Split('\t');
      if(!map.ContainsKey(split[0])) map.Add(split[0].Trim(), split[1].Trim());
    }
    return map;
  }

  Dictionary<string, int> ParseTwoColumnTSVI(string resourcePath){
    Dictionary<string, int> map = new Dictionary<string, int>();
    foreach(string entry in Resources.Load<TextAsset>(resourcePath).text.Split('\n')){
      string[] split = entry.Split('\t');
      if(!map.ContainsKey(split[0])) map.Add(split[0].Trim(), int.Parse(split[1].Trim()));
    }
    return map;
  }

  Vector3[] ParseVec3Array(string resourcePath){
    string[] lines = Resources.Load<TextAsset>(resourcePath).text.Split('\n');
    Vector3[] pts = new Vector3[lines.Length];
    for(int i=0; i<lines.Length;i++){
      string[] split = lines[i].Split(',');
      pts[i] = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
    }
    return pts;
  }

  Dictionary<(int, int), float> ParseCorrelations(string resourcePath){
    var map = new Dictionary<(int, int), float>();
    string[] lines = Resources.Load<TextAsset>(resourcePath).text.Split('\n');
    for(int i=0; i<lines.Length;i++){
      string[] split = lines[i].Split(',');
      for(int j=0; j<split.Length;j++){
        // try{
        //   float.Parse(split[j]);
        // }catch{
        //   Debug.Log(split[j]);
        //   Debug.Log(split[j].Length);
        //   Debug.Log("("+i+","+j+")");
        // }
        map.Add((i,i+j+1), float.Parse(split[j]));
      }
    }
    return map;    
  }

/// <summary>
/// returns the correlation coefficient of two companies.!--
/// WARNING: propagates the NaNs from the python script!
/// </summary>
  public static float GetRelation(string symbol1, string symbol2){
    int i1 = indexMap[symbol1], i2 = indexMap[symbol2];
    //Debug.Log("("+i1+","+i2+")");

    if(i1==i2) return 1f;
    else if(i1<i2) return correlationMap[(i1,i2)];
    else return correlationMap[(i2,i1)];
  }

  
}