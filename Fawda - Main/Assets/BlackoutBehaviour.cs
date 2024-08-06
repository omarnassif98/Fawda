using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class BlackoutBehaviour : MonoBehaviour
{
    Image foreground;
    Animator animator;

    public void Awake()
    {
        foreground = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }
    UnityAction callback = null;

    public void Pulse(UnityAction _callback = null)
    {
        callback = _callback;
        animator.SetTrigger("pulse");
    }

    public void EndPulse()
    {
        if (callback != null) callback();
        callback = null;
    }


}
