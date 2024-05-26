using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class PlayerProfileIntroBehaviour
{
    TMP_Text nameInput;
    public UnityEvent<string> createEvent {get; private set;}

    // Start is called before the first frame update
    public PlayerProfileIntroBehaviour()
    {
        createEvent = new UnityEvent<string>();
        nameInput = GameObject.Find("Canvas").transform.Find("Foreground/Modal/Safe Area/Profile Creation/InputField (TMP)/Text Area/Text").GetComponent<TMP_Text>();
        GameObject.Find("Canvas").transform.Find("Foreground/Modal/Safe Area/Profile Creation/Button").GetComponent<Button>().onClick.AddListener(() => ProgressIntro());
        DebugLogger.SourcedPrint("Profile Intro", "AWAKE");
        ProgressIntro();
    }

    public void ProgressIntro(){
        DebugLogger.SourcedPrint("Profile Intro", "Len: " + nameInput.text.Trim().Length);
        if(nameInput.text.Trim().Length < 2) return; //CHECK IN ON THIS
        DebugLogger.SourcedPrint("Profile Intro", "Name: " + nameInput.text);
        createEvent.Invoke(nameInput.text);
        }
}
