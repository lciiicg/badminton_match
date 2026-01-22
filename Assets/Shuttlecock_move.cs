using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Shuttlecock_Move : MonoBehaviour
{
    ShuttlecockEntity shuttlecock;
    public GameManager gameManager;
    public StickmanController LeftPlayer;
    public StickmanController RightPlayer;
    float courtLength;
    float courtWidth;
    float netHeight;


    // 离散坐标数组
    public float[] xLeft;
    public float[] xRight;
    public float[] zPoints;

    // 羽毛球发球位置
    float relativeHeight = 1.15f;
    float relativeDistance = 0.35f;
    bool left_hit;
    bool right_hit;
    bool isLeftSwinging;
    bool isRightSwinging;
    bool if_left_even;
    bool if_right_even;

    // 玩家位置与挥拍
    Vector3 leftPlayerPos;
    Vector3 rightPlayerPos;
    Vector3 shuttlecockPos;
    Vector3 target_position;

    //上次击球方
    int next_player;
    bool isFlying = false;
    public bool left_add_score = false;
    public bool right_add_score = false;
    bool game_over;


    //模拟飞行参数
    float baseShuttlecockSpeed;
    float hitDistanceThreshold; // 击球判定距离阈值
    float vHorizontal;            // 水平速度 m/s
    float g;                   // 重力加速度

    /// <summary>
    /// 处理羽毛球击打逻辑
    /// </summary>
    /// <param name="receiverPos">接球方球员位置</param>
    /// <param name="swing">是否处于挥拍有效期</param>
    /// <param name="shuttlePos">当前羽毛球位置</param>
    /// <param name="opponentPos">对面球员位置（用于重新计算落点）</param>
    /// <returns>返回true表示球已经落地，false表示球仍在空中</returns>
    public bool HandleShuttlecockHit(Vector3 receiverPos, bool swing, Vector3 shuttlePos, Vector3 opponentPos)
    {
        // 1. 球已落地
        if (shuttlePos.y <= 0f)
        {
            // 判断落点在左场还是右场
            bool landedRight = shuttlePos.x > 0f;
            if (landedRight)
            { 
                right_add_score = false;
                left_add_score = true;
                left_hit = true;
                Debug.Log("球已落地, 在右场: " + landedRight); 
            }
            if (!landedRight)
            { 
                left_add_score = false;
                right_add_score = true;
                right_hit = true;
                Debug.Log("球已落地, 在左场: " + landedRight);
            }
        }

        // 2. 计算接球方与球的水平距离
        Vector3 horizontalDisplacement = new Vector3(shuttlePos.x - receiverPos.x, 0, shuttlePos.z - receiverPos.z);
        float distance = horizontalDisplacement.magnitude;

        // 3. 球未落地前的击打判定
        if (distance <= hitDistanceThreshold && swing)
        {
            // 成功击打
            Debug.Log("成功击打！重新计算落点");

            // 重新计算落点
            Vector3 newEndPos = CalculateTargetPoint(opponentPos); // 使用前面封装的落点函数
            // 重新模拟飞行
            StopAllCoroutines();
            StartCoroutine(SimulateShuttlecockFlight(shuttlePos, newEndPos, vHorizontal, g));

            next_player = (next_player == 0) ? 1 : 0; // 切换接球方

            return false; // 球仍在空中
        }
        else
        {
            // 击打失败，飞行继续
            if (distance <= hitDistanceThreshold && !swing)
                Debug.Log("击打失败：未处于挥拍有效期");
            else if (distance > hitDistanceThreshold && swing)
                Debug.Log("击打失败：距离过远");
            return false;
        }
    }


    /// <summary>
    /// 模拟羽毛球从起点到落点的理想抛物线飞行
    /// </summary>
    /// <param name="startPos">起点位置</param>
    /// <param name="endPos">落点位置 Y = 0</param>
    /// <param name="vHorizontal">水平速度 (m/s)</param>
    /// <param name="g">重力加速度</param>
    /// <returns>协程可用 IEnumerator</returns>
    public IEnumerator SimulateShuttlecockFlight(Vector3 startPos, Vector3 endPos, float vHorizontal, float g)
    {
        // 1. 水平位移和方向
        Vector3 horizontalDisplacement = new Vector3(endPos.x - startPos.x, 0, endPos.z - startPos.z);
        float horizontalDistance = horizontalDisplacement.magnitude;
        Vector3 horizontalDir = horizontalDisplacement.normalized;

        // 2. 总飞行时间
        float totalTime = horizontalDistance / vHorizontal;
        Debug.Log("总飞行时间: " + totalTime + "秒");
        
        // 3. 垂直初速度
        float y0 = startPos.y;
        float yf = endPos.y;
        float Vy = g * totalTime * 0.5f;
        Debug.Log("垂直初速度: " + Vy + "m/s");

        // 4. 飞行模拟
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            // 水平位移
            Vector3 posHorizontal = startPos + horizontalDir * vHorizontal * elapsedTime;
            // 垂直位移
            
            float posY = y0 + Vy * elapsedTime - 0.5f * g * elapsedTime * elapsedTime;

            transform.position = new Vector3(posHorizontal.x, posY, posHorizontal.z);
            //Debug.Log("总飞行时间: " + totalTime + "秒");
            //Debug.Log("水平速度" + horizontalDir * vHorizontal + "水平位移" + horizontalDir * vHorizontal * elapsedTime + "垂直位移" + posY);
            Debug.Log("垂直速度: " + Vy + "加速度" + g);

            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保落点位置精确
        transform.position = endPos;
    }

    void Start()
    {
        shuttlecock = GetComponent<ShuttlecockEntity>();

        courtWidth = gameManager.courtWidth;
        courtLength = gameManager.courtLength;
        netHeight = gameManager.netHeight;
        g = gameManager.g;
        baseShuttlecockSpeed = gameManager.baseShuttlecockSpeed;

        left_hit = gameManager.left_hit;
        right_hit = gameManager.right_hit;
        if_left_even = gameManager.if_left_even;
        if_right_even = gameManager.if_right_even;

        next_player = -1;

        // 调用函数生成离散点
        GenerateCourtPoints();
    }

    void Update()
    {
        if (game_over) return;

        leftPlayerPos = LeftPlayer.PlayerPosition;
        rightPlayerPos = RightPlayer.PlayerPosition;
        isLeftSwinging = gameManager.isLeftSwinging;
        isRightSwinging = gameManager.isRightSwinging;
        if_left_even = gameManager.if_left_even;
        if_right_even = gameManager.if_right_even;

        // 发球逻辑（只在球不飞时触发）
        Debug.Log("isFlying: " + isFlying + ", left_hit: " + left_hit + ", right_hit: " + right_hit);
        //Debug.Log("Left Player Pos: " + leftPlayerPos + ", Right Player Pos: " + rightPlayerPos);
        if (!isFlying)
        {
            if (left_hit && !right_hit)
            {
                ShuttlecockServe(leftPlayerPos, nextPlayer: 1);
            }
            else if (!left_hit && right_hit)
            {

                ShuttlecockServe(rightPlayerPos, nextPlayer: 0);
            }
        }

        // 对打逻辑（球在空中）
        if (isFlying)
        {
            Vector3 receiverPos = next_player == 0 ? leftPlayerPos : rightPlayerPos;
            bool swing = next_player == 0 ? isLeftSwinging : isRightSwinging;
            Vector3 opponentPos = next_player == 0 ? rightPlayerPos : leftPlayerPos;

            bool landed = HandleShuttlecockHit(receiverPos, swing, transform.position, opponentPos);
            if (landed)
            {
                //球落地后重置
               isFlying = false;
                next_player = -1;
            }
        }
    }

    // ===================== 生成场地离散点 =====================
    void GenerateCourtPoints()
    {
        // X 轴离散
        int xCount = 6;
        float xStart = 0.1f;   // 距网前 0.1m
        float xStep = 1.1f;

        xLeft = new float[xCount];
        xRight = new float[xCount];

        for (int i = 0; i < xCount; i++)
        {
            float mid = xStart + i * xStep + xStep / 2f;

            xRight[i] = mid;
            xLeft[i] = -mid;
        }

        // Z 轴离散
        int zCount = 5;
        float zStart = -2.59f;
        float zStep = 1.036f;

        zPoints = new float[zCount];

        for (int i = 0; i < zCount; i++)
        {
            zPoints[i] = zStart + i * zStep + zStep / 2f;
        }

        // Debug 输出，可删除
        Debug.Log("XLeft: " + string.Join(", ", xLeft));
        Debug.Log("XRight: " + string.Join(", ", xRight));
        Debug.Log("Z: " + string.Join(", ", zPoints));
    }

    /// <summary>
    /// 计算落点，选离球员最远的离散中心点
    /// </summary>
    /// <param name="playerPos">球员位置</param>
    /// <returns>落点 Vector3</returns>
    public Vector3 CalculateTargetPoint(Vector3 playerPos)
    {
        float minZ = - (courtWidth / 2 - 0.46f);
        float maxZ = (courtWidth / 2 - 0.46f);

        if (left_hit) //左侧球员发球
        {
            minZ = if_left_even ? -(courtWidth / 2 - 0.46f) : 0; //球的落地在右侧半场的最小Z坐标
            maxZ = if_left_even ? 0 : (courtWidth / 2 - 0.46f); //球的落地在右侧半场的最大Z坐标
        }

        if (right_hit)
        {
            minZ = if_right_even ? 0 : -(courtWidth / 2 - 0.46f); //球的落地在左侧半场的最小Z坐标
            maxZ = if_right_even ? (courtWidth / 2 - 0.46f) : 0; //球的落地在左侧半场的最大Z坐标
        }

        //无限制
        float[] xArray;

        // 判断球员在左场还是右场
        if (playerPos.x < 0)
            xArray = xRight;
        else
            xArray = xLeft;

        Vector3 farthestPoint = Vector3.zero;

        float X_Distance = 0f;
        float target_x = 0f;
        foreach (float x in xArray)
        { 
            if (Mathf.Abs(x - playerPos.x)> X_Distance)
            {
                X_Distance = Mathf.Abs(x - playerPos.x);
                target_x = x;
            }
        }

        float Z_Distance = 0f;
        float target_z = 0f;
        foreach (float z in zPoints)
        {
            if (Mathf.Abs(z - playerPos.z) > Z_Distance && z >= minZ &&  z <= maxZ)
            {
                Z_Distance = Mathf.Abs(z - playerPos.z);
                target_z = z;
            }
        }
        farthestPoint.x = target_x;
        farthestPoint.z = target_z;
        farthestPoint.y = 0.05f; // 示例：发球或落点高度
        return farthestPoint;
    }

    // 发球/击球统一调用
    void ShuttlecockServe(Vector3 playerPos, int nextPlayer)
    {
        shuttlecockPos = new Vector3(playerPos.x + (playerPos.x < 0 ? relativeDistance : -relativeDistance),
                                     relativeHeight, playerPos.z);
        transform.position = shuttlecockPos;

        //target_position = CalculateTargetPoint(shuttlecockPos);
        //Debug.Log("球员位置" + playerPos + "球起点" + shuttlecockPos + "球落点: " + target_position);
        //StartCoroutine(SimulateShuttlecockFlight(shuttlecockPos, target_position, baseShuttlecockSpeed, g));
        //HandleShuttlecockHit()
        //isFlying = true;
        //this.next_player = nextPlayer;
    }
}
