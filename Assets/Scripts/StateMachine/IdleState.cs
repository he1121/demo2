using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private FSM manager;
    private Parametar parametar;
    private float Timer;

    public IdleState(FSM manager)//待机
    {
        this.manager = manager;
        this.parametar = manager.parametar;
    }
    public void OnEnter()
    {
        parametar.anim.Play("Idle");
        Debug.Log("待机状态");

    }

    public void OnUpdate()
    {
        Timer += Time.deltaTime;

        if (manager.isHitByPlayer)
        {
            manager.TransitionState(stateType.Hit);
            Debug.Log("怪物待机时受到攻击");

        }
        if (parametar.target!=null&& parametar.isHit)
        {
            manager.TransitionState(stateType.Chase);
            Debug.Log("追击状态");

        }

        if (Timer >= parametar.idleTime)
        {
            manager.TransitionState(stateType.Patrol);
            Debug.Log("巡逻状态");

        }
    }

    public void OnExit()
    {
        Timer = 0;
    }
}

public class PatrolState : IState//巡逻
{
    private FSM manager;
    private Parametar parametar;
    private int patrolPos;

    public PatrolState(FSM manager)
    {
        this.manager = manager;
        this.parametar = manager.parametar;
    }
    public void OnEnter()
    {
        parametar.anim.Play("Walk");
        Debug.Log("巡逻状态");

    }

    public void OnUpdate()
    {
        manager.FlipTo(parametar.patrolPoints[patrolPos]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                                                        parametar.patrolPoints[patrolPos].position,
                                                        parametar.moveSpeed * Time.deltaTime);
        //if (FSM.isHitByPlayer)
        //{
        //    Debug.Log("怪物巡逻时受到攻击");
        //    manager.TransitionState(stateType.Hit);
        //}

        if (Vector2.Distance(manager.transform.position, parametar.patrolPoints[patrolPos].position) < .1f)
        {
            manager.TransitionState(stateType.Idle);
            Debug.Log(manager.gameObject.name + "待机");

        }
        if (parametar.target!=null && parametar.isHit)
        {
            manager.TransitionState(stateType.Chase);
            Debug.Log("追击状态");

        }
    }

    public void OnExit()
    {
        patrolPos++;
        if (patrolPos >= parametar.patrolPoints.Length)
        {
            patrolPos = 0;
        }
    }
}


public class ChaseState : IState//追击状态
{
    private FSM manager;
    private Parametar parametar;

    public ChaseState(FSM manager)
    {
        this.manager = manager;
        this.parametar = manager.parametar;
    }
    public void OnEnter()
    {
        parametar.anim.Play("Walk");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parametar.target);
        Debug.Log("是否受到玩家攻击：" + manager.isHitByPlayer);

        if (manager.isHitByPlayer)
        {
            parametar.canAttack = false;
            manager.TransitionState(stateType.Hit);
            Debug.Log("怪物追击时受到攻击");
        }
        else
        {
            parametar.canAttack = true;
        }
        if (parametar.target)
        {//怪物向玩家移动
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,new Vector2(parametar.target.position.x,manager.transform.position.y), parametar.chaseSpeed * Time.deltaTime);

        }
        if (parametar.target == null || manager.transform.position.x < parametar.chasePoints[0].position.x || manager.transform.position.x > parametar.chasePoints[1].position.x)
        {
            manager.TransitionState(stateType.Idle);
        }
        if (Physics2D.OverlapCircle(parametar.attackPoint.position, parametar.怪物攻击半径, parametar.攻击目标层级)&&parametar.isHit&& parametar.target)
        {
            if (parametar.canAttack)
            {
                manager.TransitionState(stateType.Attack);
            }
            else
            {
                manager.TransitionState(stateType.Hit);
            }
        }

    }

    public void OnExit()
    {
    }
}

public class AttackState : IState
{
    private FSM manager;
    private Parametar parametar;
    private AnimatorStateInfo info;

    public AttackState(FSM manager)
    {
        this.manager = manager;
        this.parametar = manager.parametar;
    }
    public void OnEnter()
    {
        parametar.anim.Play("Attack");
    }

    public void OnUpdate()
    {
        info = parametar.anim.GetCurrentAnimatorStateInfo(0);   
        if (info.normalizedTime >= .95f&&parametar.canAttack)//攻击动画播放完之后回到追击状态
        {
            manager.TransitionState(stateType.Chase);         
        }
        
    }

    public void OnExit()
    {

    }
}

public class HitState : IState
{
    private FSM manager;
    private Parametar parametar;
    private AnimatorStateInfo info;
    private PlayerController pc;
    public HitState(FSM manager)
    {
        this.manager = manager;
        this.parametar = manager.parametar;
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


    }
    public void OnEnter()
    {
        if (parametar.healthCurrent > 0)
        {
            parametar.anim.Play("Hit");
            parametar.healthCurrent-=pc.攻击力;
            AudioManager.EnemyDamageAudio();
            Debug.Log(manager.gameObject.name+"剩余血量："+ parametar.healthCurrent);
        }
        else
        {
            manager.TransitionState(stateType.Death);
        }
    }

    public void OnUpdate()
    {
        info = parametar.anim.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= .95f )//受伤动画播放完之后回到追击状态
        {
            if(parametar.healthCurrent > 0)
            {
                parametar.target = GameObject.FindWithTag("Player").transform;
                manager.TransitionState(stateType.Chase);
            }
            else
            {
                parametar.isDead = true;
                manager.TransitionState(stateType.Death);

            }

        }

    }

    public void OnExit()
    {
        manager.isHitByPlayer = false;

    }
}

public class DeathState : IState
{
    private FSM manager;
    private Parametar parametar;
    private AnimatorStateInfo info;

    private Enemy enemy;


    public DeathState(FSM manager)
    {
        this.manager = manager;
        this.parametar = manager.parametar;
        enemy = manager.GetComponent<Enemy>();
    }
    public void OnEnter()
    {
        parametar.anim.Play("Death");
    }

    public void OnUpdate()
    {
        info = parametar.anim.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= .95f)//死亡动画播放完之后消失
        {
            enemy.DropObj();
            manager.enemyDead();
            Debug.Log("怪物死亡");

        }
    }

    public void OnExit()
    {
    }
}
