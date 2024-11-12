using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MashPolygonUI : Graphic
{

    private void OnGUI()
    {
        // 实时检测更新绘制 OnPopulateMesh 中 transform.child 位置
        SetAllDirty();
    }

    /// <summary>
    /// 根据 transform.child 位置 绘制 Mesh
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

        // 几何图形的顶点，本例中根据子节点坐标确定顶点
        foreach (Transform child in transform)
        {
            vh.AddVert(child.localPosition, color32, new Vector2(0f, 0f));
        }

        for (int i = 0; i < (transform.childCount - 2); i++)
        {
            // 几何图形中的三角形
            vh.AddTriangle(i + 1, i + 2, 0);

        }

    }

    /// <summary>
    /// 点的辅助绘制
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