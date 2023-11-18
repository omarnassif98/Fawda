using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackgroundBehaviour : MonoBehaviour
{
    Vector2 driftDir;
    [SerializeField]float idealDriftSpeed = 75, realDriftSpeed, idealDriftAngle = 35, realDriftAngle;
    Vector2 posOffset;
    [SerializeField] Sprite backgroundImage;
    // 3x3 grid
    ///  0 1 2
    ///  3 4 5
    ///  6 7 8
    // everything is defined as an offset to center
    [SerializeField] Image[] images;
    RectTransform center;
    [SerializeField] Vector2[] offsetMultipliers;
    void Start()
    {
        if (images.Length != offsetMultipliers.Length){
            Debug.LogError("THERE IS SOMETHING SERIOUSLY WRONG WITH THE BACKGROUND");
        }
        realDriftSpeed = idealDriftSpeed;
        realDriftAngle = idealDriftAngle;
        driftDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * realDriftAngle), Mathf.Sin(Mathf.Deg2Rad * realDriftAngle));
        center = images[4].GetComponent<RectTransform>();
        posOffset = transform.parent.GetComponent<CanvasScaler>().referenceResolution;
        UIManager.singleton.screenTransitionEvent.AddListener(() => StartCoroutine(JoltBackground()));
        for(int i = 0; i < images.Length; i++){
            images[i].sprite = backgroundImage;
            images[i].pixelsPerUnitMultiplier = 4;
            images[i].GetComponent<RectTransform>().sizeDelta = posOffset;
        }
    }


    IEnumerator JoltBackground(){
        idealDriftSpeed = 550;
        yield return new WaitForSeconds(0.95f);
        idealDriftSpeed = 75;
    }
    void RestructureScreens(){
        Image[] temp = new Image[3];
        if (center.anchoredPosition.x >= posOffset.x/2){
            temp[0] = images[2];
            temp[1] = images[5];
            temp[2] = images[8];
            
            
            for(int i = 0; i<3;i++){
                images[2 + 3*i] = images[1 + 3*i];
                images[1 + 3*i] = images[0 + 3*i];
                images[0 + 3*i] = temp[i];
            }
        }

        if (center.anchoredPosition.x < -posOffset.x/2){
            temp[0] = images[0];
            temp[1] = images[3];
            temp[2] = images[6];
            
            for(int i = 0; i<3;i++){
                images[0 + 3*i] = images[1 + 3*i];
                images[1 + 3*i] = images[2 + 3*i];
                images[2 + 3*i] = temp[i];
            }
        }

        if (center.anchoredPosition.y < -posOffset.y/2){
            temp[0] = images[6];
            temp[1] = images[7];
            temp[2] = images[8];
            
            for(int i = 0; i<3;i++){
                images[6 + 1*i] = images[3 + 1*i];
                images[3 + 1*i] = images[0 + 1*i];
                images[0 + 1*i] = temp[i];
            }
        }

        if (center.anchoredPosition.y > posOffset.y/2){
            temp[0] = images[0];
            temp[1] = images[1];
            temp[2] = images[2];
            
            for(int i = 0; i<3;i++){
                images[0 + 1*i] = images[3 + 1*i];
                images[3 + 1*i] = images[6 + 1*i];
                images[6 + 1*i] = temp[i];
            }
        }
     center = images[4].GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        realDriftSpeed = Mathf.Lerp(realDriftSpeed,idealDriftSpeed,0.07f);
        realDriftAngle = idealDriftAngle + Mathf.Sin(Time.time/4) * 30;
        driftDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * realDriftAngle), Mathf.Sin(Mathf.Deg2Rad * realDriftAngle));
        Vector2 right = center.right;
        Vector2 up = center.up;
        center.anchoredPosition += driftDir * realDriftSpeed * Time.deltaTime;
        for(int i = 0; i < images.Length; i++){
            images[i].GetComponent<RectTransform>().up = up;
            images[i].GetComponent<RectTransform>().anchoredPosition = center.anchoredPosition + right * Vector2.Scale(offsetMultipliers[i],posOffset).x + up * Vector2.Scale(offsetMultipliers[i],posOffset).y;
        }
        RestructureScreens();
    }
}
