using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SongData")]


[Serializable]
public class SongData_So : ScriptableObject
{
    [SerializeField]
   public List<ItemSong> song_List = new List<ItemSong>();
}
[Serializable]
public class ItemData{

   
}
[Serializable]
public class ItemSong:ItemData{ 

    public string songName;
    public string author;
    public Sprite image;
    public Sprite blurImage;
    [Header("难度设置")]
    public string esyRank;
    public string hardRank;
    public bool isIN;
    public string inRank;
    public bool isAT;
    public string atRank;
    public string songRank;
    [Header("歌曲资源")]
    public string chartPath;
    public AudioClip music;
}
