using UnityEngine;

public class ShuttlecockEntity : MonoBehaviour
{
    [Header("羽毛球参数")]
    public float cylinderRadius = 0.01f;   // 球体/底座半径
    public float cylinderHeight = 0.015f;   // 球体/底座高度
    public float coneRadius = 0.036f;       // 羽毛圆锥底半径
    public float coneHeight = 0.09f;       // 羽毛圆锥高度

    void Awake()
    {
        BuildShuttlecock();
    }
    
    void BuildShuttlecock()
    {
        // ===== 底部圆柱 =====
        GameObject baseCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseCylinder.name = "BaseCylinder";
        baseCylinder.transform.parent = transform;
        baseCylinder.transform.localScale = new Vector3(cylinderRadius * 2f, cylinderHeight, cylinderRadius * 2f);
        baseCylinder.transform.localPosition = new Vector3(0, cylinderHeight / 2f, 0);
        baseCylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
        baseCylinder.GetComponent<Renderer>().material.color = Color.white;

        // ===== 上部羽毛圆锥 =====
        GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cone.name = "FeathersCone";
        cone.transform.parent = transform;
        // 使用 Cylinder 模拟圆锥：底面大，上面小
        cone.transform.localScale = new Vector3(coneRadius * 2f, coneHeight / 2f, coneRadius * 2f);
        cone.transform.localPosition = new Vector3(0, cylinderHeight + coneHeight / 2f, 0);
        cone.transform.localRotation = Quaternion.Euler(0, 0, 180);
        cone.GetComponent<Renderer>().material.color = Color.white;

        // 将顶部缩放成圆锥（上面小，下面大）
        MeshFilter mf = cone.GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // 顶部顶点 y 最大的点缩小成尖端
            if (vertices[i].y > 0)
            {
                vertices[i].x = 0;
                vertices[i].z = 0;
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
