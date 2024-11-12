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

    private List<TouchData> touchDataList = new List<TouchData>();//��������
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
                // ��������ʵ�������û�У��򴴽�һ���µ� GameObject ����� PlayControl ���
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
        //��ʼ��note��ͼƬ����
        InitializeNoteScaleDictionary();
        // ȷ��ֻ��һ�� PlayControl ʵ������
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // �ڳ����л�ʱ���ָ�ʵ��
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
                    // ��������ʼ�¼�
                    touchData.type = TouchData.TouchType.Click;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Moved:
                    // �������ƶ��¼�
                    touchData.type = TouchData.TouchType.Swipe;
                    Vector3 worldDele = cam.ScreenToWorldPoint(new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, cam.nearClipPlane));
                    touchData.lastPosition = worldPosition - worldDele;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Stationary:
                    // ��������ֹ�¼�
                    touchData.type = TouchData.TouchType.LongPress;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Ended:
                    // �����������¼�
                    touchData.type = TouchData.TouchType.Click;
                    touchData.position = worldPosition;
                    break;
                case TouchPhase.Canceled:
                    // ������ȡ���¼�
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
        float Time = temp / 44100.0f; // ������ʱ��ת��Ϊ��
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

