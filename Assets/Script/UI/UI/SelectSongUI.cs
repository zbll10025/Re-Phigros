using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SelectSongUI :BasePanel
{
    public SongData_So data_So; 
    public Transform nameCotentTransform;
    public Transform smallCotentTransform;
    public Transform bgCotentTrabsform;
    public List<SongItem> allSongItem = new List<SongItem>();   
    public List<Image> allSmallImage = new List<Image>();
    public List<Image> allBgImage = new List<Image>();
    [Header("UI放缩")]
    public float uiK=2f;
    #region 滑动行为的参数
    [Header("滑动的参数")]
    //滑动同步
    public ScrollRect nameScrollRect;
    public ScrollRect smallImageRect;
    public ScrollRect bgScrollRect;
    public RectTransform Mid;
    public float valueSmallImag = 565.8f;
    public float valueBG;
    public float K_nameTo_small;
    public float K_nameTo_BG;
    public RectTransform nameCotentRectTransform;
    //暂停同步
    public Vector2 lastPosition;
    public bool isStop;
    #endregion
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        Application.targetFrameRate = 144;
        _name = "SelectSongUI";
        IntScroll();
        IntUI(data_So);
    }
    public void IntUI(SongData_So songData)
    {
        foreach (ItemSong data in songData.song_List){

           SongItem item = PoolManger.Instance.Get("SongDescribe", "Prefabs/SongDescribe").GetComponent<SongItem>();
            item._name.text = data.songName;
            item.author.text = data.author;
            item.rank.text = data.esyRank;
            item.ui = this;
            Vector3 eur = new Vector3(0, 0, 20);
            Vector3 uiScale = new Vector3(uiK, uiK, uiK);
            allSongItem.Add(item);
            item.transform.SetParent(nameCotentRectTransform);
            Quaternion newQuat = Quaternion.Euler(eur);
            item.transform.localRotation = newQuat;
            //item.transform.localScale = uiScale;

            Image small = PoolManger.Instance.Get("SmallImage", "Prefabs/Image").GetComponent<Image>();
            small.sprite  =data.image;
            allSmallImage.Add(small);
            small.transform.SetParent(smallCotentTransform);
            small.transform.localRotation = newQuat;
            //small.transform.localScale = uiScale;

            Image bg = PoolManger.Instance.Get("BgImage", "Prefabs/Image").GetComponent<Image>();
            bg.sprite = data.blurImage;
            allBgImage.Add(bg);
            bg.transform.SetParent(bgCotentTrabsform);
            //bg.transform.localScale = uiScale;
        }
    }
    public void IntScroll()
    {
        //scrollRect = GetComponent<ScrollRect>();
        // 监听onValueChanged事件
        nameScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        nameCotentRectTransform = nameScrollRect.content;
        K_nameTo_small = ScrollToK(nameScrollRect, smallImageRect);
        K_nameTo_BG = ScrollToK(nameScrollRect, bgScrollRect);
        // valueSmallImag = smallImageRect.content.localPosition.y;
        valueBG =0;
        //CotentMidUpdate();
    }//初始化滑动。Ps:在创建时调用
    void OnScrollValueChanged(Vector2 value)
    {
        // 输出当前的滑动位置
        //Debug.Log("Scroll Rect Position: " + value);

        if (value == lastPosition)
        {
            isStop = true;
            CotentMidUpdate();
        }

        // 滚动位置发生了变化，执行你的逻辑
        lastPosition = value;
        //时时更新其他滚动条
        float tempy = (nameScrollRect.content.localPosition.y * K_nameTo_small) + valueSmallImag;
        //Debug.Log(nameScrollRect.content.localPosition.y +"  "+ tempy+" "+ valueSmallImag);
        smallImageRect.content.localPosition = new Vector2(smallImageRect.content.localPosition.x, tempy);
        tempy = (nameScrollRect.content.localPosition.y * K_nameTo_BG) + valueBG;
        bgScrollRect.content.localPosition = new Vector2(bgScrollRect.content.localPosition.x, tempy);
    }
    public void OnSelected(RectTransform item)
    {
        RectTransform viewportRect = nameScrollRect.viewport.GetComponent<RectTransform>();
        RectTransform contentRect = nameScrollRect.content.GetComponent<RectTransform>();
        Vector3 itemTo_viewPosition = item.localPosition + contentRect.localPosition;
        Vector3 offset = Mid.transform.localPosition - itemTo_viewPosition;
        Vector3 temp = contentRect.localPosition + offset;
        Vector3 finallPosition = new Vector3(contentRect.localPosition.x, temp.y, 0);
        contentRect.DOLocalMove(finallPosition, 0.25f);

    }
    public float ScrollToK(ScrollRect A, ScrollRect B)
    {
        float ACotentHeight = A.content.GetComponent<GridLayoutGroup>().cellSize.y;
        float ACotentSpacing = A.content.GetComponent<GridLayoutGroup>().spacing.y;
        float AValue = ACotentHeight + ACotentSpacing;

        float BCotentHeight = B.content.GetComponent<GridLayoutGroup>().cellSize.y;
        float BCotentSpacing = B.content.GetComponent<GridLayoutGroup>().spacing.y;
        //float BValue = BCotentHeight;
        float BValue = BCotentHeight + BCotentSpacing;
        float k = BValue / AValue;
        return k;
    }//计算不同滑动条的滑动比值

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
    }//寻找最接近中心点的内容并移动到中心

}
