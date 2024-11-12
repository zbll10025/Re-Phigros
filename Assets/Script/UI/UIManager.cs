using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private static UIManager _instance;
    private Transform uiRoot;
    public Transform UIRoot
    {
        get
        {
            if (uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    uiRoot = new GameObject("Canvas").transform;
                }
            }
            return uiRoot;
        }
    }
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // ��������ʵ�������û�У��򴴽�һ���µ� GameObject ����� PlayControl ���
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    _instance = obj.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    public Dictionary<string, BasePanel> panel_Dict = new Dictionary<string, BasePanel>();
    public BasePanel OpenPanel(string name,string path)
    {
        if (panel_Dict.ContainsKey(name))
        {
            if (!panel_Dict[name].isOpen)
            {
                panel_Dict[name].OpenPanel();
            }
            else
            {
                Debug.Log(name + "UI���ڴ�״̬");
            }
            return panel_Dict[name];
        }
        else
        {
            return CreatPanel(name, path);
        }
    }
    public void ClosePanel(string name)
    {
        if (panel_Dict.ContainsKey(name))
        {
            panel_Dict[name].ClosePanel();
        }
        else
        {
            Debug.Log("�ر�ʱUIManger�Ҳ���" + name + ",�Ƿ�ɹ�����");
        }
    }
    public void DestoryPanel(string name)
    {
        if (panel_Dict.ContainsKey(name))
        {
            panel_Dict[name].ClosePanel();
            PoolManger.Instance.Recycle(name, panel_Dict[name].gameObject);
        }
        else {

            Debug.Log("����ʱUIManger�Ҳ���" + name + ",�Ƿ�ɹ�����");
        }
    }
    BasePanel CreatPanel(string name,string path)
    {
       var obj =  PoolManger.Instance.Get(name, path);
        obj.transform.parent = UIRoot;
        obj.SetActive(false);
       return  obj.GetComponent<BasePanel>();
    }
}
