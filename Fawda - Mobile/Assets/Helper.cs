using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class Helper
{


    public static IEnumerator LerpStep(UnityAction<float> _setter, UnityAction _callback = null, float _time = 1, float _progress = 0){
        yield return new WaitForSeconds(1/60);
        float intermediate = Mathf.Lerp(_progress,1,1/(_time * 60f));

        _setter(intermediate);
        if(Mathf.Abs(1 - intermediate) < 0.05){
            _setter(1);
            if (_callback != null) _callback();
        }else Orchestrator.singleton.StartCoroutine(LerpStep(_setter,_callback,_time,intermediate));

    }
}
