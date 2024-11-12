using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBeat : MonoBehaviour
{
    public Animator anim;
    public AudioClip tapAudio;
    public AudioClip dragAudio;
    public AudioClip flickAudio;
    //public GameObject note;
    //public bool isFirstplay = true;
    public bool isHold;
    public bool isOverHold;
    void Start()
    {
        //SetGreatEffect(NoteType.Tap);
    }

    
    void Update()
    {
        
    }

    public void SetGreatEffect(NoteType noteType)
    {
        //anim = GetComponent<Animator>();
        SetPlayMusic(noteType);
        anim.SetBool("isGreat", true);

    }
    public void SetPrefectEffect(NoteType noteType)
    {
        //anim = GetComponent<Animator>();
        SetPlayMusic(noteType);
        anim.SetBool("isPrefect", true);
    }
    public void AnimOver()
    {
        if (!isHold)
        {
            RycleClear();
            //不是hold类型则直接回收
        }
        else
        {
            //是hold类型判断长按是否结束，结束则回收
            if (isOverHold)
            {
                //Debug.Log("111111111111111");
                RycleClear();
            }
        }
        //RycleClear();
       
    }
    public void SetPlayMusic(NoteType noteType)
    {
        switch (noteType) { 
            case NoteType.Tap:
                PlayControl.Instance.PlayerEffectBeat(tapAudio);
                break;
            case NoteType.Drag:
                PlayControl.Instance.PlayerEffectBeat(dragAudio);
                break;
            case NoteType.Flick:
                PlayControl.Instance.PlayerEffectBeat(flickAudio);
                break;
            case NoteType.Hold:
                PlayControl.Instance.PlayerEffectBeat(tapAudio);
                isHold = true;
                break;

        }
    }

    public void RycleClear()
    {
        isHold = false;
        isOverHold = false;
        transform.parent = null;
        anim.SetBool("isPrefect", false);
        anim.SetBool("isGreat", false);
        transform.rotation = Quaternion.identity;
        PoolManger.Instance.Recycle("EffectBeat", this.gameObject);

    }
}
