using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using SonicBloom.Koreo;
using System;
using Cysharp.Threading.Tasks;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;
using Unity.Burst;
using System.Threading;

public class Chart : MonoBehaviour
{
    public string jsonFilePath = "";
    
    public ChartData chartData;
    public List <Line> line_list;
    public List<Note> allNotes ;
    public float X = 17.75f;
    public float Y = 10f;
    public float noteX = 0;
    public float noteY = 0;
    public GameObject prefab;
    [Header("是否提醒多押")]
    public bool isRhyme = true;
    public Dictionary<float,int> rhymeTimeDic = new Dictionary<float,int>();
    #region job的参数
    //结果储存
    public NativeArray<float3> linesPosition;
    public NativeArray<float3> linesRotate;
    public NativeArray<float3> linesFade;
    public NativeArray<float3> notePosition;
    //line
    public NativeArray<DataLineMove> dataLinesMoves;
    public NativeArray<DataLineEvent> dataLinesRotate;
    public NativeArray <DataLineEvent> dataLinesFade;
    //note
    public NativeArray<DataNote>dataNotes;
    #endregion
    private void Awake()
    {
        chartData = ReadJson(jsonFilePath);
        X =177.8f;
        Y =100;
        noteX = 177.8f * 0.05625f;
        noteY = 100f * 0.6f;
    }
    private void Start()
    {
        IntChart();
        linesPosition=new NativeArray<float3>(line_list.Count, Allocator.Persistent);
        linesRotate=new NativeArray<float3>(line_list.Count,Allocator.Persistent);
        linesFade = new NativeArray<float3>(line_list.Count, Allocator.Persistent);
        notePosition=new NativeArray<float3>(allNotes.Count, Allocator.Persistent);
        //移动
        dataLinesMoves = new NativeArray<DataLineMove>(line_list.Count, Allocator.Persistent);
        //旋转
        dataLinesRotate=new NativeArray<DataLineEvent>(line_list.Count, Allocator.Persistent);
        //消失
        dataLinesFade = new NativeArray<DataLineEvent>(line_list.Count,Allocator.Persistent);
        dataNotes = new NativeArray<DataNote>(allNotes.Count, Allocator.Persistent);
        //Debug.Log(dataNotes.Count());
    }
    public   async void UpdataChart()
    {
        float curTime = PlayControl.Instance.currentTime;
        //更新判定线位置
        for(int i = 0; i < line_list.Count; i++)
        {
            bool isLineMoveChange = line_list[i].LineMoveEventUpdata(curTime);
            bool isLineRotateChange = line_list[i].LineRotateEventUpdata(curTime);
            bool isLineFadeChange = line_list[i].LineFadeEventUpdata(curTime);
            dataLinesMoves[i] = new DataLineMove 
            {   StartTime = (float)line_list[i].curLineMoveEvent.StartTime,
                EndTime = (float)line_list[i].curLineMoveEvent.EndTime,
                Start = (float)line_list[i].curLineMoveEvent.Start,
                Start2 = (float)line_list[i].curLineMoveEvent.Start2,
                End = (float)line_list[i].curLineMoveEvent.End,
                End2 = (float)line_list[i].curLineMoveEvent.End2,
                isChangePosition = isLineMoveChange,
                curPosition = line_list[i].transform.position,
                T = 1.875f/line_list[i].bpm
            };
            dataLinesFade[i] = new DataLineEvent
            {
                StartTime = (float)line_list[i].curLineDisEvent.StartTime,
                EndTime = (float)line_list[i].curLineDisEvent.EndTime,
                Start = (float)line_list[i].curLineDisEvent.Start,
                End = (float)line_list[i].curLineDisEvent.End,
                T = 1.875f / line_list[i].bpm,
                isChange = isLineFadeChange,
                curValue = new float3(0,0, line_list[i].lineSprite.color.a)
            };
            dataLinesRotate[i] = new DataLineEvent
            {
                StartTime = (float)line_list[i].curLineRotateEvent.StartTime,
                EndTime = (float)line_list[i].curLineRotateEvent.EndTime,
                Start = (float)line_list[i].curLineRotateEvent.Start,
                End = (float)line_list[i].curLineRotateEvent.End,
                T = 1.875f / line_list[i].bpm,
                isChange = isLineRotateChange,
                curValue = line_list[i].transform.eulerAngles

            };
        }
        for(int i = 0; i < allNotes.Count; i++)
        {
            if (allNotes[i].isbeat)
            {
                dataNotes[i] = new DataNote { isbeat = true };
                continue;

            }
            allNotes[i].NotDataeUpData(curTime);
            dataNotes[i] = new DataNote
            { 
                 StartTime = allNotes[i].curNoteData.StartTime,
                 EndTime = allNotes[i].curNoteData.EndTime,
                 isbeat = allNotes[i].isbeat,
                 baseValue = allNotes[i].curNoteData.Value,
                 value =allNotes[i].speed,
                 T = allNotes[i].T,
                 PreDistance = allNotes[i].curNoteData.PreDistance,
                 
            };

        }
        LineMoveJob lineMove = new LineMoveJob { 
            curTime = curTime,
            dataLinesMove = dataLinesMoves,
            Position = linesPosition,
            X = this.X,
            Y = this.Y,
        };
        LineEventJob lineRotate = new LineEventJob { 
            currentTime = curTime,
            dataLinesEvent = dataLinesRotate,
            result = linesRotate,

        };
        LineEventJob lineFade = new LineEventJob {
            
            currentTime= curTime,
            dataLinesEvent = dataLinesFade,
            result = linesFade,

        };
        NoteJob noteJob = new NoteJob {
            currentTime = curTime,
            dataNotes = this.dataNotes,
            result = this.notePosition,
            X = this.noteX,
            Y = this.noteY,
        };

        JobHandle sheduleLineMove = lineMove.ScheduleParallel(dataLinesMoves.Count(), 5, default);
        JobHandle sheduleLineRotate = lineRotate.ScheduleParallel(dataLinesRotate.Count(), 5, default);
        JobHandle sheduleLineFade = lineFade.ScheduleParallel(dataLinesFade.Count(), 5, default);
        JobHandle sheduleNote = noteJob.ScheduleParallel(dataNotes.Count(), 64, default);
        sheduleLineMove.Complete();
        sheduleLineFade.Complete();
        sheduleLineRotate.Complete();
        sheduleNote.Complete();
        for(int i = 0; i < line_list.Count; i++)
        {
            line_list[i].transform.position = linesPosition[i];
            Quaternion newRotation = Quaternion.Euler(linesRotate[i]);
            line_list[i].transform.rotation = newRotation;

            SpriteRenderer lineSprite = line_list[i].lineSprite;
            Color newColor = lineSprite.color;
            newColor.a = linesFade[i].z;
            lineSprite.color = newColor;
        }

        for(int i = 0; i < allNotes.Count; i++)
        {
            //allNotes[i].AutoJude(curTime,JudeType.prefect);//先自动判定一下是否以判定
            allNotes[i].LifeJUde(curTime);
            if (allNotes[i].isbeat)
            {
                allNotes[i].transform.localPosition = new Vector3(allNotes[i].positionX * noteX, 0, 0);
                continue;
            }
            if (allNotes[i].isAbove)
            {
                allNotes[i].transform.localPosition  = new Vector3(allNotes[i].positionX*noteX , allNotes[i].floorPosition - notePosition[i].y , 0);
                //Debug.Log(notePosition[i].y);
            }
            else
            {
                allNotes[i].transform.localPosition = new Vector3(allNotes[i].positionX * noteX, allNotes[i].floorPosition + notePosition[i].y, 0);
            }
        }
        await UniTask.Delay(10);
    }
    public ChartData ReadJson(string jsonPath)
    {
        string json = Resources.Load<TextAsset>(jsonPath).text;
        Debug.Log(json);
        ChartData chartData = ChartData.FromJson(json);
        return chartData;
    }//读取json文件转化为ChartData
    public void JudeLineNotes(List<TouchData>touchDataList,float curTime)
    {
        foreach (var line in line_list)
        {
            line.JudeLineNotes(touchDataList,curTime);
        }
    }
    public void IntChart()
    {
        int lineNumber = 0;
        foreach (JudgeLineList judedata in chartData.JudgeLineList)
        {
             Line line= GameObject.Instantiate(prefab).GetComponent<Line>();
           // Line line = PoolManger.Instance.Get("Line", "Prefabs/Line").GetComponent<Line>();
            line.X = X;
            line.Y = Y;
            line.noteX = noteX;
            line.noteY = noteY;
           
            line.number = lineNumber;
            lineNumber++;
            line.bpm = judedata.Bpm;
            line.T = 1.875f/line.bpm;
            line.speedEvents = judedata.SpeedEvents;
            line.notesAbove = judedata.NotesAbove;
            line.notesBelow = judedata.NotesBelow;
            line.rhymeTimeDic = rhymeTimeDic;
            line.allNotes = allNotes;
            line_list.Add(line);
            MoveEventToQueue(judedata.JudgeLineMoveEvents, line.moveEvents);
            LineEventToQueue(judedata.JudgeLineRotateEvents, line.rotateEvent);
            LineEventToQueue(judedata.JudgeLineDisappearEvents, line.disappearEvent);
            if (isRhyme)
            {
                line.RhymeDataToDic();
            }
        }

        foreach (Line line in line_list)
        {
            line.InstanceNotes(isRhyme);
            //SpeedEventToQueue(judedata.SpeedEvents, line.speedEventQueue);

            #region Dotween方案

            //Koreographer.Instance.RegisterForEvents("LineMove", line.LineMove);
            //Koreographer.Instance.RegisterForEvents("LineRotate", line.LineRotate);
            //Koreographer.Instance.RegisterForEvents("LineDisappear", line.LineDisappear);
            //Koreographer.Instance.RegisterForEvents("SpeedEvent", line.LineSpeed);
            //BindLineMove(judedata.JudgeLineMoveEvents, judedata.Bpm);
            //BindLineRotate(judedata.JudgeLineRotateEvents, judedata.Bpm);
            //BindLineDisappear(judedata.JudgeLineDisappearEvents, judedata.Bpm);
            //BindLineSpeed(judedata.SpeedEvents, judedata.Bpm);
            #endregion
        }

    }
    #region 将数据转化为队列
    public void LineEventToQueue(JudgeLineEvent[] data, Queue<JudgeLineEvent>result)
    {
        foreach (JudgeLineEvent item in data)
        {
            result.Enqueue(item);
        }
    }
    public void MoveEventToQueue(JudgeLineMoveEvent[] data, Queue<JudgeLineMoveEvent> result)
    {
        foreach (JudgeLineMoveEvent item in data)
        {
            result.Enqueue(item);
        }
    }

