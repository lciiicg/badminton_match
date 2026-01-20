using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public StickmanController LeftPlayer;
    public StickmanController RightPlayer;

    public int left_score;
    public int right_score;
    public int top_score = 21;

    public bool left_hit;
    public bool right_hit;
    public bool even;

    // ===== UI =====
    TextMeshProUGUI leftScoreText;
    TextMeshProUGUI rightScoreText;
    TextMeshProUGUI colonText;

    void Awake()
    {
        left_hit = true;
        right_hit = false;
        left_score = 0;
        right_score = 0;
        even = true;

        CreateScoreBoard();
    }

    void Update()
    {
        // 实时刷新比分
        leftScoreText.text = left_score.ToString();
        rightScoreText.text = right_score.ToString();
    }

    // =====================================================
    // 创建“比赛转播风格”计分板
    // =====================================================
    void CreateScoreBoard()
    {
        // ---------- Canvas ----------
        GameObject canvasGO = new GameObject("ScoreBoardCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
            UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // ---------- 父容器 ----------
        GameObject panelGO = new GameObject("ScorePanel");
        panelGO.transform.SetParent(canvasGO.transform);

        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -40);
        panelRect.sizeDelta = new Vector2(600, 100);

        // ---------- 左分 ----------
        leftScoreText = CreateText(
            "LeftScore",
            panelGO.transform,
            new Vector2(-120, 0),
            72,
            TextAlignmentOptions.Right
        );

        // ---------- 冒号 ----------
        colonText = CreateText(
            "Colon",
            panelGO.transform,
            Vector2.zero,
            72,
            TextAlignmentOptions.Center
        );
        colonText.text = ":";

        // ---------- 右分 ----------
        rightScoreText = CreateText(
            "RightScore",
            panelGO.transform,
            new Vector2(120, 0),
            72,
            TextAlignmentOptions.Left
        );
    }

    TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        Vector2 pos,
        int fontSize,
        TextAlignmentOptions alignment
    )
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 100);
        rect.anchoredPosition = pos;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.text = "0";

        return tmp;
    }

    // ================== 对外加分接口 ==================
    public void LeftPlayerScore()
    {
        left_score++;
        even = (left_score + right_score) % 2 == 0;
    }

    public void RightPlayerScore()
    {
        right_score++;
        even = (left_score + right_score) % 2 == 0;
    }
}
