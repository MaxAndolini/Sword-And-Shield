using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private const float attackCoolDown = 4.5f;
    [SerializeField] private float damage;

    public float Speed;

    [SerializeField] private float stoppingDistance;

    public bool isDead;

    private Animator anim;
    private GameObject enemies;

    private NavMeshAgent enemy;
    private AudioSource footStep;

    private float lastAttackTime = 2;
    private float lastFootStep;

    private GameObject player;

    private void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.Find("EnemyCreator");
    }

    private void Update()
    {
        if (isDead == false && player.GetComponent<Character>().isDead == false)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < stoppingDistance)
            {
                StopEnemy();
                Attack();

                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Speed);
                transform.LookAt(player.transform);
            }
            else
            {
                GoToTarget();
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Speed);
            }
        }
        else
        {
            StopEnemy();
        }
    }

    private void GoToTarget()
    {
        enemy.isStopped = false;
        enemy.SetDestination(player.transform.position);
        anim.SetBool("Walking", true);

        if (footStep == null) footStep = FindObjectOfType<Audio>().EnemyWalk(gameObject);

        if (lastFootStep + 0.5f < Time.time)
        {
            lastFootStep = Time.time;
            if (!footStep.isPlaying) footStep.Play();
        }
    }

    private void StopEnemy()
    {
        enemy.isStopped = true;
        anim.SetBool("Walking", false);
        Destroy(footStep);
    }

    private void Attack()
    {
        if (!(Time.time - lastAttackTime >= attackCoolDown)) return;
        anim.SetTrigger("Attack");
        lastAttackTime = Time.time;

        FindObjectOfType<Audio>().PlayEnemy(gameObject, Audio.Audios.Sword);

        player.GetComponent<Character>().TakeDamage(damage);
        player.GetComponent<Character>().CheckHealth();

        player.GetComponent<Character>().ShieldTakeDamageToDurability(damage);
        player.GetComponent<Character>().CheckShieldDurability();
    }

    public void Death()
    {
        anim.SetTrigger("Die");
        Destroy(gameObject, 5f);
        player.GetComponent<Character>().AddScore();
        enemies.GetComponent<SpawnEnemy>().ReduceEnemy();
        damage = 0;
        Speed = 0f;
        isDead = true;
    }
}