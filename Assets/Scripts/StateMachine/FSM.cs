using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//管理敌人所用的状态机
public enum stateType
{
    Idle, Patrol, Chase, Attack, Hit, Death
}

[Serializable]

public class Parametar
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;

    public Transform[] patrolPoints;
    public Transform[] chasePoints;

    public Animator anim;

    public Transform target;

    public LayerMask 攻击目标层级;//怪物攻击的层级
    public Transform attackPoint;
    public float 怪物攻击半径;

    public bool isHit;//玩家时候进入怪物追击范围


    public bool isDead;

    public bool canAttack = true;//怪物能否攻击

    public int healthCurrent;
    public int healthMax;

    public Image EnemyHealth;

}


public class FSM : MonoBehaviour
{

    public IState currentState;

    private Dictionary<stateType, IState> States = new Dictionary<stateType, IState>();

    public Parametar parametar = new Parametar();
    private Rect 射线检测方块;
    public BoxCollider2D 射线检测方块碰撞体;
    public float 追击范围射线长度 = 0.2f;
    private float RayOffset = 0.15f;
    public LayerMask 追击检测层级;//怪物碰撞检测的层级

    public bool isHitByPlayer;//怪物是否受到玩家攻击


    public void Awake()
    {
        States.Add(stateType.Idle, new IdleState(this));
        States.Add(stateType.Patrol, new PatrolState(this));
        States.Add(stateType.Chase, new ChaseState(this));
        States.Add(stateType.Attack, new AttackState(this));
        States.Add(stateType.Hit, new HitState(this));
        States.Add(stateType.Death, new DeathState(this));
        //dic();
    }




    void Start()
    {
        parametar.anim = GetComponent<Animator>();
        TransitionState(stateType.Idle);
        parametar.healthCurrent = parametar.health;
        parametar.healthMax = parametar.health;


    }

    //void dic()
    //{
    //    foreach(KeyValuePair<stateType, IState> entey in States)
    //    {
    //        Debug.Log(entey.Key + ":" + entey.Value);
    //    }
    //}

    public void Update()
    {
        currentState.OnUpdate();

        if (parametar.health > 0)
        {
            设置射线检测();
            射线检测追踪范围();
            attackEnemy();
            parametar.isDead = false;
            EnemyHealthBar();
        }

        else
        {
            TransitionState(stateType.Death);
            parametar.isDead = true;
        }


       


    }

    void attackEnemy()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "playerAttackEff")
        {
            isHitByPlayer = true;
        }
    }


    public void enemyDead()
    {
        Destroy(gameObject);
    }

    public void TransitionState(stateType stateType)//切换状态
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = States[stateType];
        currentState.OnEnter();
    }

    public void FlipTo(Transform flipTarget)
    {
        if (flipTarget != null)
        {
            if (transform.position.x > flipTarget.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (transform.position.x < flipTarget.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }


    public void 设置射线检测()
    {
        射线检测方块 = new Rect(射线检测方块碰撞体.bounds.min.x,
                                射线检测方块碰撞体.bounds.min.y,
                                射线检测方块碰撞体.bounds.size.x,
                                射线检测方块碰撞体.bounds.size.y
                                );
    }

    public void 射线检测追踪范围()
    {
        float rayLength = 追击范围射线长度;
        rayLength += 射线检测方块.height / 2;
        Vector2 射线中心点 = 射线检测方块.center;
        射线中心点.y += RayOffset;
        bool isCollide = false;
        RaycastHit2D[] hitInfo = new RaycastHit2D[2];
        hitInfo[0] = RayCast(射线中心点, Vector2.left, rayLength, 追击检测层级, Color.blue, true);
        hitInfo[1] = RayCast(射线中心点, Vector2.right, rayLength, 追击检测层级, Color.blue, true);

        if (hitInfo[0])
        {
            parametar.target = hitInfo[0].transform;
            isCollide = true;
        }
        else if (hitInfo[1])
        {
            parametar.target = hitInfo[1].transform;
            isCollide = true;
        }
        else
        {
            parametar.target = null;
            isCollide = false;
        }
        if (isCollide)
        {
            parametar.isHit = true;
        }
        else
        {
            parametar.isHit = false;
        }
    }


     public void EnemyHealthBar()
    {
        parametar.EnemyHealth.fillAmount = (float)parametar.healthCurrent / parametar.healthMax;
    }

    public IEnumerator EnemyAttackDelta()//怪物攻击间隔时间
    {
        yield return new WaitForSecondsRealtime(5f);
        parametar.canAttack = true;
    }
    public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parametar.attackPoint.position, parametar.怪物攻击半径);
    }




}
