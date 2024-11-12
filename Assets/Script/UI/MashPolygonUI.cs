using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MashPolygonUI : Graphic
{

    private void OnGUI()
    {
        // ʵʱ�����»��� OnPopulateMesh �� transform.child λ��
        SetAllDirty();
    }

    /// <summary>
    /// ���� transform.child λ�� ���� Mesh
    /// </summary>
    /// <param name="vh"></param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (transform.childCount <= 2)
        {
            return;
        }

        Color32 color32 = color;
        vh.Clear();

        // ����ͼ�εĶ��㣬�����и����ӽڵ�����ȷ������
        foreach (Transform child in transform)
        {
            vh.AddVert(child.localPosition, color32, new Vector2(0f, 0f));
        }

        for (int i = 0; i < (transform.childCount - 2); i++)
        {
            // ����ͼ���е�������
            vh.AddTriangle(i + 1, i + 2, 0);

        }

    }

    /// <summary>
    /// ��ĸ�������
    /// </summary>
    private void OnDrawGizmos()
    {
        if (transform.childCount == 0)
        {
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, 2.55f);
            Gizmos.DrawIcon(transform.GetChild(i).position, "numbers/numbers_" + i + ".PNG", false);
        }

    }
}