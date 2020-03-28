using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Maps symbols to names.
/// </summary>
public class SymbolToName : MonoBehaviour{
  private static SymbolToName singletonInstance;
  public static SymbolToName singleton { get { return singletonInstance; } }

  Dictionary<string, string> map = new Dictionary<string, string>();

  void Awake(){
    if (singletonInstance != null && singletonInstance != this){
      Destroy(this);
      return;
    } else {
      singletonInstance = this;
    }
    foreach(string entry in Resources.Load<TextAsset>("Data/symbolToName").text.Split('\n')){
      string[] split = entry.Split('\t');
      if(!map.ContainsKey(split[0])) map.Add(split[0], split[1]);
    }
  }
  
  /// <summary>
  /// Returns the full name attributed with this symbol. If the symbol is not found, return the symbol.
  /// </summary>
  /// <param name="symbol"></param>
  public string GetName(string symbol)
  {
    if(map.ContainsKey(symbol)) return map[symbol];
    else return symbol;
  }

}