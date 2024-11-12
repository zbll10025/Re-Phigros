using Cysharp.Threading.Tasks;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayControl : MonoBehaviour
{
    private static PlayControl _instance;

    private List<TouchData> touchDataList = new List<TouchData>();//触摸数据
    public Dictionary<NoteScaleParam,NoteScaleData>dicNoteScalParam = new Dictionary<NoteScaleParam,NoteScaleData>();
    public float timeOffset = 0;
    public float currentTime = 0;
    public Camera cam;
    public SimpleMusicPlayer musicPlay;
    public bool isPlayStart;
    public AudioSource effectBeat;
    public Chart chart;
    public static PlayControl Instance
    {
        get
        {
            if (_instance == null)
            {
                // 查找现有实例，如果没有，则创建一个新的 GameObject 并添加 PlayControl 组件
                _instance = FindObjectOfType<PlayControl>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PlayControl");
                    _instance = obj.AddComponent<PlayControl>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 144;
        //初始化note的图片属性
        InitializeNoteScaleDictionary();
        // 确保只有一个 PlayControl 实例存在
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // 在场景切换时保持该实例
    }

    void Start()
    {
        PoolManger.Instance.CreatPool("EffectBeat", "Prefabs/EffectBeat", 50);
        effectBeat = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentMusicTime();
        InputCheck();
        chart.JudeLineNotes(touchDataList, currentTime);
    }
    private void FixedUpdate()
    {
        //GamePause();
        if (isPlayStart)
        {
            isPlayStart = true;
            musicPlay.Play();
            chart.UpdataChart();
            //await UniTask.Delay(10);
        }
    }


    public void InputCheck()
    {
        touchDataList.Clear();
        Touch[] touches = Input.touches;
        //Debug.Log(touches.Count());
        foreach (Touch touch in touches)
        {

            TouchData touchData = new TouchData(TouchData.TouchType.Click, touch.position, Vector2.zero, false);

            Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cam.nearClipPlane));
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // 处理触摸开始事件
                    touchData.type = TouchData.TouchType.Click;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Moved:
                    // 处理触摸移动事件
                    touchData.type = TouchData.TouchType.Swipe;
                    Vector3 worldDele = cam.ScreenToWorldPoint(new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, cam.nearClipPlane));
                    touchData.lastPosition = worldPosition - worldDele;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Stationary:
                    // 处理触摸静止事件
                    touchData.type = TouchData.TouchType.LongPress;
                    touchData.position = worldPosition;
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
    }
    public void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPlayStart = true;
             musicPlay.Play();
               
        }
    }
    public float GetCurrentMusicTime()
    {
        float temp = Koreographer.Instance.GetMusicSampleTime();
        float Time = temp / 44100.0f; // 将样本时间转换为秒
        Time -= timeOffset;
        if (Time < 0)
        {
            Time = 0;
        }
        currentTime = Time;
        return Time;
    }
    void InitializeNoteScaleDictionary()
    {
        dicNoteScalParam[NoteScaleParam.Tap] = new NoteScaleData { width = 30f, height = 2.8f };
        dicNoteScalParam[NoteScaleParam.TapL] = new NoteScaleData { width = 33f, height = 5f };
        dicNoteScalParam[NoteScaleParam.Drag] = new NoteScaleData { width = 30f, height = 2f };
        dicNoteScalParam[NoteScaleParam.DragL] = new NoteScaleData { width = 30f, height = 4f };
        dicNoteScalParam[NoteScaleParam.Hold] = new NoteScaleData { width = 30f, height = 2.0f };
        dicNoteScalParam[NoteScaleParam.HoldL] = new NoteScaleData { width = 30f, height = 2.0f };
        dicNoteScalParam[NoteScaleParam.Flick] = new NoteScaleData { width = 32f, height = 5f };
        dicNoteScalParam[NoteScaleParam.FlickL] = new NoteScaleData { width = 34f, height = 8f };
    }

    public void PlayerEffectBeat(AudioClip clip)
    {
        effectBeat.PlayOneShot(clip);
    }
    public void onstart()
    {
        isPlayStart = true;

    }
}

public class NoteScaleData {

    public float width;
    public float height;
}

