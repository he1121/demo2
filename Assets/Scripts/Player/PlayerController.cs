using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float 水平输入分量_;
    public float 移动速度 = 6;

    public static Vector2 _newPosition;//记录玩家下一个坐标值
    Vector2 _speed;//速度中间量

    public float gravity = -12;//重力系数
    public bool 开启重力 = true;
    public float 缓冲参数 = 0;

    public BoxCollider2D _boxCollider;

    Rect 射线检测方块;
    bool isFalling = false;
    float RayOffset = 0.15f;
    public LayerMask PlatformMask = 0;
    public LayerMask CrossPlatformMask = 0;
    float _smallValue = 0.0001f;//容错校验参数
    GameObject StandingOn;//当前站立平台
    bool is碰撞下方;

    bool isGrounded { get { return is碰撞下方; } }
    Vector2 辅助外力参数;
    bool canJump;
    public float jumpHeight = 2;
    public float doubleJumpHeight = 2;

    public float 垂直输入分量_;
    public float 关闭对可穿越平台碰撞时间 = 0.3f;
    LayerMask platformMaskSave;

    public float 头顶射线长度 = 0.2f;

    private Animator animPlayer;

    private bool canDoubleJump;

    private bool canAttack=true;
    public float attackDeltaTime;

    public int 攻击力;

    public GameObject attackEff;
    private PlayerHealth ph;

    private void Awake()
    {
        platformMaskSave = PlatformMask;
    }

    void Start()
    {
        animPlayer = GetComponentInParent<Animator>();
        ph = GetComponent<PlayerHealth>();
        attackEff.SetActive(false);
    }

    void Update()
    {
        if (!PlayerHealth.isAlive)
        {
            Debug.Log("玩家死亡");
           _newPosition = new Vector2(0, 0);
        }
        else
        {
            Debug.Log("玩家还活着");

            //Run();
            input();
            beforeMove();
            射线检测();
            move();
            Flip();
            Gravity();
            SwitchAnim();
            Attack();
            重置参数();
        }


    }

    void input()
    {
        水平输入分量_ = Input.GetAxisRaw("Horizontal");
        垂直输入分量_ = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void beforeMove()
    {
        _speed.x = 水平输入分量_ * 移动速度;
        _newPosition = _speed * Time.deltaTime;
    }

    void move()
    {
        this.transform.Translate(_newPosition, Space.World);
        bool playerHasXAxisSpeed = Mathf.Abs(_newPosition.x) > Mathf.Epsilon;
        animPlayer.SetBool("walk", playerHasXAxisSpeed);
    }
    void Gravity()
    {

        if (!开启重力) return;
        if (开启重力 && isGrounded) 
        { 
            _speed.y = 0.0f;
        } 
        _speed.y += (gravity * Time.deltaTime);//重力加速度
        if (缓冲参数 != 0)
        {
            _speed.y *= 缓冲参数;//几何级衰减
        }
        
    }

    void 射线检测()
    {
        设置射线检测();
        射线检测脚下();
        射线检测头顶();
    }
    void 设置射线检测()
    {
        射线检测方块 = new Rect(_boxCollider.bounds.min.x,
                                _boxCollider.bounds.min.y,
                                _boxCollider.bounds.size.x,
                                _boxCollider.bounds.size.y                                    
                                );
    }

    void 射线检测脚下()
    {
        if (_newPosition.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        if ((gravity > 0) && (!isFalling))
        {
            return;
        }
        float rayLength = 射线检测方块.height / 2 + RayOffset;
        float rayForwardLength= 射线检测方块.width / 2 + RayOffset;
        if (isFalling)
        {
            rayLength += Mathf.Abs(_newPosition.y);
        }
        Vector2 射线中心点 = 射线检测方块.center;


        射线中心点.y += RayOffset;

        RaycastHit2D[] hitInfo = new RaycastHit2D[10];
        bool isTouchGround = false;
        bool isTouchWall = false;

        float direction = -transform.localScale.x;
        Vector2 hitWallDir = new Vector2(direction, 0f);
        float ForwardOffset = 射线检测方块.height / 9;
        Vector2 墙壁检测中心点1 = new Vector2(射线中心点.x,射线中心点.y-1f+0*ForwardOffset);
        Vector2 墙壁检测中心点2 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 1 * ForwardOffset);
        Vector2 墙壁检测中心点3 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 2 * ForwardOffset);
        Vector2 墙壁检测中心点4 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 3 * ForwardOffset);
        Vector2 墙壁检测中心点5 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 4 * ForwardOffset);
        Vector2 墙壁检测中心点6 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 5 * ForwardOffset);
        Vector2 墙壁检测中心点7 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 6 * ForwardOffset);
        Vector2 墙壁检测中心点8 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 7 * ForwardOffset);
        Vector2 墙壁检测中心点9 = new Vector2(射线中心点.x, 射线中心点.y - 1f + 8 * ForwardOffset);



        if (_newPosition.y > 0)//向上跳跃时不检测可穿越平台
        {
            hitInfo[0] = RayCast(射线中心点, -transform.up, rayLength, PlatformMask &~CrossPlatformMask, Color.red, true);
            //hitInfo[1] = RayCast(墙壁检测中心点, hitWallDir, rayLength, PlatformMask & ~CrossPlatformMask, Color.green, true);

        }
        else
        {
            hitInfo[0] = RayCast(射线中心点, -transform.up, rayLength, PlatformMask, Color.red, true);

        }

        if (_newPosition.x > 0)
        {
            hitInfo[1] = RayCast(墙壁检测中心点1, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[2] = RayCast(墙壁检测中心点2, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[3] = RayCast(墙壁检测中心点3, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[4] = RayCast(墙壁检测中心点4, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[5] = RayCast(墙壁检测中心点5, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[6] = RayCast(墙壁检测中心点6, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[7] = RayCast(墙壁检测中心点7, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[8] = RayCast(墙壁检测中心点8, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[9] = RayCast(墙壁检测中心点9, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);


        }
        else
        {
            hitInfo[1] = RayCast(墙壁检测中心点1, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[2] = RayCast(墙壁检测中心点2, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[3] = RayCast(墙壁检测中心点3, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[4] = RayCast(墙壁检测中心点4, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[5] = RayCast(墙壁检测中心点5, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[6] = RayCast(墙壁检测中心点6, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[7] = RayCast(墙壁检测中心点7, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[8] = RayCast(墙壁检测中心点8, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
            hitInfo[9] = RayCast(墙壁检测中心点9, hitWallDir, rayForwardLength, PlatformMask & ~CrossPlatformMask, Color.green, true);
        }
        if (hitInfo[0])
        {
            isTouchGround = true;
        }
        if (hitInfo[1]|| hitInfo[2]|| hitInfo[3]|| hitInfo[4]|| hitInfo[5]|| hitInfo[6]|| hitInfo[7]|| hitInfo[8]|| hitInfo[9])
        {
            isTouchWall = true;
            Debug.Log("前面有墙");
        }
        if (isTouchWall&&transform.localScale==new Vector3(-水平输入分量_,1,1))
        {
            _newPosition.x = 0;
        }

        if (isTouchGround)
        {
            StandingOn = hitInfo[0].collider.gameObject;
            isFalling = false;
            is碰撞下方 = true;

            if (辅助外力参数.y > 0)
            {
                _newPosition.y = _speed.y * Time.deltaTime;
                is碰撞下方 = false;
            }
            else
            {
                _newPosition.y = -Mathf.Abs(hitInfo[0].point.y - 射线中心点.y) + 射线检测方块.height / 2 + RayOffset;
            }

            

            if (Mathf.Abs(_newPosition.y) < _smallValue)
            {
                _newPosition.y = 0;
            }
        }
        else
        {
            is碰撞下方 = false;
        }
    }

    void 射线检测头顶()
    {
        if (_newPosition.y < 0)
        {
            return;
        }
        float rayLength = 头顶射线长度;
        rayLength += 射线检测方块.height / 2;
        Vector2 射线中心点 = 射线检测方块.center;
        射线中心点.y += RayOffset;
        bool isCollide = false;
        RaycastHit2D[] hitInfo = new RaycastHit2D[1];
        hitInfo[0] = RayCast(射线中心点, Vector2.up, rayLength, PlatformMask & ~CrossPlatformMask, Color.blue, true);

        if (hitInfo[0])
        {
            isCollide = true;
        }
        if (isCollide)
        {
            _speed.y = 0;
            _newPosition.y = hitInfo[0].distance - 射线检测方块.height / 2;
        }
    }

    public static RaycastHit2D RayCast(Vector2 rayOriginPoint,Vector2 rayDirection,float rayDistance,LayerMask mask,Color color,bool drawGizmo)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }



    void Flip()//翻转角色
    {
        bool playerHasXAxisSpeed = Mathf.Abs(_newPosition.x) > Mathf.Epsilon;
        if (!playerHasXAxisSpeed)
            return;
        else
            {
                if (_newPosition.x > 0.0f)
                {
                    transform.localScale= new Vector3(-1,1,1);
                }
                if (_newPosition.x < 0.0f)
                {
                transform.localScale = new Vector3(1, 1, 1);
            }
            }
    }

    /*
       void Run()
    {
        float moveDir = Input.GetAxisRaw("Horizontal");
        Vector2 playerVel = new Vector2(moveDir * runSpeed, rbPlayer.velocity.y);
        rbPlayer.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(rbPlayer.velocity.x) > Mathf.Epsilon;
        animPlayer.SetBool("walk", playerHasXAxisSpeed);        
    }
    */

    void Jump()
    {
        if (isGrounded)
        {
            if (垂直输入分量_ < 0)
            {              
                if (StandingOn.layer == LayerMask.NameToLayer("可穿越平台"))
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
                    StartCoroutine(关闭对可穿越平台的碰撞(关闭对可穿越平台碰撞时间));
                }
                return;
            }
            _speed.y = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(gravity));
            辅助外力参数.y = _speed.y;
            animPlayer.SetBool("jump", true);
            canDoubleJump = true;
        }
        else
        {
            if (canDoubleJump)
             { 
                 animPlayer.SetBool("doublejump", true);

                 _speed.y = Mathf.Sqrt(2f * doubleJumpHeight * Mathf.Abs(gravity));
                 canDoubleJump = false;
             }
            return;
        }
    }
    public IEnumerator 关闭对可穿越平台的碰撞(float time)
    {
        PlatformMask-=CrossPlatformMask;
        yield return new WaitForSeconds(time);
        PlatformMask = platformMaskSave;
    }
    void 重置参数()
    {
        辅助外力参数.x = 0;
        辅助外力参数.y = 0;
    }

    void SwitchAnim()
    {
        animPlayer.SetBool("idle", true);
        if (animPlayer.GetBool("jump"))
        {
            if (_newPosition.y < 0.0f)
            {
                animPlayer.SetBool("jump", false);
                animPlayer.SetBool("land", true);
            }
        }
        else if (isGrounded)
        {
            animPlayer.SetBool("land", false);
            animPlayer.SetBool("idle", true);
        }
        if (animPlayer.GetBool("doublejump"))
        {
            if (_newPosition.y < 0.0f)
            {
                animPlayer.SetBool("doublejump", false);
                animPlayer.SetBool("land", true);
            }
        }
        if(_newPosition.y<0.0f&&!isGrounded)
        {
            animPlayer.SetBool("walk", false);
            animPlayer.SetBool("land", true);
        }
        if(_newPosition==new Vector2(0,0)&& isGrounded)
        {
            animPlayer.SetBool("idle", true);
        }


    }

    void Attack()
    {
        if (Input.GetButtonDown("Attack")&&canAttack)
        {
            animPlayer.SetTrigger("attack");
            attackEff.SetActive(true);
            canAttack = false;
            StartCoroutine(attackEffTime());
            StartCoroutine(CanAttack());
        }
    }

    IEnumerator attackEffTime()
    {
        yield return new WaitForSeconds(0.2f);
        attackEff.SetActive(false);

    }
    IEnumerator CanAttack()
    {
        yield return new WaitForSeconds(attackDeltaTime);
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        switch (collision.tag)
        {
            case "Deadline":
                PlayerHealth.isAlive = false;
                animPlayer.SetTrigger("dead");
                Invoke("PlayerRelive", 1f);
                break;
            case "healthObj":
                ph.health += 10;
                Debug.Log("拾取" + collision.name);
                Destroy(collision.gameObject);
                break;
            case "attackObj":
                ph.health += 10;
                Debug.Log("拾取" + collision.name);
                Destroy(collision.gameObject);
                break;
            case "moveSpeedObj":
                攻击力 += 2;
                Destroy(collision.gameObject);
                break;
            case "全能药水":
                移动速度 += 2;
                攻击力 += 2;
                ph.health += 10;
                Destroy(collision.gameObject);
                break;
            case "秘方碎片":
                Destroy(collision.gameObject);
                break;
        }
        //if (collision.tag == "Deadline")
        //{
        //    PlayerHealth.isAlive = false;
        //    animPlayer.SetTrigger("dead");
        //    Invoke("PlayerRelive", 1f);
        //}

        //if (collision.tag == "healthObj")
        //{
        //    ph.health += 10;
        //    Debug.Log("拾取" + collision.name);
        //    Destroy(collision.gameObject);
        //}
        //if (collision.tag == "attackObj")
        //{
        //    攻击力 += 2;
        //    Destroy(collision.gameObject);

        //}
        //if (collision.tag == "moveSpeedObj")
        //{
        //    移动速度 += 2;
        //    Destroy(collision.gameObject);

        //}
        //if (collision.tag == "全能药水")
        //{
        //    移动速度 += 2;
        //    攻击力 += 2;
        //    ph.health += 10;
        //    Destroy(collision.gameObject);
        //}
        //if (collision.tag == "秘方碎片")
        //{
        //    Destroy(collision.gameObject);

        //}
    }

    public void PlayerRelive()
    {
        PlayerHealth.isAlive = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
