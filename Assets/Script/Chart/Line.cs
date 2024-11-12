
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
    public List<Note> lineNotes;//line�µ�����note
    #region ���ݶ���
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
            note.Int((NoteType)(item.Type-1),(float)item.Time,(float)item.PositionX,(float)item.HoldTime,(float)item.Speed,(float)item.FloorPosition,noteX,noteY,T,isabove); //ѡ����Ӧ��ͼƬ
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
    }//��ʼ��line�µ�����Note��Ps����chart��ʼ��lineʱ����

    #region �ж��ߵ��������ݸ���

    public bool LineMoveEventUpdata(float  currentTime)
    {
        // �Ƴ��Ѿ��������¼�
        
        JudgeLineMoveEvent currentEvent = moveEvents.Peek();
        float startTimerTemp = (float)currentEvent.StartTime * (1.875f / bpm);
        float endTimeTemp = (float)currentEvent.EndTime * (1.875f / bpm);
        if (currentTime >= endTimeTemp)
        {
            moveEvents.Dequeue(); // ��������ɵ��¼�
            deque = endTimeTemp;
            deletTime = currentTime;

        }

        // ��ȡ��ǰ���ڽ��е��¼�
        JudgeLineMoveEvent activeEvent = moveEvents.Peek();
        curLineMoveEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // �������Բ�ֵ
            //float t = (currentTime - startTime) / (endTime - startTime);
            //Vector3 startPosiiotn = new Vector3((float)activeEvent.Start * X, (float)activeEvent.Start2 * Y);
            //Vector3 endPosition = new Vector3((float)activeEvent.End * X, (float)activeEvent.End2 * Y);
            //return Vector3.Lerp(startPosiiotn, endPosition, t);
            return true;
        }
        //Debug.Log("��" + number + "�ж��ߵ��ƶ��¼���" + "��ʼ" + startTime + "��ǰ" + currentTime + "����" + endTime + "ǰһ��" + deque + "ɾ��ʱ��ʱ��" + deletTime);
       
        return  false;
    }
    public bool LineRotateEventUpdata(float currentTime)
    {
        
        JudgeLineEvent currentEvent = rotateEvent.Peek();
        float startTimerTemp = (float)currentEvent.StartTime * (1.875f / bpm);
        float endTimeTemp = (float)currentEvent.EndTime * (1.875f / bpm);
        if (currentTime >= endTimeTemp)
        {
            rotateEvent.Dequeue(); // ��������ɵ��¼�

        }
        JudgeLineEvent activeEvent = rotateEvent.Peek();
        curLineRotateEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // �������Բ�ֵ
            //float t = (currentTime - startTime) / (endTime - startTime);
            //Vector3 startRotate = new Vector3(0,0,(float)activeEvent.Start);
            //Vector3 endRotate = new Vector3(0,0,(float)activeEvent.End);
            //return Vector3.Lerp(startRotate, endRotate, t);
            return true;
        }
        //Debug.Log("��" + number + "�ж��ߵ��ƶ��¼���" + "��ʼ" + startTime + "��ǰ" + currentTime + "����" + endTime + "ǰһ��" + deque + "ɾ��ʱ��ʱ��" + deletTime);
        
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
            disappearEvent.Dequeue(); // ��������ɵ��¼�

        }
        JudgeLineEvent activeEvent = disappearEvent.Peek();
        curLineDisEvent = activeEvent;
        float startTime = (float)activeEvent.StartTime * (1.875f / bpm);
        float endTime = (float)activeEvent.EndTime * (1.875f / bpm);
        if (currentTime >= startTime && currentTime < endTime)
        {
            // �������Բ�ֵ
            //float t = (currentTime - startTime) / (endTime - startTime);
            //float startA =  (float)activeEvent.Start;
            //float endA =  (float)activeEvent.End;
            //return Mathf.Lerp(startA, endA, t);
            return true;
        }
        //Debug.Log("��" + number + "�ж��ߵ��ƶ��¼���" + "��ʼ" + startTime + "��ǰ" + currentTime + "����" + endTime + "ǰһ��" + deque + "ɾ��ʱ��ʱ��" + deletTime);
        //return lineSprite.color.a;
        return false;
    }
    #endregion     //��ǰ�����¼����ݵĸ��£����ж��Ƿ���¡�
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
    }//����Ƕ�Ѻģʽ��Ƕ�Ѻ��note��Ps����chart��ʼ��lineʱ����

    public void JudeLineNotes(List<TouchData>touchDataList,float curTime)
    {
        if (touchDataList.Count == 0)
        {
            return;
        }//��������Ϊ0�򲻽����ж�
        foreach (var note in lineNotes)
        {
            float bTemp = note.beatTime * T - curTime;
            if (note.isover || (note.isbeat && note.noteType != NoteType.Hold)|| bTemp >= 0.2||(bTemp<-0.2 &&note.noteType!=NoteType.Hold) )
            {
                continue;
            }//û��isover��note���ѵ���˵�note�Ҳ���hold�򲻽����ж�
            
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







