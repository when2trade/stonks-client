using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchButton : Clickable
{
    TouchScreenKeyboard keyboard;
    public override void Click(Vector3 hitpos){
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, true, "", 0);
    }
    void Update(){
      if(keyboard !=null && keyboard.status == TouchScreenKeyboard.Status.Done){
        string input = keyboard.text.ToUpper();

        keyboard = null;
      }
    }

    public void SearchFor(string search){
      RelationalScatterPlotter.singleton.ShowEdgesConnectedTo(search);
      var gO = RelationalScatterPlotter.singleton.GetPoint(search);
      if(gO!=null)
        StretchScaler.singleton.FlyTo(gO.transform);
    }
}
