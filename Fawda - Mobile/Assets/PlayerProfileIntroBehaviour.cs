using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class PlayerProfileIntroBehaviour
{
    public Transform flow;
    TMP_Text nameInput;
    public UnityEvent<string> createEvent {get; private set;}

    // Start is called before the first frame update
    public PlayerProfileIntroBehaviour()
    {
        GameObject profileSetupFlowPrefab = Resources.Load("DeployableScreens/Profile Creation") as GameObject;
        createEvent = new UnityEvent<string>();

        flow = GameObject.Instantiate(profileSetupFlowPrefab,GameObject.Find("Canvas").transform.Find("Safe Area")).transform;
        nameInput = flow.Find("InputField (TMP)/Text Area/Text").GetComponent<TMP_Text>();
        flow.Find("Button").GetComponent<Button>().onClick.AddListener(() => ProgressIntro());
        DebugLogger.SourcedPrint("Profile Intro", "AWAKE");
    }

    public void ProgressIntro(){
        DebugLogger.SourcedPrint("Profile Intro", "Len: " + nameInput.text.Trim().Length);
        if(nameInput.text.Trim().Length < 2) return; //CHECK IN ON THIS
        DebugLogger.SourcedPrint("Profile Intro", "Name: " + nameInput.text);
        createEvent.Invoke(nameInput.text);
        }
}