    public void SpeedEventToQueue(SpeedEvent[] data, Queue<SpeedEvent> result)
    {
        foreach (SpeedEvent item in data)
        {
            result.Enqueue(item);
        }
    }
    #endregion
    
    #region 绑定track废案
    //
    //public void BindLineMove(JudgeLineMoveEvent[] list,float bpm)
    //{
    //    foreach (var item in list)
    //    {
    //        KoreographyEvent trackEvent = CreatTrackEvent((float)item.StartTime,bpm);
    //        TrackEventData.Instance.AddLineMove(trackEvent);
    //    }

    //}

    //public void BindLineRotate(JudgeLineEvent[] list,float bpm)
    //{
    //    foreach (var item in list)
    //    {
    //        KoreographyEvent trackEvent = CreatTrackEvent((float)item.StartTime, bpm);
    //        TrackEventData.Instance.AddLineRotate(trackEvent);
    //    }
    //}

    //public void BindLineDisappear(JudgeLineEvent[] list,float bpm)
    //{
    //    foreach (var item in list)
    //    {
    //        KoreographyEvent trackEvent = CreatTrackEvent((float)item.StartTime, bpm);
    //        TrackEventData.Instance.AddLineDisappear(trackEvent);
    //    }
    //}
    //public void BindLineSpeed(SpeedEvent[]list,float bpm)
    //{
    //    foreach (var item in list)
    //    {
    //        KoreographyEvent trackEvent = CreatTrackEvent((float)item.StartTime, bpm);
    //        TrackEventData.Instance.AddSpeedEvent(trackEvent);
    //    }
    //}
    //public KoreographyEvent CreatTrackEvent(float time,float bpm)
    //{
    //    KoreographyEvent trackEvent = new KoreographyEvent();
    //    int sample = (int)((time * (1.875f / bpm)) * 44100);
    //    if (sample < 0) { sample = 0; }
    //    trackEvent.StartSample = sample;
    //    trackEvent.EndSample = sample;
    //    //Debug.Log(time + "kore:" + sample);
    //    return trackEvent;
    //}
    #endregion
}
[BurstCompile]
public struct LineMoveJob : IJobFor
{
    [ReadOnly]
    public float curTime;
    [ReadOnly]
    public NativeArray<DataLineMove> dataLinesMove;
    [ReadOnly]
    public float X;
    [ReadOnly]
    public float Y;
    public NativeArray<float3> Position;


