using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public  int Blinks;
    public  float time;

    private  Animator animPlayer;
    public AudioSource playerAudio;

    private  Renderer myRender;

    public static  bool isAlive;
    public  bool canMove;

    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Enemy enemy;

    private AnimatorStateInfo info;


    void Start()
    {
        isAlive = true;

        myRender = GetComponent<Renderer>();

        animPlayer = gameObject.GetComponent<Animator>();

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        HealthBar.healthMax = health;
    }

    void Update()
    {
        if (health <= 0)
        {
            isAlive = false;
        }
        else
        {
            isAlive = true;
        }

        info = animPlayer.GetCurrentAnimatorStateInfo(0);
        HealthBar.healthCurrent = health;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //玩家碰撞到怪物攻击后受伤
        if (collision.gameObject.CompareTag("EnemyAttackEff"))
        {
            enemy =collision.gameObject.transform.parent.GetComponent<Enemy>();

            if (playerHealth != null)
            {
                DamagePlayer(enemy.enemyDamage);

            }
        }
    }



    public void DamagePlayer(int damage)
    {
        health -= damage;
        Debug.Log(health);
        HealthBar.healthCurrent = health;
        if (!isAlive)
        {
            animPlayer.SetTrigger("dead");
            Invoke("playerDead", 1f);
        }
        else
        {
            animPlayer.SetTrigger("damage");
            AudioManager.PlayerDamageAudio();
            int blinks = Blinks;
            BlinkPlayer(blinks, time);
        }
        
    }

    void playerDead()
    {
        Destroy(gameObject);
    }
    void BlinkPlayer(int numBlinks,float blinkTime)
    {
        StartCoroutine(DoBlink(numBlinks,blinkTime));
    }

    IEnumerator DoBlink(int numBlinks, float blinkTime)
    {
        for(int i = 0; i < numBlinks * 2; i++)
        {
            myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(blinkTime);
        }
        myRender.enabled = true;
    }
}
