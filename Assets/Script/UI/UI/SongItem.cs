using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongItem : MonoBehaviour
{
    public Image buttonImage;
    public Button buttonButton;
    public TextMeshProUGUI _name;
    public TextMeshProUGUI author;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI RabkNum;
    public SelectSongUI ui;
    public void SelectEffect()
    {
        buttonImage.DOFade(0.2f, 0.1f).OnComplete(() =>
        {
            buttonImage.DOFade(0, 0.1f);
        });

    }
    public void SelectSong(){

        ui.OnSelected(transform as RectTransform);
    
    }
    public void Clear()
    {
        _name.text = "";
        author.text = "";
        rank.text = "";
        RabkNum.text = "";
    }
}
