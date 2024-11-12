using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BasePanel : MonoBehaviour
{
    
    public bool isOpen = false;
    public string _name;

    #region 
    public virtual void OpenPanel()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }
    public virtual void ClosePanel()
    {
        isOpen = false;
        gameObject.SetActive(false);
        Clear();
    }

    #endregion 
    public virtual void PanelInt()
    {
        //��ʼ��
    }

    public virtual void Clear()
    {

    }
}
