using UnityEngine;
using UnityEngine.Events;

public class CustomClickable : Clickable
{
    public UnityEvent onClick;
    
    public override void Click(Vector3 hitPos){
      onClick.Invoke();
    }
}
