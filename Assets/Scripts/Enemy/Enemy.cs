using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Enemy : MonoBehaviour
{
    public int enemyDamage;
    public bool isHit;//怪物是否收到攻击

    public bool hitPlayerByTouch;

    public CircleCollider2D AttackArea;

    private PlayerHealth playerHealth;

    private FSM fsm;

    public GameObject[] dropObj;
    public void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        fsm = GameObject.FindGameObjectWithTag("Enemy").GetComponent<FSM>();
        hitPlayerByTouch = false;
        AttackArea.enabled = false;
    }

    public void FixedUpdate()
    {
        Vector3 front = new Vector3(-1, 0, 0);
        if (transform.localScale.x < 0)
        {
            front = new Vector3(1, 0, 0);
        }

        if (fsm.parametar.health <= 0)
        {
            DropObj();
        }
    }

    public void DropObj()
    {
        Debug.Log(dropObj.Length);
        int index = Random.Range(0, dropObj.Length);
        Instantiate(dropObj[index], gameObject.transform.position, Quaternion.identity);
    }


    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    //public void enemyAttackDelta()
    //{
    //    StartCoroutine(EnemyAttackDelta());
    //}
    //public IEnumerator EnemyAttackDelta()//怪物攻击间隔时间
    //{
    //    yield return new WaitForSecondsRealtime(5f);
    //}

    public void AttackAreaEnable()
    {
        AttackArea.enabled=true;
    }

    public void AttackAreaDisable()
    {
        AttackArea.enabled=false;

    }


    private void OnTriggerEnter2D(Collider2D collision)//怪物碰撞玩家造成伤害
    {
        if (collision.gameObject.CompareTag("Player")&&hitPlayerByTouch)
        {
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer(enemyDamage);

            }
        }
    }


}
