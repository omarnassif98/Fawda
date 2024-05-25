using TMPro;
using UnityEngine;
using UnityEngine.Events;
public class PlayerProfileIntroBehaviour
{
    TMP_Text nameInput;
    public UnityEvent<string> createEvent {get; private set;}

    // Start is called before the first frame update
    public PlayerProfileIntroBehaviour()
    {
        createEvent = new UnityEvent<string>();
        //Summon Screen + Bind Button
    }

    public void ProgressIntro(){
        if(nameInput.text.Trim().Length == 0) return;
        createEvent.Invoke(nameInput.text);
        }
}
