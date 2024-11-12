using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class InputSystem : MonoBehaviour
{
    private List<TouchData> touchDataList = new List<TouchData>();
    public Camera cam ;
    public TextMeshProUGUI touchCount;
    public TextMeshProUGUI tapCount;
    public TextMeshProUGUI longCount;
    public TextMeshProUGUI moveCount;
    public int tapNum = 0;
    public int longPNum = 0;
    public int moveNum = 0;
    public Transform line1;
    public Transform line2;
    public Transform line3;
    public Transform obj;
    
    private void Awake()
    {
        Application.targetFrameRate = 144;
        Input.multiTouchEnabled = true;
        obj.SetParent(line1);
    }
    void Update()
    {
        touchDataList.Clear();
        tapNum = 0;
        longPNum = 0;
        moveNum = 0;
        Touch[] touches = Input.touches;
        foreach (Touch touch in touches) { 
            
            TouchData touchData = new TouchData(TouchData.TouchType.Click, touch.position, Vector2.zero, false);
            //Debug.Log(touch.position);
            Vector3 worldPosition  = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cam.nearClipPlane));
            Vector3 local = line1.InverseTransformPoint(worldPosition);
            obj.transform.localPosition = local;
            Debug.Log(local);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // 处理触摸开始事件
                    touchData.type = TouchData.TouchType.Click;
                    touchData.position = worldPosition;
                    tapNum++;
                    break;
                case TouchPhase.Moved:
                    // 处理触摸移动事件
                    touchData.type = TouchData.TouchType.Swipe;
                    Vector3 worldDele = cam.ScreenToWorldPoint(new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, cam.nearClipPlane));
                    touchData.lastPosition = worldPosition -worldDele;
                    touchData.position = worldPosition;
                    moveNum++;
                    break;
                case TouchPhase.Stationary:
                    // 处理触摸静止事件
                    touchData.type = TouchData.TouchType.LongPress;
                    touchData.position = worldPosition;
                    longPNum++;
                    break;
                case TouchPhase.Ended:
                    // 处理触摸结束事件
                    touchData.type = TouchData.TouchType.Click;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Canceled:
                    // 处理触摸取消事件
                    break;
            }
            touchDataList.Add(touchData);
        }
        touchCount.text = "AllCount:" + touchDataList.Count.ToString();
        tapCount.text = "tapCount:" + tapNum.ToString();
        longCount.text = "longCount:"+longPNum.ToString();
        moveCount.text = "moveCount:" + moveNum.ToString();
    }


}
