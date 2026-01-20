using UnityEngine;

public class StickmanBuilder : MonoBehaviour
{
    public enum Side { Left, Right }
    public Side side = Side.Right;

    [Header("Body Size (meters)")]
    const float totalHeight = 1.8f;
    const float headHeight = 0.225f;
    const float torsoHeight = 0.675f;

    [Header("Spawn Offset")]
    public Vector3 spawnLocalOffset = Vector3.zero;

    Transform torso;
    Transform head;
    public Transform racketRoot;

    void Awake()
    {
        Build();
    }

    public void Build()
    {
        float dir = (side == Side.Right) ? 1f : -1f;

        // 清理旧模型
        foreach (Transform c in transform) Destroy(c.gameObject);

        // ===== Torso =====
        torso = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        torso.name = "Torso";
        torso.parent = transform;
        torso.localScale = new Vector3(0.18f, torsoHeight, 0.18f);
        torso.localPosition = spawnLocalOffset;
        torso.GetComponent<Renderer>().material.color = Color.black;

        // ===== Head =====
        head = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        head.name = "Head";
        head.parent = transform;
        head.localScale = Vector3.one * headHeight;
        head.localPosition = spawnLocalOffset + new Vector3(0, torsoHeight + headHeight / 2f, 0);
        head.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.6f);

        // ===== Racket Root =====
        racketRoot = new GameObject("RacketRoot").transform;
        racketRoot.parent = transform;
        racketRoot.localPosition = spawnLocalOffset + new Vector3(-(0.1f * dir), torsoHeight * 0.8f, 0);
        racketRoot.localRotation = Quaternion.Euler(20f * dir, -20f, 0);

        // ===== Handle =====
        Transform handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        handle.name = "Handle";
        handle.parent = racketRoot;
        handle.localScale = new Vector3(0.03f, 0.1f, 0.03f);
        handle.localPosition = new Vector3(0, 0.1f, 0);

        // ===== Frame =====
        Transform frame = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        frame.name = "Frame";
        frame.parent = racketRoot;
        frame.localScale = new Vector3(0.01f, 0.13f, 0.25f);
        frame.localRotation = Quaternion.Euler(90, 0, 0);
        frame.localPosition = new Vector3(0, 0.33f, 0);
    }
}
