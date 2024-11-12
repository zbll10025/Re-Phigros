/*
 * author: AnYuanLzh
 * date:   2014/01/18
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;


public class MyTouch : MonoBehaviour
{
    /// <summary>
    /// �����һ����ָ��
    /// </summary>
    public TextMeshProUGUI touchCount;
    class MyFinger
    {
        public int id = -1;
        public Touch touch;

        static private List<MyFinger> fingers = new List<MyFinger>();
        /// <summary>
        /// ��ָ����
        /// </summary>
        static public List<MyFinger> Fingers
        {
            get
            {
                if (fingers.Count == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MyFinger mf = new MyFinger();
                        mf.id = -1;
                        fingers.Add(mf);
                    }
                }
                return fingers;
            }
        }

    }

    // СȦȦ������ʵʱ��ʾ��ָ������λ��
    GameObject[] marks = new GameObject[5];
    public GameObject markPerfab = null;

    // ����Ч����������ʾ��ָ�ֶ��Ĵ��·��
    ParticleSystem[] particles = new ParticleSystem[5];
    public ParticleSystem particlePerfab = null;


    // Use this for initialization
    void Start()
    {
        // init marks and particles
        for (int i = 0; i < MyFinger.Fingers.Count; i++)
        {
            GameObject mark = Instantiate(markPerfab, Vector3.zero, Quaternion.identity) as GameObject;
            mark.transform.parent = this.transform;
            mark.SetActive(false);
            marks[i] = mark;

            ParticleSystem particle = Instantiate(particlePerfab, Vector3.zero, Quaternion.identity) as ParticleSystem;
            particle.transform.parent = this.transform;
            particle.Pause();
            particles[i] = particle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Touch[] touches = Input.touches;
        touchCount.text = "AllCount:" + (touches.Count()).ToString();
        // �������е��Ѿ���¼����ָ
        // --����Ѿ������ڵ���ָ
        foreach (MyFinger mf in MyFinger.Fingers)
        {
            if (mf.id == -1)
            {
                continue;
            }
            bool stillExit = false;
            foreach (Touch t in touches)
            {
                if (mf.id == t.fingerId)
                {
                    stillExit = true;
                    break;
                }
            }
            // ���
            if (stillExit == false)
            {
                mf.id = -1;
            }
        }
        // ������ǰ��touches
        // --������������Ƿ��Ѿ���¼��AllFinger��
        // --�ǵĻ����¶�Ӧ��ָ��״̬�����ǵķŷżӽ�ȥ
        foreach (Touch t in touches)
        {
            bool stillExit = false;
            // ����--���¶�Ӧ����ָ
            foreach (MyFinger mf in MyFinger.Fingers)
            {
                if (t.fingerId == mf.id)
                {
                    stillExit = true;
                    mf.touch = t;
                    break;
                }
            }
            // ������--����¼�¼
            if (!stillExit)
            {
                foreach (MyFinger mf in MyFinger.Fingers)
                {
                    if (mf.id == -1)
                    {
                        mf.id = t.fingerId;
                        mf.touch = t;
                        break;
                    }
                }
            }
        }

        // ��¼����ָ��Ϣ�󣬾�����Ӧ��Ӧ��״̬��¼��
        for (int i = 0; i < MyFinger.Fingers.Count; i++)
        {
            MyFinger mf = MyFinger.Fingers[i];
            if (mf.id != -1)
            {
                if (mf.touch.phase == TouchPhase.Began)
                {
                    marks[i].SetActive(true);
                    marks[i].transform.position = GetWorldPos(mf.touch.position);

                    particles[i].transform.position = GetWorldPos(mf.touch.position);
                }
                else if (mf.touch.phase == TouchPhase.Moved)
                {
                    marks[i].transform.position = GetWorldPos(mf.touch.position);

                    if (!particles[i].isPlaying)
                    {
                        particles[i].Play();
                    }
                    particles[i].transform.position = GetWorldPos(mf.touch.position);
                }
                else if (mf.touch.phase == TouchPhase.Ended)
                {
                    marks[i].SetActive(false);
                    marks[i].transform.position = GetWorldPos(mf.touch.position);
                    particles[i].Play();
                    particles[i].transform.position = GetWorldPos(mf.touch.position);
                }
                else if (mf.touch.phase == TouchPhase.Stationary)
                {
                    if (particles[i].isPlaying)
                    {
                        particles[i].Pause();
                    }
                    particles[i].transform.position = GetWorldPos(mf.touch.position);
                }
            }
            else
            {
                ;
            }
        }

        // exit
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //		// test
        //		if(Input.GetMouseButtonDown(0))
        //		{
        //			GameObject mark = Instantiate(markPerfab, Vector3.zero, Quaternion.identity) as GameObject;
        //			mark.transform.parent = this.transform;
        //			mark.transform.position = GetWorldPos(Input.mousePosition);
        //
        //			ParticleSystem particle = Instantiate(particlePerfab, Vector3.zero, Quaternion.identity) as ParticleSystem;
        //			particle.transform.parent = this.transform;
        //			particle.transform.position = GetWorldPos(Input.mousePosition);
        //			particle.loop = false;
        //			particle.Play();
        //		}
    }

    /// <summary>
    /// ��ʾ��ظ߶�����
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("֧�ֵ���ָ��������" + MyFinger.Fingers.Count);
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        for (int i = 0; i < MyFinger.Fingers.Count; i++)
        {
            GUILayout.BeginVertical();
            MyFinger mf = MyFinger.Fingers[i];
            GUILayout.Label("��ָ" + i.ToString());
            if (mf.id != -1)
            {
                GUILayout.Label("Id�� " + mf.id);
                GUILayout.Label("״̬�� " + mf.touch.phase.ToString());
            }
            else
            {
                GUILayout.Label("û�з��֣�");
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public Vector3 GetWorldPos(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10));
    }


}