    public void Execute(int i)
    {
        //int threadId = Thread.CurrentThread.ManagedThreadId;
        //Debug.Log($"Thread ID: {threadId} for entity {i}");
        float T = dataLinesMove[i].T;
        if (dataLinesMove[i].isChangePosition)
        {
            float t = (curTime - dataLinesMove[i].StartTime*T) / (dataLinesMove[i].EndTime*T - dataLinesMove[i].StartTime * T);
            float3 sPos = new float3(dataLinesMove[i].Start * X, dataLinesMove[i].Start2 * Y,0);
            float3 ePos = new float3(dataLinesMove[i].End*X,dataLinesMove[i].End2*Y,0);
            Position[i] = new float3(
                sPos.x + (ePos.x - sPos.x) * t,
                sPos.y + (ePos.y - sPos.y) * t,
                sPos.z + (ePos.z - sPos.z) * t 
            );
        }
        else
        {
            Position[i] = dataLinesMove[i].curPosition;
        }
    }

}
[BurstCompile]
public struct LineEventJob : IJobFor {

    [ReadOnly]
    public float currentTime;
    [ReadOnly]
    public NativeArray<DataLineEvent> dataLinesEvent;
    public NativeArray<float3> result;

    public void Execute(int i)
    {
        float T = dataLinesEvent[i].T;
        if (dataLinesEvent[i].isChange)
        {
            float t = (currentTime - dataLinesEvent[i].StartTime*T) / (dataLinesEvent[i].EndTime*T- dataLinesEvent[i].StartTime * T);
        float3 startRotate = new float3(0, 0, (float)dataLinesEvent[i].Start);
        float3 endRotate = new float3(0, 0, (float)dataLinesEvent[i].End);
        result[i] = new float3(
                startRotate.x + (endRotate.x - startRotate.x) * t,
                startRotate.y + (endRotate.y - startRotate.y) * t,
                startRotate.z + (endRotate.z - startRotate.z) * t
            );
        }
        else
        {
            result[i] = dataLinesEvent[i].curValue;
        }

    }

}
[BurstCompile]
public struct NoteJob : IJobFor 
{
    [ReadOnly]
    public float currentTime;
    [ReadOnly]
    public NativeArray<DataNote> dataNotes;
    [ReadOnly]
    public float X;
    [ReadOnly]
    public float Y;
    [WriteOnly]
    public NativeArray<float3> result;
    public void Execute(int i)
    {
        //Debug.Log($"Thread ID: {Thread.CurrentThread.ManagedThreadId} for entity {index}");
        if (dataNotes[i].isbeat)
        {
            //如果已经点击则不需要更新位置
            return;
        }
        float baseSpeed = dataNotes[i].baseValue;
        float speed = dataNotes[i].value;
        float startTime = dataNotes[i].StartTime * dataNotes[i].T;
        float endTime = dataNotes[i].EndTime * dataNotes[i].T;
        float hasMoveDistance = 0;
        
        if (currentTime >= startTime && currentTime < endTime)
        {
            float durTime = currentTime - startTime;
            hasMoveDistance = durTime * speed * baseSpeed * Y;
        }
       // Debug.Log(baseSpeed + "     " + speed + "     " + startTime/ dataNotes[i].T + "    " + currentTime + "        " + endTime/ dataNotes[i].T + "         " +hasMoveDistance+"     "+ dataNotes[i].PreDistance);
        hasMoveDistance += dataNotes[i].PreDistance;
        result[i] = new float3(0, hasMoveDistance, 0);

    }

}


public struct DataLineMove
{
    public float StartTime;
    public float EndTime;
    public float Start;
    public float End;
    public float Start2;
    public float End2;
    public bool isChangePosition;
    public float3 curPosition;
    public float T;
}

public struct DataLineEvent{
    public float StartTime;
    public float EndTime;
    public float Start;
    public float End;
    public float T;
    public bool isChange;
    public float3 curValue;
}

public struct DataNote{ 
    public float StartTime;
    public float EndTime;
    public float value;
    public float PreDistance;
    public bool isbeat;
    public float baseValue;
    public float T;
    public float floorPosition;
    public bool isAbove;
}
