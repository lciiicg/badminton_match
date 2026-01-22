using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ===== 玩家对象 =====
    public float PlayerMoveSpeed = 3f;
    public float SwingSpeed = 400f;
    public float MaxSwingAngle = 79f;

    public StickmanController LeftPlayer;
    public StickmanController RightPlayer;


    // ===== 场地尺寸 =====
    public float netHeight = 1.55f;
    public float courtLength = 13.4f;
    public float courtWidth = 6.1f;

    // ===== 比分 =====
    public Shuttlecock_Move Shuttlecock_move;
    public bool left_add_score;
    public bool right_add_score;
    public bool game_over;
    int left_score;
    int right_score;
    int top_score = 21;


    // ===== 击球状态 =====
    public bool left_hit;
    public bool right_hit;
    public bool if_left_even;
    public bool if_right_even;
    public bool isLeftSwinging;
    public bool isRightSwinging;

    //模拟飞行
    public float baseShuttlecockSpeed = 0.4f;
    public float hitDistanceThreshold = 0.5f; // 击球判定距离阈值
    public float vHorizontal = 4f;            // 水平速度 m/s
    public float g = 9.81f;                   // 重力加速度


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

        MaxSwingAngle = 79f;

        left_score = 0;
        right_score = 0;
        if_left_even = true;
        if_right_even = true;
        game_over = false;

        CreateScoreBoard();
    }

    void Update()
    {
        isLeftSwinging = LeftPlayer.isSwinging;
        isRightSwinging = RightPlayer.isSwinging;

        // 实时刷新比分
        left_add_score = Shuttlecock_move.left_add_score;
        right_add_score = Shuttlecock_move.right_add_score;

        if (Shuttlecock_move.isFlying)
        {
            left_hit = false;
            right_hit = false;
        }

        if (left_add_score)
        {
            left_score += 1;
            if_left_even = !if_left_even;
            left_hit = true;
            right_hit = false;
            left_add_score = false;
        }
        if (right_add_score)
        {
            right_score += 1;
            if_right_even = !if_right_even;
            right_hit = true;
            left_hit = false;
            right_add_score = false;
        }

        leftScoreText.text = left_score.ToString();
        rightScoreText.text = right_score.ToString();
        // 检查是否有玩家达到胜利分数
        if (!game_over && (left_score >= top_score || right_score >= top_score))
        {
            game_over = true;
            Debug.Log("Game Over!");
            if (left_score > right_score)
            {
                Debug.Log("Left Player Wins!");
            }
            else
            {
                Debug.Log("Right Player Wins!");
            }
        }
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
}
