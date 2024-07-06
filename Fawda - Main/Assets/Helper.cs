using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class Helper
{


    public static IEnumerator LerpStep(UnityAction<float> _setter, UnityAction _callback = null, float _time = 1, float _progress = 0){
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        _progress += Time.fixedDeltaTime;
        float intermediate = _progress/_time;
        _setter(intermediate);
        if(Mathf.Abs(1 - intermediate) < 0.05){
            _setter(1);
            if (_callback != null) _callback();
        }//else Orchestrator.singleton.StartCoroutine(LerpStep(_setter,_callback,_time,_progress));
    }


}
