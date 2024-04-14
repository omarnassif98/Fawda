using UnityEngine;
using UnityEngine.Events;

public class BlackoutBehaviour : MonoBehaviour
{
    public UnityEvent blackoutHiddenEvent {get; private set;}
    public UnityEvent blackoutFinishEvent {get; private set;}

    void Awake(){
        blackoutFinishEvent = new UnityEvent();
        blackoutHiddenEvent = new UnityEvent();
    }

    public void TriggerBlackoutEndEvent(){
        blackoutFinishEvent.Invoke();
        blackoutFinishEvent.RemoveAllListeners();
    }

    public void TriggerBlackoutHiddenEvent(){
        blackoutHiddenEvent.Invoke();
        blackoutHiddenEvent.RemoveAllListeners();
    }

    public void Pulse(){
        GetComponent<Animator>().SetTrigger("Pulse");
    }
}
