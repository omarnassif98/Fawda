using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackgroundBehaviour : MonoBehaviour
{
    Vector2 driftDir;
    public float idealDriftSpeed = 75, idealDriftAngle = 35, idealCheckerboardOpacity = 0.2f;
    float realDriftAngle = 0, realDriftSpeed = 0, realCheckerboardOpacity = 0;
    Vector2 posOffset;
    [SerializeField] Sprite backgroundImage;
    // 3x3 grid
    ///  0 1 2
    ///  3 4 5
    ///  6 7 8
    // everything is defined as an offset to center
    [SerializeField] Image[] images;
    RectTransform center;
    Vector2[] offsetMultipliers =
    {
        Vector2.up + Vector2.left,
        Vector2.up,
        Vector2.up + Vector2.right,
        Vector2.left,
        Vector2.zero,
        Vector2.right,
        Vector2.down + Vector2.left,
        Vector2.down,
        Vector2.down + Vector2.right
    };
    void Start()
    {
        if (images.Length != offsetMultipliers.Length){
            DebugLogger.singleton.Log("THERE IS SOMETHING SERIOUSLY WRONG WITH THE BACKGROUND");
        }
        realDriftSpeed = idealDriftSpeed;
        realDriftAngle = idealDriftAngle;
        driftDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * realDriftAngle), Mathf.Sin(Mathf.Deg2Rad * realDriftAngle));
        center = images[4].GetComponent<RectTransform>();
        posOffset = transform.parent.GetComponent<CanvasScaler>().referenceResolution;
        UIManager.singleton.screenTransitionEvent.AddListener(JoltBackground);
        for(int i = 0; i < images.Length; i++){
            images[i].sprite = backgroundImage;
            images[i].GetComponent<RectTransform>().sizeDelta = posOffset;
        }
    }

    public void JoltBackground() => StartCoroutine(JoltBackgroundCoroutine());

    IEnumerator JoltBackgroundCoroutine(){
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
        if (Mathf.Abs(idealDriftSpeed - realDriftSpeed) > 0.1f) realDriftSpeed = Mathf.Lerp(realDriftSpeed,idealDriftSpeed,0.07f);
        else realDriftSpeed = idealDriftSpeed;

        if (Mathf.Abs(idealCheckerboardOpacity - realCheckerboardOpacity) > 0.01f) realCheckerboardOpacity = Mathf.Lerp(realCheckerboardOpacity,idealCheckerboardOpacity,0.10f);
        else realCheckerboardOpacity = idealCheckerboardOpacity;

        realDriftAngle = idealDriftAngle + Mathf.Sin(Time.time/4) * 30;
        driftDir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * realDriftAngle), Mathf.Sin(Mathf.Deg2Rad * realDriftAngle));
        Vector2 right = center.right;
        Vector2 up = center.up;
        center.anchoredPosition += driftDir * realDriftSpeed * Time.deltaTime;

        for(int i = 0; i < images.Length; i++){
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, realCheckerboardOpacity);
            images[i].GetComponent<RectTransform>().anchoredPosition = center.anchoredPosition + right * Vector2.Scale(offsetMultipliers[i],posOffset).x + up * Vector2.Scale(offsetMultipliers[i],posOffset).y;
        }
        RestructureScreens();
    }
}
