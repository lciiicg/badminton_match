using System;
using UnityEngine;

public class BadmintonCourtCreator : MonoBehaviour
{
    // 场地尺寸
    float courtLength = 13.4f;
    float courtWidth = 6.1f;

    void Start()
    {
        CreateCourt();
        CreateLines();
        CreateNet();
    }

    // ================= 场地 =================
    void CreateCourt()
    {
        GameObject court = GameObject.CreatePrimitive(PrimitiveType.Cube);
        court.name = "Court";
        court.transform.parent = transform;
        court.transform.localScale = new Vector3(courtLength, 0.05f, courtWidth);
        court.transform.localPosition = Vector3.zero;

        court.GetComponent<Renderer>().material.color = new Color(0.2f, 0.6f, 0.3f);
    }

    // ================= 线条 =================
    void CreateLines()
    {
        float lineWidth = 0.05f;
        float y = 0.03f;

        // 双人边线（左右）
        CreateLine(new Vector3(0, y, courtWidth / 2), new Vector3(courtLength, lineWidth, lineWidth), "double_side_boundary_top");
        CreateLine(new Vector3(0, y, -courtWidth / 2), new Vector3(courtLength, lineWidth, lineWidth), "double_side_boundary_bottom");

        // 单人边线
        CreateLine(new Vector3(0, y, courtWidth / 2 - 0.46f), new Vector3(courtLength, lineWidth, lineWidth), "singal_side_boundary_top");
        CreateLine(new Vector3(0, y, -courtWidth / 2 + 0.46f), new Vector3(courtLength, lineWidth, lineWidth), "singal_side_boundary_bottom");


        // 底线（前后）
        CreateLine(new Vector3(-courtLength / 2, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "base_boundary_left");
        CreateLine(new Vector3(courtLength / 2, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "base_boundary_right");

        //前发球线
        CreateLine(new Vector3(-1.98f, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "front_service_line_left");
        CreateLine(new Vector3(1.98f, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "front_service_line_right");

        //后发球线
        CreateLine(new Vector3(-courtLength / 2 + 0.76f, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "back_service_line_left");
        CreateLine(new Vector3(courtLength / 2 - 0.76f, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "back_service_line_right");


        // 中线
        CreateLine(new Vector3(0, y, 0), new Vector3(lineWidth, lineWidth, courtWidth), "center_line_Z");
        CreateLine(new Vector3(0, y, 0), new Vector3(courtLength, lineWidth, lineWidth), "center_line_X");
    }

    void CreateLine(Vector3 pos, Vector3 scale, String name)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = name;
        line.transform.parent = transform;
        line.transform.localScale = scale;
        line.transform.localPosition = pos;
        line.GetComponent<Renderer>().material.color = Color.white;
    }

    // ================= 球网 =================
    void CreateNet()
    {
        float netHeight = 1.55f;

        // 网
        GameObject net = GameObject.CreatePrimitive(PrimitiveType.Cube);
        net.name = "Net";
        net.transform.parent = transform;
        net.transform.localScale = new Vector3(0.05f, netHeight, courtWidth);
        net.transform.localPosition = new Vector3(0, netHeight / 2, 0);
        net.GetComponent<Renderer>().material.color = Color.gray;

        // 左立柱
        CreatePost(new Vector3(0, netHeight / 2, courtWidth / 2));

        // 右立柱
        CreatePost(new Vector3(0, netHeight / 2, -courtWidth / 2));
    }

    void CreatePost(Vector3 pos)
    {
        GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        post.transform.parent = transform;
        post.transform.localScale = new Vector3(0.1f, 0.8f, 0.1f);
        post.transform.localPosition = pos;
        post.GetComponent<Renderer>().material.color = Color.black;
    }
}
