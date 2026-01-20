using UnityEngine;

public enum PlayerSide { Left, Right }
public enum SwingType { Drop, Clear, Smash }
public enum ShotDirection { Left, Center, Right }

public class StickmanController : MonoBehaviour
{
    public GameManager gameSystem;
    public bool left_hit;
    public bool right_hit;
    public bool if_even;
    public PlayerSide side;
    public float moveSpeed = 3f;
    public float swingSpeed = 400f;
    public float maxSwingAngle = 120f;

    StickmanBuilder builder;
    Transform racket;

    bool isSwinging;
    float swing;

    public float longPressTime = 0.4f;
    public float doubleClickInterval = 0.25f;
    // 右玩家击球键
    KeyCode[] rightSwingKeys = { KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };
    // 左玩家击球键（可改）
    KeyCode[] leftSwingKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

    KeyCode[] swingKeys;
    KeyCode lastKey = KeyCode.None;
    bool keyPressed;
    float keyDownTime;
    float lastKeyUpTime;

    public Vector3 PlayerPosition => transform.position;
    public SwingType CurrentSwingType { get; private set; }
    public ShotDirection CurrentDirection { get; private set; }

    float courtLength = 13.4f;
    float courtWidth = 6.1f;

    // 并在 Start() 方法中添加初始化：
    void Start()
    {
        builder = GetComponent<StickmanBuilder>();
        racket = builder.racketRoot;
        swingKeys = (side == PlayerSide.Right) ? rightSwingKeys : leftSwingKeys;
        if (gameSystem == null) gameSystem = FindObjectOfType<GameManager>();
        if (gameSystem != null)
        {
            left_hit = gameSystem.left_hit;
            right_hit = gameSystem.right_hit;
            Debug.Log($"0:left_hit: {left_hit}, right_hit: {right_hit}");
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleSwing();
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
            if_even = gameSystem.even;
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
            if (if_even)
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
            if (if_even)
            {
                maxZ = 0;
                minZ = -(courtWidth / 2 - 0.46f);
            }
            else
            {
                maxZ = (courtWidth / 2 - 0.46f);
                minZ = 0;
            }
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minZ, maxZ)
        );
    }

    ShotDirection GetDirectionByKey(KeyCode key)
    {
        if (side == PlayerSide.Right)
        {
            if (key == KeyCode.Alpha8) return ShotDirection.Left;
            if (key == KeyCode.Alpha9) return ShotDirection.Center;
            return ShotDirection.Right;
        }
        else
        {
            if (key == KeyCode.LeftArrow) return ShotDirection.Left;
            if (key == KeyCode.Space) return ShotDirection.Center;
            return ShotDirection.Right;
        }
    }
    void ResetKeyState()
    {
        keyPressed = false;
        lastKey = KeyCode.None;
    }

    void StartSwing(SwingType type, ShotDirection dir)
    {
        if (isSwinging) return;

        isSwinging = true;
        CurrentSwingType = type;
        CurrentDirection = dir;
    }

    void FireShot(SwingType type, ShotDirection dir)
    {
        StartSwing(type, dir);
    }
    void HandleSwing()
    {
        foreach (KeyCode key in swingKeys)
        {
            if (Input.GetKeyDown(key))
            {
                keyPressed = true;
                keyDownTime = Time.time;
                lastKey = key;
            }

            if (keyPressed && Input.GetKey(lastKey))
            {
                if (Time.time - keyDownTime >= longPressTime)
                {
                    FireShot(SwingType.Smash, GetDirectionByKey(lastKey));
                    ResetKeyState();
                }
            }

            if (keyPressed && Input.GetKeyUp(lastKey))
            {
                float t = Time.time - keyDownTime;

                if (t < doubleClickInterval && Time.time - lastKeyUpTime <= doubleClickInterval)
                    FireShot(SwingType.Clear, GetDirectionByKey(lastKey));
                else if (t < longPressTime)
                    FireShot(SwingType.Drop, GetDirectionByKey(lastKey));

                lastKeyUpTime = Time.time;
                ResetKeyState();
            }
        }

        // ===== 挥拍动画 =====
        if (isSwinging)
        {
            swing += swingSpeed * Time.deltaTime;

            float dir = (side == PlayerSide.Right) ? 1f : -1f; // 右手+1，左手-1
            racket.localRotation = Quaternion.Euler(0, 0, -20 + swing * dir);

            if (swing >= maxSwingAngle)
            {
                isSwinging = false;
                swing = 0;
                racket.localRotation = Quaternion.Euler(20*dir, 0, 20 * dir);
            }
        }

    }
}
