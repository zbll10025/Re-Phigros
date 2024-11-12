using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Note : MonoBehaviour
{
    public Transform fatherTransform;
    public float X; //相对父物体x轴的单位
    public float Y;//相对父物体y轴的单位
    public float T;//事件的时间单位
    public bool isAbove;
    public NoteType noteType;
    public float beatTime;
    public float positionX;
    public float holdTime;
    public float speed;
    public float nH; // 与计算holdNote的长度有关
    public float currentBaseSpeed;
    public float floorPosition;
    public SpriteRenderer noteSpritRender;
    public EffectBeat temPeffectBeat;//临时的值与按住hold特效有关
    public NoteScaleParam noteScaleParam;
    public bool isNoteL;
    public bool isbeat;
    public bool isover;
    public string Effectpath;
    public NoteData curNoteData;
    public bool isTouch;
    public Color badMissColor;
    public JudeType judeType;
    #region 图片资源
    public Sprite tapSprite;
    public Sprite tapSpriteLight;
    public Sprite holdSprit;
    public Sprite holdSpriteLight;
    public Sprite DragSprite;
    public Sprite DragSpriteLight;
    public Sprite FlickSprite;
    public Sprite FlickSpriteLight;
    #endregion
     
    #region note垂直位移数据
    public Queue<NoteData> noteDatas = new Queue<NoteData>();
    #endregion

    public  void UpdataNote(float curTime)
    {
        //float curTime = PlayControl.Instance.currentTime;
        if (!isbeat)
        {
        //点击不管什么类型的note都不再移动
        //transform.localPosition =GetCureentPosition(curTime);
        }
        //如果是hold则在点击判定后不再移动
        //AutoJude(curTime);
       //UniTask.Delay(10);
    }

    public void Int(NoteType noteType,float beaTime,float positionX,float holdTime,float speed,float floorPosion,float X,float Y,float T,bool isabove)
    {
        this.noteType = noteType;
        this.beatTime = beaTime;
        this.positionX = positionX;
        this.holdTime = holdTime;
        this.speed = speed;
        this.floorPosition = floorPosion;
        this.X = X;
        this.Y = Y;
        this.T = T;
        this.isAbove = isabove;
        if(noteType == NoteType.Tap)
        {
            noteSpritRender.sprite =  tapSprite;
        }
        else if(noteType == NoteType.Drag)
        {
            noteSpritRender.sprite = DragSprite;
        }
        else if(noteType == NoteType.Flick){
            noteSpritRender.sprite= FlickSprite;
        }
        else if(noteType == NoteType.Hold)
        {
            nH = this.speed;
            this.speed = 1;
            //noteSprit.color = Color.blue;
            IntHold();
        }

    }
    public void IntHold()
    {
        float h = holdTime*T*nH*Y;
        //Debug.Log(h);  
        //float spriteHeight = holdSprit.rect.height;
        //float scaleFactor = (h*100)/ spriteHeight; //100px是图片与画面转化的单位
        noteSpritRender.sprite = holdSprit;
        noteSpritRender.gameObject.transform.localPosition = new Vector3(0, h, 0);
        if (!isAbove)
        {
            noteSpritRender.gameObject.transform.localPosition = new Vector3(0, -h, 0);
            noteSpritRender.gameObject.transform.localScale = new Vector3(1, -1, 1);

        }
        noteSpritRender.size = new Vector2(30f, h); 
        //noteSpritRender.gameObject.transform.localScale = new Vector3(noteSpritRender.gameObject.transform.localScale.x, scaleFactor, 1f);
    }
    //如果是hold则改变大小
    public void lineSpeedEventToNoteData(SpeedEvent[] speedEvents )
    {
        //Debug.Log(T);
        float sum = 0;
        bool isFind = false;
        foreach (var item in speedEvents){
            NoteData data = new NoteData();
            float preDistance = sum;
            data.Int((float)item.StartTime, (float)item.EndTime, (float)item.Value, preDistance);
            noteDatas.Enqueue(data);
            if (!isFind)
            {
                
                if (beatTime * T < item.EndTime * T)
                {
                    isFind = true;
                    //Debug.Log("111111111111");
                    floorPosition = sum + (beatTime - (float)item.StartTime) * T * speed * (float)item.Value * Y;
                    if (isAbove)
                    {
                    transform.localPosition = new Vector3(positionX * X, floorPosition, 0);
                    }
                    else
                    {
                        floorPosition = -floorPosition;
                        transform.localPosition = new Vector3(positionX * X, floorPosition, 0);

                    }
                }
            }
            sum += (float)((item.EndTime - item.StartTime)*T * speed * item.Value * Y);
            
        }
        curNoteData = noteDatas.Peek();
    }

    public void NotDataeUpData(float currentTime)
    {
       
        NoteData noteData = noteDatas.Peek();
        float endTimeTemp = noteData.EndTime*T;
        if (currentTime >= endTimeTemp)
        {
            noteDatas.Dequeue();
        }
         curNoteData = noteDatas.Peek();
        #region 废案
        //currentBaseSpeed = curNoteData.Value;
        //float startTime = curNoteData.StartTime*T;
        //float endTime = curNoteData.EndTime*T;
        //float baseSpeed = curNoteData.Value;
        //float hasMoveDistance = 0;
        //float positionY;
        //if (currentTime >= startTime && currentTime < endTime)
        //{
        //    float durTime =  currentTime - startTime;
        //    hasMoveDistance = durTime * speed * baseSpeed * Y;
        //}
        //hasMoveDistance += curNoteData.PreDistance;
        //if (isAbove)
        //{
        //    positionY = floorPosition - hasMoveDistance;
        //}
        //else
        //{
        //    positionY = floorPosition + hasMoveDistance;
        //}
        //return new Vector3(positionX*X, positionY, 0);

        #endregion
    }//
    public void LifeJUde(float curTime)
    {
        if(isover)
        {
            return;
        }
        if (noteType != NoteType.Hold)
        {
            if ((beatTime * T)+0.16f <= curTime)
            {
                isover = true;
                isbeat = true;
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            if (curTime>=beatTime*T)
            {
                isbeat = true;
            }
            if (curTime >= (beatTime * T)+0.16f && curTime < ((beatTime + holdTime) * T))
            {
                if(judeType == JudeType.none)
                {
                    noteSpritRender.color = badMissColor;
                    judeType = JudeType.miss;
                }
            }else if(curTime>= ((beatTime + holdTime) * T))
            {
                    isover = true;
                if (temPeffectBeat != null)
                {
                    temPeffectBeat.isOverHold = true;
                }
                    this.gameObject.SetActive(false);
                return;
            }
            HoldUpdata(curTime);
        }
    }//检测note的生命周期
    public void BadJude()
    {
        isbeat =true;
        isover =true;
        noteSpritRender.DOFade(0, 0.2f).OnComplete(()=>{
            this.gameObject.SetActive(false);
        });
        //hold的处理
        //comb清零
    }
    public JudeType JUdeTime(float curTime)
    {
        //float preTemp = note.beatTime * T - curTime;
        float temp =math.abs(beatTime * T - curTime);
        JudeType judeType;
        if (temp > 0.16)
        {
            judeType = JudeType.bad;
        }
        else if (temp <= 0.16f && temp >= 0.8)
        {
            judeType = JudeType.good;
        }
        else
        {
            judeType = JudeType.prefect;
        }
        return judeType;
    }
    public  void AutoJude(float curTime)
    {

        if (noteType != NoteType.Hold)
        {
            if (!isbeat)
            {
                switch (noteType){
                
                    case NoteType.Tap:
                        judeType = JUdeTime(curTime);
                        GetBeatEffect();
                        break;
                    case NoteType.Flick:
                        judeType = JUdeTime(curTime);
                        if(judeType == JudeType.bad)
                        {
                            judeType = JudeType.none;
                            return ;
                        }
                        judeType = JudeType.prefect;
                        GetBeatEffect(); 
                        break;
                    case NoteType.Drag:
                        judeType= JUdeTime(curTime);
                        if (judeType == JudeType.bad)
                        {
                            judeType = JudeType.none;
                            return;
                        }
                        judeType = JudeType.prefect;
                        GetBeatEffect();
                        break;

                }

                if (JUdeTime(curTime)==JudeType.prefect)
                GetBeatEffect();
            }
            isbeat = true;
            isover = true;
            //await UniTask.Delay(250);
            this.gameObject.SetActive(false);
        }
        else {
            if (curTime >= ((beatTime+holdTime) * T))
            {
                if (!isover)
                {
                    isover = true;
                    if (temPeffectBeat != null)
                    {
                        temPeffectBeat.isOverHold = true;
                    }
                    this.gameObject.SetActive(false);

                }
                
            }else if(curTime>=(beatTime*T) && curTime< ((beatTime + holdTime) * T))
            {
                if (judeType==JudeType.none)
                {
                judeType = JUdeTime(curTime);       
                }
                if(judeType == JudeType.miss || judeType == JudeType.bad)
                {
                    noteSpritRender.color = badMissColor;
                    if (temPeffectBeat != null)
                    {
                        temPeffectBeat.isOverHold = true;
                    }
                    //ChangeHoldHeight(curTime);
                    isbeat = true;
                    return;
                }
                else
                {
                    if (temPeffectBeat == null)
                    {
                        temPeffectBeat = PoolManger.Instance.Get("EffectBeat", "Prefabs/EffectBeat").GetComponent<EffectBeat>();
                        temPeffectBeat.isHold = true;//与其他类型的区别点
                        temPeffectBeat.transform.SetParent(transform.parent);
                        temPeffectBeat.transform.localPosition =new Vector3(transform.localPosition.x,0,0);
                        temPeffectBeat.gameObject.SetActive(true);
                            if (judeType == JudeType.prefect)
                            {
                                temPeffectBeat.SetPrefectEffect(noteType);
                            }
                            else{ 
                    
                                temPeffectBeat.SetGreatEffect(noteType);
                            }
                    }
                    isbeat = true;
                    //ChangeHoldHeight(curTime);
                }
            }
        }
    }//判定是否点击
    public void GetBeatEffect()
    {
        if(judeType == JudeType.bad)
        {
            BadJude();
            return;
        }else if(judeType == JudeType.none||judeType == JudeType.miss){

            return;      
        }
        EffectBeat effectBeat = PoolManger.Instance.Get("EffectBeat", "Prefabs/EffectBeat").GetComponent<EffectBeat>();
        effectBeat.transform.SetParent(transform.parent);
        effectBeat.transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
        effectBeat.gameObject.SetActive(true);
        if (judeType == JudeType.prefect)
        {
            effectBeat.SetPrefectEffect(noteType);

        }
        else
        {
            effectBeat.SetGreatEffect(noteType);
        }
        noteSpritRender.gameObject.SetActive(false);
    }
    public void HoldUpdata(float curTime)
    {
        if (curTime >= ((beatTime + holdTime) * T))
        {
            if (!isover)
            {
                isover = true;
                if (temPeffectBeat != null)
                {
                    temPeffectBeat.isOverHold = true;
                }
                this.gameObject.SetActive(false);

            }

        }
        else if (curTime >= (beatTime * T) && curTime < ((beatTime + holdTime) * T))
        { 
            if (judeType == JudeType.miss || judeType == JudeType.bad)
            {
                noteSpritRender.color = badMissColor;
                if (temPeffectBeat != null)
                {
                    temPeffectBeat.isOverHold = true;
                }
            }
            ChangeHoldHeight(curTime);
        }
    }
    public void ChangeHoldHeight(float curTime)
    {
        //Debug.Log(11111);
        float h = ( (holdTime * T) - (curTime - (beatTime*T)) )* nH * Y;//总长
        if (curTime >= (beatTime + holdTime) * T)
        {
            h = 0;
        }
        //float spriteHeight = holdSprit.rect.height;
        //float scaleFactor = (h * 100) / spriteHeight;
        //noteSpritRender.gameObject.transform.localScale = new Vector3(noteSpritRender.gameObject.transform.localScale.x, scaleFactor, 1f);
        noteSpritRender.size = new Vector2(noteSpritRender.size.x, h);
        //多押长条有误差位置与长度误差为5
        //if (isNoteL&&h>=1000)
        //{
        //    h += 5;
        //}

        noteSpritRender.gameObject.transform.localPosition = new Vector3(0, h, 0);
        if (!isAbove)
        {
           noteSpritRender.gameObject.transform.localPosition = new Vector3(0, -h, 0);
        }
    }//根据时间更新长条长度

    public void MultipleRhyme()
    {
        switch (noteType){ 
        
         case NoteType.Tap:
                noteSpritRender.sprite = tapSpriteLight;
                break;
        case NoteType.Hold:
                noteSpritRender.sprite= holdSpriteLight;
                break;
        case NoteType.Flick:
                noteSpritRender.sprite = FlickSpriteLight;
                break;
        case NoteType.Drag:
                noteSpritRender.sprite = DragSpriteLight;
                break;
        }

    }//如果要多押提醒则改变相应的图片
    
    public void SwitchToNoteScaleParam(NoteType noteTyp,bool isRhyme)
    {
        switch (noteTyp){ 
            case NoteType.Tap:
                   noteScaleParam = NoteScaleParam.Tap;
                   if (isRhyme) { noteScaleParam = NoteScaleParam.TapL; }
                   break;
            case NoteType.Hold:
                noteScaleParam = NoteScaleParam.Hold;
                if (isRhyme) { noteScaleParam = NoteScaleParam.HoldL; }
                break;
            case NoteType.Flick:
                noteScaleParam = NoteScaleParam.Flick;
                if (isRhyme) { noteScaleParam = NoteScaleParam.FlickL; }
                break;
            case NoteType.Drag:
                noteScaleParam = NoteScaleParam.Drag;
                if (isRhyme) { noteScaleParam = NoteScaleParam.DragL; }
                break;
        }

    }//更具类型，是否多押来调整图片初始参数的接口。Ps：note初始化时调用

    public void SetSpriteScale(NoteScaleData data)
    {
        if(noteType!=NoteType.Hold)
        noteSpritRender.size = new Vector2(data.width, data.height);
    }//Ps：note初始化时调用



}
public class NoteData{
    public float StartTime { get; set; }

    public float EndTime { get; set; }

    public float Value { get; set; }

    public float PreDistance;

    public void Int(float startTime,float EndTime,float value,float PreDistance)
    {
        this.StartTime = startTime;
        this.EndTime   = EndTime;
        this.Value = value; 
        this.PreDistance = PreDistance;
    }

}
