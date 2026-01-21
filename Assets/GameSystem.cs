using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ===== 玩家对象 =====
    public float PlayerMoveSpeed = 3f;
    public float SwingSpeed = 400f;
    public float MaxSwingAngle = 120f;
    public float longPressTime = 0.4f;
    public float doubleClickInterval = 0.25f;

    public StickmanController LeftPlayer;
    public StickmanController RightPlayer;
    public Vector3 leftPlayerPos;
    public Vector3 rightPlayerPos;
    public SwingType leftPlayerSwingType;
    public SwingType rightPlayerSwingType;
    public ShotDirection leftShotDirection;
    public ShotDirection rightShotDirection;

    // ===== 场地尺寸 =====
    public float netHeight = 1.55f;
    public float courtLength = 13.4f;
    public float courtWidth = 6.1f;

    // ===== 比分 =====
    public int left_score;
    public int right_score;
    public int top_score = 21;

    // ===== 击球状态 =====
    public bool left_hit;
    public bool right_hit;
    public bool if_even;

    //球速
    public float baseShuttlecockSpeed = 1f;
    public float dropSpeedRate = 0.98f;  // 吊球减速倍率
    public float clearSpeedRate = 1.02f; // 高远球加速倍率
    public float smashSpeedRate = 2.0f;  // 扣杀球加速倍率



    // ===== UI =====
    TextMeshProUGUI leftScoreText;
    TextMeshProUGUI rightScoreText;
    TextMeshProUGUI colonText;

    void Awake()
    {
        if (Random.value > 0.5f)
        {
            right_hit = true;
            left_hit = false;
        }
        else
        {
            left_hit = true;
            right_hit = false;
        }

        left_hit = false;
        right_hit = false;
        left_score = 0;
        right_score = 0;
        if_even = true;

        CreateScoreBoard();
    }

    void Update()
    {
        // 实时刷新比分
        leftScoreText.text = left_score.ToString();
        rightScoreText.text = right_score.ToString();
        leftPlayerPos = LeftPlayer.PlayerPosition;
        rightPlayerPos = RightPlayer.PlayerPosition;
        leftPlayerSwingType = LeftPlayer.CurrentSwingType;
        rightPlayerSwingType = RightPlayer.CurrentSwingType;
        leftShotDirection = LeftPlayer.CurrentDirection;
        rightShotDirection = RightPlayer.CurrentDirection;
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
        if_even = (left_score + right_score) % 2 == 0;
    }

    public void RightPlayerScore()
    {
        right_score++;
        if_even = (left_score + right_score) % 2 == 0;
    }
}
