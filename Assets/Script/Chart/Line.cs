
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Line : MonoBehaviour
{
    public int number;
    public float bpm;
    public NotesAbove[] notesAbove;
    public NotesBelow[] notesBelow;
    public SpeedEvent[] speedEvents;
    public List<Note> allNotes; //
    public List<Note> lineNotes;//line下的所有note
    #region 数据队列
    public Queue<JudgeLineMoveEvent> moveEvents = new Queue<JudgeLineMoveEvent>();
    public Queue<JudgeLineEvent> rotateEvent = new Queue<JudgeLineEvent>();
    public Queue<JudgeLineEvent> disappearEvent = new Queue<JudgeLineEvent>();
    public Queue<SpeedEvent> speedEventQueue = new Queue<SpeedEvent>();
    #endregion
    public float X;
    public float Y;
    public float noteX;
    public float noteY;
    public float T;
    public float baseNoteSpeed = 1;
    public GameObject notePrefab;
    public SpriteRenderer lineSprite;
    public Dictionary<float, int> rhymeTimeDic;
    public JudgeLineMoveEvent curLineMoveEvent;
    public JudgeLineEvent curLineRotateEvent;
    public JudgeLineEvent curLineDisEvent; 
    float deque = 0;
    float deletTime = 0;
    float JudeXoffset = 15.85f;
    
    public void InstanceNotes(bool isRhyme)
    {
        bool isabove = true;
        foreach(var item in notesAbove)
        {
            Note note = GameObject.Instantiate(notePrefab).GetComponent<Note>();
            note.transform.parent = transform;
            note.fatherTransform = transform;   
            note.Int((NoteType)(item.Type-1),(float)item.Time,(float)item.PositionX,(float)item.HoldTime,(float)item.Speed,(float)item.FloorPosition,noteX,noteY,T,isabove); //选择相应的图片
            if (isRhyme)
            {
                if (rhymeTimeDic.ContainsKey(note.beatTime) && rhymeTimeDic[note.beatTime]>1)
                {
                    note.MultipleRhyme();
                    note.isNoteL = true;
                }
                
            }
            note.SwitchToNoteScaleParam(note.noteType, note.isNoteL);
            note.SetSpriteScale(PlayControl.Instance.dicNoteScalParam[note.noteScaleParam]);
            note.lineSpeedEventToNoteData(speedEvents);
            allNotes.Add(note);
            lineNotes.Add(note);
        }
        isabove = false;
        foreach(var item in notesBelow)
        {  
            Note note = GameObject.Instantiate(notePrefab).GetComponent<Note>();
            note.transform.parent = transform;
            note.fatherTransform = transform;
            note.Int((NoteType)(item.Type - 1), (float)item.Time, (float)item.PositionX, (float)item.HoldTime, (float)item.Speed, (float)item.FloorPosition, noteX, noteY, T, isabove);
            if (rhymeTimeDic.ContainsKey(note.beatTime) && rhymeTimeDic[note.beatTime] > 1)
            {
                note.MultipleRhyme();
                note.isNoteL = true;
            }
            note.SwitchToNoteScaleParam(note.noteType, note.isNoteL);
            note.SetSpriteScale(PlayControl.Instance.dicNoteScalParam[note.noteScaleParam]);
            note.lineSpeedEventToNoteData(speedEvents);
            allNotes.Add(note);
            lineNotes.Add(note);

        }
    }//初始化line下的所有Note。Ps：在chart初始化line时调用

    #region 判定线的运行数据更新

    public bool LineMoveEventUpdata(float  currentTime)
    {
        // 移除已经结束的事件
        
        JudgeLineMoveEvent currentEvent = moveEvents.Peek();
        float startTimerTemp = (float)currentEvent.StartTime * (1.875f / bpm);
        float endTimeTemp = (float)currentEvent.EndTime * (1.875f / bpm);
        if (currentTime >= endTimeTemp)
        {
            moveEvents.Dequeue(); // 弹出已完成的事件
            deque = endTimeTemp;
            deletTime = currentTime;

        }

        // 获取当前正在进行的事件
        JudgeLineMoveEvent activeEvent = moveEvents.Peek();
        curLineMoveEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // 计算线性插值
            //float t = (currentTime - startTime) / (endTime - startTime);
            //Vector3 startPosiiotn = new Vector3((float)activeEvent.Start * X, (float)activeEvent.Start2 * Y);
            //Vector3 endPosition = new Vector3((float)activeEvent.End * X, (float)activeEvent.End2 * Y);
            //return Vector3.Lerp(startPosiiotn, endPosition, t);
            return true;
        }
        //Debug.Log("第" + number + "判定线的移动事件：" + "开始" + startTime + "当前" + currentTime + "结束" + endTime + "前一个" + deque + "删除时的时间" + deletTime);
       
        return  false;
    }
    public bool LineRotateEventUpdata(float currentTime)
    {
        
        JudgeLineEvent currentEvent = rotateEvent.Peek();
        float startTimerTemp = (float)currentEvent.StartTime * (1.875f / bpm);
        float endTimeTemp = (float)currentEvent.EndTime * (1.875f / bpm);
        if (currentTime >= endTimeTemp)
        {
            rotateEvent.Dequeue(); // 弹出已完成的事件

        }
        JudgeLineEvent activeEvent = rotateEvent.Peek();
        curLineRotateEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // 计算线性插值
            //float t = (currentTime - startTime) / (endTime - startTime);
            //Vector3 startRotate = new Vector3(0,0,(float)activeEvent.Start);
            //Vector3 endRotate = new Vector3(0,0,(float)activeEvent.End);
            //return Vector3.Lerp(startRotate, endRotate, t);
            return true;
        }
        //Debug.Log("第" + number + "判定线的移动事件：" + "开始" + startTime + "当前" + currentTime + "结束" + endTime + "前一个" + deque + "删除时的时间" + deletTime);
        
        //return transform.eulerAngles;
             return false ;

    } 
     public bool LineFadeEventUpdata(float currentTime)
    {
       
        JudgeLineEvent currentEvent = disappearEvent.Peek();
        float startTimerTemp = (float)currentEvent.StartTime * (1.875f / bpm);
        float endTimeTemp = (float)currentEvent.EndTime * (1.875f / bpm);
        if (currentTime >= endTimeTemp)
        {
            disappearEvent.Dequeue(); // 弹出已完成的事件

        }
        JudgeLineEvent activeEvent = disappearEvent.Peek();
        curLineDisEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // 计算线性插值
            //float t = (currentTime - startTime) / (endTime - startTime);
            //float startA =  (float)activeEvent.Start;
            //float endA =  (float)activeEvent.End;
            //return Mathf.Lerp(startA, endA, t);
            return true;
        }
        //Debug.Log("第" + number + "判定线的移动事件：" + "开始" + startTime + "当前" + currentTime + "结束" + endTime + "前一个" + deque + "删除时的时间" + deletTime);
        //return lineSprite.color.a;
        return false;
    }
    #endregion     //当前各个事件数据的更新，并判断是否更新。
    public int GetSample(float time)
    {
        int sample = (int)((time * (1.875f / bpm)) * 44100);
        return sample;
    }

    public float GetTime(int sample)
    {
        return sample / 44100.0f;
    }
    
    public void RhymeDataToDic()
    {
        foreach(var note in notesAbove)
        {
            if (rhymeTimeDic.ContainsKey((float)note.Time))
            {
                rhymeTimeDic[(float)note.Time]++;
            }
            else
            {
                rhymeTimeDic.Add((float)note.Time,1);
            }
        }
        foreach(var note in notesBelow)
        {
            if (rhymeTimeDic.ContainsKey((float)note.Time))
            {
                rhymeTimeDic[(float)note.Time]++;
            }
            else
            {
                rhymeTimeDic.Add((float)note.Time, 1);
            }

        }
    }//如果是多押模式标记多押的note。Ps：在chart初始化line时调用

    public void JudeLineNotes(List<TouchData>touchDataList,float curTime)
    {
        if (touchDataList.Count == 0)
        {
            return;
        }//触摸数据为0则不进行判断
        foreach (var note in lineNotes)
        {
            float bTemp = note.beatTime * T - curTime;
            if (note.isover || (note.isbeat && note.noteType != NoteType.Hold)|| bTemp >= 0.2||(bTemp<-0.2 &&note.noteType!=NoteType.Hold) )
            {
                continue;
            }//没有isover的note和已点击了的note且不是hold则不进行判定
            
            foreach (TouchData touchData in touchDataList)
            {
                float touchX = transform.InverseTransformPoint(touchData.position).x;
                switch (note.noteType)
                {
                    case NoteType.Tap:
                        if (VecrticlJudeX(touchX, note.transform.localPosition.x)&&(touchData.type==TouchData.TouchType.Click) &&(!touchData.isJude))
                        {
                            touchData.isJude = true;
                            note.isTouch = true;
                            note.AutoJude(curTime);
                        }
                        break;
                    case NoteType.Hold:
                        if (!note.isbeat)
                        {
                            if (VecrticlJudeX(touchX, note.transform.localPosition.x)&& (touchData.type == TouchData.TouchType.Click) && (!touchData.isJude))
                            {
                                touchData.isJude = true;
                                note.isTouch = true;
                                note.AutoJude(curTime);
                            }
                        }
                        else
                        {
                            if(VecrticlJudeX(touchX, note.transform.localPosition.x))
                            {
                                note.isTouch = true;
                                note.AutoJude(curTime);
                            }
                            else
                            {
                                note.isTouch = false;
                                note.AutoJude(curTime);
                            }
                        }
                        break;
                    case NoteType.Drag:
                        if (VecrticlJudeX(touchX, note.transform.localPosition.x))
                        {
                            note.AutoJude(curTime);
                            note.isTouch = true;
                        }
                        break;
                    case NoteType.Flick:
                        if (FlickVecrticlJudeX(touchData, note.transform.localPosition.x)&&touchData.type == TouchData.TouchType.Swipe)
                        {
                            touchData.isJude=true;
                            note.isTouch = true;
                            note.AutoJude(curTime);

                        }
                        break;
                }
            }
           
        }

    }
   
    public bool VecrticlJudeX(float touchX , float noteX)
    {
        float minX = noteX - JudeXoffset;
        float maX = noteX+JudeXoffset;
        if (touchX >= minX && touchX <= maX)
        {
            return true;
        }
       
            return false;
    }
    public bool FlickVecrticlJudeX(TouchData data,float noteX)
    {
        float minX = noteX - JudeXoffset;
        float maX = noteX + JudeXoffset;
        float touchX = transform.InverseTransformPoint(data.position).x;
        float touchLastX = transform.InverseTransformPoint(data.lastPosition).x;

        if ((touchX < minX && touchLastX < minX) || (touchX > maX && touchLastX > maX))
        {
            return false;
        }

        return true;
    }
}







