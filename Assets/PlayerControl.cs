using UnityEngine;

public enum PlayerSide { Left, Right }

public class StickmanController : MonoBehaviour
{
    public GameManager gameSystem;
    public bool left_hit;
    public bool right_hit;
    bool if_left_even;
    bool if_right_even;
    public PlayerSide side;
    public float moveSpeed;
    public float swingSpeed;
    public float maxSwingAngle;

    StickmanBuilder builder;
    Transform racket;

    public bool isSwinging;
    float swingTimer;                // 挥拍剩余时间
    public float swingActiveTime = 1f;   // 挥拍有效时长（秒）
    float swing;

    // 右玩家击球键
    KeyCode[] rightSwingKeys = { KeyCode.O};
    // 左玩家击球键
    KeyCode[] leftSwingKeys = { KeyCode.Q};

    KeyCode[] swingKeys;
    KeyCode lastKey = KeyCode.None;
    bool keyPressed;
    float keyDownTime;
    float lastKeyUpTime;

    public Vector3 PlayerPosition => transform.position;

    float courtLength;
    float courtWidth;

    bool game_over;

    // 并在 Start() 方法中添加初始化：
    void Start()
    {
        builder = GetComponent<StickmanBuilder>();
        racket = builder.racketRoot;

        // 根据左右球员选择击球键
        swingKeys = (side == PlayerSide.Right) ? rightSwingKeys : leftSwingKeys;

        moveSpeed = gameSystem.PlayerMoveSpeed;
        swingSpeed = gameSystem.SwingSpeed;
        maxSwingAngle = gameSystem.MaxSwingAngle;
        Debug.Log($"maxSwingAngle: {maxSwingAngle}");

        courtLength = gameSystem.courtLength;
        courtWidth = gameSystem.courtWidth;

        if (gameSystem == null) gameSystem = FindObjectOfType<GameManager>();
        if (gameSystem != null)
        {
            left_hit = gameSystem.left_hit;
            right_hit = gameSystem.right_hit;
            if_left_even = gameSystem.if_left_even;
            if_right_even = gameSystem.if_right_even;
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }

    void Update()
    {
        if (!game_over)
        {
            HandleMovement();
            HandleSwing();
        }
    }

    void HandleMovement()
    {
        float h = 0, v = 0;

        if (side == PlayerSide.Right)
        {
            if (Input.GetKey(KeyCode.J)) h = -1;
            if (Input.GetKey(KeyCode.L)) h = 1;
            if (Input.GetKey(KeyCode.I)) v = 1;
            if (Input.GetKey(KeyCode.K)) v = -1;
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) h = -1;
            if (Input.GetKey(KeyCode.D)) h = 1;
            if (Input.GetKey(KeyCode.W)) v = 1;
            if (Input.GetKey(KeyCode.S)) v = -1;
        }

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.position += move;

        float minX = 0f, maxX = 0f, minZ = -2.6f, maxZ = 2.6f;

        if (gameSystem != null)
        {
            left_hit = gameSystem.left_hit;
            right_hit = gameSystem.right_hit;
            if_left_even = gameSystem.if_left_even;
            if_right_even = gameSystem.if_right_even;
            //Debug.Log($"left_hit: {left_hit}");
            //Debug.Log($"right_hit: {right_hit}");
        }
        if (!left_hit && !right_hit)
        {
            Debug.Log($"1:left_hit: {left_hit}, right_hit: {right_hit}");
            minX = side == PlayerSide.Left ? -6.7f : 0f;
            maxX = side == PlayerSide.Left ? 0f : 6.7f;
            minZ = - (courtWidth / 2 - 0.46f);
            maxZ = (courtWidth / 2 - 0.46f);
        }
        if (left_hit && !right_hit)
        {

            Debug.Log($"2:left_hit: {left_hit}, right_hit: {right_hit}");
            // 左球员发球
            minX = side == PlayerSide.Left ? -(courtLength / 2 - 0.76f) : 0f;
            maxX = side == PlayerSide.Left ? -1.98f : 6.7f;
            // 左双右单
            if (if_left_even)
            {
                maxZ = side == PlayerSide.Left ? (courtWidth / 2 - 0.46f) : 0;
                minZ = side == PlayerSide.Left ? 0 : -(courtWidth / 2 - 0.46f);
            }
            else 
            {
                maxZ = side == PlayerSide.Left ? 0 : (courtWidth / 2 - 0.46f);
                minZ = side == PlayerSide.Left ? -(courtWidth / 2 - 0.46f) : 0;
            }
        }

        if (!left_hit && right_hit)
        {
            Debug.Log($"3:left_hit: {left_hit}, right_hit: {right_hit}");
            // 右球员发球
            minX = side == PlayerSide.Left ? -(courtLength / 2) : 1.98f;
            maxX = side == PlayerSide.Left ? 0 : (courtLength / 2 - 0.76f);
            // 左双右单
            if (if_right_even)
            {
                maxZ = side == PlayerSide.Left ? (courtWidth / 2 - 0.46f) : 0;
                minZ = side == PlayerSide.Left ? 0 : -(courtWidth / 2 - 0.46f);
            }
            else
            {
                maxZ = side == PlayerSide.Left ? 0 : (courtWidth / 2 - 0.46f);
                minZ = side == PlayerSide.Left ? -(courtWidth / 2 - 0.46f) : 0;
            }
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minZ, maxZ)
        );
    }
    void StartSwing()
    {
        // 如果已经在挥拍有效期内，不重复触发
        if (isSwinging) return;

        isSwinging = true;
        swingTimer = swingActiveTime;   // 重置挥拍有效时间
    }


    void HandleSwing()
    {

        if (!isSwinging)
        {
            // === 监听击球键（左 Q / 右 O）===
            foreach (KeyCode key in swingKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    StartSwing();
                    break;
                }
            }
        }

        // === 挥拍有效时间倒计时 ===
        if (isSwinging)
        {
            swingTimer -= Time.deltaTime;
            if (swingTimer <= 0f)
            {
                isSwinging = false;
                swingTimer = 0f;
                swing = 0f;   // 顺手重置，防止下次残留
            }
        }

            // === 挥拍动画（和有效期同步）===
        if (isSwinging)
        {
            swing += swingSpeed * Time.deltaTime;
            Debug.Log($"swing: {swing}, maxSwingAngle: {maxSwingAngle}");
            float dir = (side == PlayerSide.Right) ? 1f : -1f;
            racket.localRotation = Quaternion.Euler(0, 0, -20 + swing * dir);
                

            if (swing >= maxSwingAngle)
            {
                swing = 0f;
                racket.localRotation = Quaternion.Euler(20 * dir, 0, 20 * dir);
            }
        }
    }

}
