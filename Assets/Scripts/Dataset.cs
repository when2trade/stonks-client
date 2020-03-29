using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Loads dataset files.
/// </summary>
public class Dataset : MonoBehaviour{
  private static Dataset singletonInstance;
  //public static Dataset singleton { get { return singletonInstance; } }

  public static Dictionary<string, string> nameMap, categoryMap;

  void Awake(){
    if (singletonInstance != null && singletonInstance != this){
      Destroy(this);
      return;
    } else {
      singletonInstance = this;
    }

    nameMap = ParseTwoColumnTSV("Data/symbolToName");
    categoryMap = ParseTwoColumnTSV("Data/symbolToCategory");
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
}