using Unity.VisualScripting;
using UnityEngine;

public class Shuttlecock_Move : MonoBehaviour
{
    ShuttlecockEntity shuttlecock;
    public GameManager gameManager;
    float courtLength;
    float courtWidth;
    float netHeight;


    //X [x0, x1,...,x6]
    //Z [z0, z1, ...,z5]
    //左右各一组

    // 羽毛球发球位置
    float relativeHeight = 1.15f;
    float relativeDistance = 0.35f;
    bool left_hit;
    bool right_hit;
    bool if_even;

    // 玩家位置
    Vector3 leftPlayerPos;
    Vector3 rightPlayerPos;
    SwingType leftPlayerSwingType;
    SwingType rightPlayerSwingType;
    ShotDirection leftShotDirection;
    ShotDirection rightShotDirection;
    Vector3 shuttlecockPos;

    //飞行参数
    Vector3 startPos = Vector3.zero;
    Vector3 endPos = Vector3.zero;

    //上次击球方
    public next_player = Left or right


    void Start()
    {
        shuttlecock = GetComponent<ShuttlecockEntity>();
        courtWidth = gameManager.courtWidth;
        courtLength = gameManager.courtLength;
        netHeight = gameManager.netHeight;
        left_hit = gameManager.left_hit;
        right_hit = gameManager.right_hit;
        if_even = gameManager.if_even;
    }

    void Update()
    {
        left_hit = gameManager.left_hit;
        right_hit = gameManager.right_hit;
        if_even = gameManager.if_even;

        if (left_hit && !right_hit)
        {
            leftPlayerPos = gameManager.leftPlayerPos;
            shuttlecockPos = new Vector3(leftPlayerPos.x + relativeDistance, relativeHeight, leftPlayerPos.z);
            //跟随球员
            transform.position = shuttlecockPos;

            leftPlayerSwingType = gameManager.leftPlayerSwingType;
            leftShotDirection = gameManager.leftShotDirection;
            
            next_player = ？

        }

        if (!left_hit && right_hit)
        {
            rightPlayerPos = gameManager.rightPlayerPos;
            shuttlecockPos = new Vector3(rightPlayerPos.x - relativeDistance, relativeHeight, rightPlayerPos.z);
            //跟随球员
            transform.position = shuttlecockPos;
            rightPlayerSwingType = gameManager.rightPlayerSwingType;
            rightShotDirection = gameManager.rightShotDirection;

            next_player = ？
            

        }

        if (!left_hit && !right_hit)
        {
            next_player = ？
            //获取next_player的

        }

    }


    void get_end_point(X, Z, )

    void HitShuttlecock(bool left_hit, bool right_hit, bool if_even, SwingType playerSwingType, ShotDirection PlayerShotDirection)
    {
        //起点

        // 根据挥拍类型和击球方向,并预估羽毛球落点
        if (left_hit && playerSwingType == SwingType.Drop)
        {
            if (if_even)
            {

            }
        }
    }
}