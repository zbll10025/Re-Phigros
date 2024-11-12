using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;


public class ScrollRectController : MonoBehaviour
{
    public ScrollRect nameScrollRect;
    public ScrollRect smallImageRect;
    public ScrollRect bgScrollRect;
    public RectTransform Mid;

    public Vector2 lastPosition;
    public bool isStop;

    public RectTransform nameCotentRectTransform;

    public  float valueSmallImag = 768.35f;
    public float valueBG;
    public float K_nameTo_small;
    public float K_nameTo_BG;
    void Start()
    {
        //scrollRect = GetComponent<ScrollRect>();
        // ����onValueChanged�¼�
        nameScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        nameCotentRectTransform = nameScrollRect.content;
        K_nameTo_small = ScrollToK(nameScrollRect, smallImageRect);
        K_nameTo_BG = ScrollToK(nameScrollRect, bgScrollRect);
       // valueSmallImag = smallImageRect.content.localPosition.y;
        valueBG  =bgScrollRect.content.localPosition.y;
        CotentMidUpdate();
    }


    void OnScrollValueChanged(Vector2 value)
    {
        // �����ǰ�Ļ���λ��
        //Debug.Log("Scroll Rect Position: " + value);

        if (value == lastPosition)
        {
           isStop = true;
           CotentMidUpdate();
        }

        // ����λ�÷����˱仯��ִ������߼�
        // ...
        lastPosition = value;
        //ʱʱ��������������
        float tempy = (nameScrollRect.content.localPosition.y*K_nameTo_small)+valueSmallImag;
        smallImageRect.content.localPosition = new Vector2(smallImageRect.content.localPosition.x, tempy);
        tempy = (nameScrollRect.content.localPosition.y * K_nameTo_BG) + valueBG;
        bgScrollRect.content.localPosition = new Vector2(bgScrollRect.content.localPosition.x, tempy);
    }
    public void OnSelected(RectTransform item)
    {
        RectTransform viewportRect = nameScrollRect.viewport.GetComponent<RectTransform>();
        RectTransform contentRect = nameScrollRect.content.GetComponent<RectTransform>();
        Vector3 itemTo_viewPosition = item.localPosition+contentRect.localPosition;
        Vector3 offset = Mid.transform.localPosition - itemTo_viewPosition;
        Vector3 temp = contentRect.localPosition + offset;
        Vector3 finallPosition = new Vector3(contentRect.localPosition.x, temp.y, 0);
        contentRect.DOLocalMove(finallPosition,0.25f);
       
    }
    public float ScrollToK(ScrollRect A , ScrollRect B)
    {
        float ACotentHeight = A.content.GetComponent<GridLayoutGroup>().cellSize.y;
        float ACotentSpacing = A.content.GetComponent<GridLayoutGroup>().spacing.y;
        float AValue = ACotentHeight + ACotentSpacing;

        float BCotentHeight = B.content.GetComponent<GridLayoutGroup>().cellSize.y;
        float BCotentSpacing = B.content.GetComponent<GridLayoutGroup>().spacing.y;
        float BValue = BCotentHeight + BCotentSpacing;
        float k = BValue / AValue;
        return k;
    }//���㲻ͬ�������Ļ�����ֵ

    public void CotentMidUpdate()
    {
        float miny = 1000000000000000000000000000f;
        RectTransform finallResult = null;
        foreach (Transform t in nameCotentRectTransform)
        {
            if (t != null)
            {

                Vector3 toT = t.localPosition + nameCotentRectTransform.localPosition;
                Vector3 offset = Mid.transform.localPosition - toT;
                if (Math.Abs(offset.y) <= Math.Abs(miny))
                {
                    miny = offset.y;
                    finallResult = t as RectTransform;
                }
            }
        }
        OnSelected(finallResult);
    }//Ѱ����ӽ����ĵ�����ݲ��ƶ�������
}
