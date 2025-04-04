using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Behaviour : MonoBehaviour
{

    public float AttackDistance;
    public float MoveSpeed;
    public float Timer;
    public Transform LeftLimit;
    public Transform RightLimit;
    [HideInInspector]public Transform target;
    [HideInInspector]public bool inRange;
    public GameObject HotZone;
    public GameObject TriggerArea;

    
    

    
    
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool cooling;
    private float intTimer;

    void Awake()
    {
        SelectTarget();
        intTimer = Timer;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!attackMode){
            Move();
        }
        
        if(!insideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack")){
            SelectTarget();
        }

        if(inRange){
            EnemyLogic();

        }
        
    }

    
    void EnemyLogic(){
        distance = Vector2.Distance(transform.position, target.position);
        if(distance > AttackDistance){
            StopAttack();
        }else if(AttackDistance >= distance && cooling == false){
            Attack();
        }
        if(cooling){
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    public void Move(){
        anim.SetBool("CanWalk", true);
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack")){
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
            
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
        }  
    }

    void Attack(){
        Timer = intTimer;
        attackMode = true;
        anim.SetBool("CanWalk", false);
        anim.SetBool("Attack", true);

    }

    void Cooldown(){
        Timer -= Time.deltaTime;
        if(Timer <= 0 && cooling && attackMode){
            cooling = false;
            Timer = intTimer;
        }
    }

    void StopAttack(){
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }


    public void TriggerCooling(){
        cooling = true;
    }

    private bool insideOfLimits(){
        return transform.position.x > LeftLimit.position.x && transform.position.x < RightLimit.position.x;
    }

    public void SelectTarget(){
        float distanceToLeft = Vector2.Distance(transform.position, LeftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, RightLimit.position);

        if(distanceToLeft > distanceToRight){
            target = LeftLimit;
        }else{
            target = RightLimit;
        }
        Flip();
    }
    public void Flip(){
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x){

            rotation.y = 180f;

        }else{
            rotation.y = 0f;
        }
        transform.eulerAngles = rotation;
    }
}