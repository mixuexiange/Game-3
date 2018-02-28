using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum EnemyState
{
    Idle, TurnAround, Walk, Run, Attack, Dead
}


public delegate void ThinkMethod();

[RequireComponent(typeof(RotationMap))]
public class EnemyAIController : MonoBehaviour
{
    [Header("速度")]
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;

    [Header("时间")]
    public float thinkTimeOfIdle;
    public float thinkTimeOfAttack;
    public float animationDampTime;
    public float destroyDelayTime;

    [Header("距离")]
    public float inspectDistance;
    public float standAttackDistance;

    [Header("属性")]
    public EnemyState enemyState;
    public string playerTag;
    public float maxHealth;
    public float causeDamage;

    private float health;
    private bool resetMap;
    private GameObject player;
    private Quaternion targetRotation;

    private const int RotateAngle = 45;

    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag(playerTag);
    }

    void Update()
    {
        ControlSlider();
        ControlRotationMap();

        if (enemyState != EnemyState.Dead)
        {
            ControlState();
            ControlPosition();
            ControlRotation();
        }
        ControlAnimation();
    }

    void ControlSlider()
    {
        Debug.DrawRay(transform.position, transform.up * 5, Color.red, 500);
        Slider slider = GetComponentInChildren<Slider>();
        slider.value = (float)health / maxHealth;
    }
    void ControlRotationMap()
    {
        switch (enemyState)
        {
            case EnemyState.Run:
                RotationMap map = GetComponent<RotationMap>();
                if (!resetMap)
                {
                    map.ResetMap();
                    resetMap = true;
                }
                break;
            default:
                resetMap = false;
                break;
        }
    }

    void ControlState()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > inspectDistance)
        {
            InvokeThinkMethod(ThinkAttackState, ThinkIdleState, thinkTimeOfIdle);
        }
        else
        {
            if (distance > standAttackDistance)
                InvokeThinkMethod(ThinkIdleState, ThinkAttackState, thinkTimeOfAttack);
            else
                enemyState = EnemyState.Attack;
        }
    }

    void InvokeThinkMethod(ThinkMethod cancelMethod, ThinkMethod invokeMethod, float delay)
    {
        CancelInvoke(GetMethodName(cancelMethod));
        if (!IsInvoking(GetMethodName(invokeMethod)))
            Invoke(GetMethodName(invokeMethod), delay);
    }

    string GetMethodName(ThinkMethod th) { return th.Method.Name; }

    void ThinkIdleState()
    {
        enemyState = (EnemyState)Random.Range((int)EnemyState.Idle, (int)EnemyState.Walk);
        targetRotation = (enemyState == EnemyState.TurnAround) ? ThinkOfRotation() : targetRotation;
        if (enemyState == EnemyState.TurnAround)
            StartCoroutine(ChangeToWalkState());
    }

    void ThinkAttackState() { enemyState = EnemyState.Run; }

    IEnumerator ChangeToWalkState()
    {
        while (true)
        {
            float current = transform.rotation.eulerAngles.y;
            float target = targetRotation.eulerAngles.y;
            if (Mathf.RoundToInt(current) == Mathf.RoundToInt(target))
            {
                string mName = GetMethodName(ThinkIdleState);
                int resultIndex = (int)(targetRotation.eulerAngles.y / RotateAngle);
                float delay = (resultIndex % 2 == 0) ? thinkTimeOfIdle : thinkTimeOfIdle * Mathf.Sqrt(2);
                CancelInvoke(mName);
                Invoke(mName, delay);
                enemyState = EnemyState.Walk;
                break;
            }
            yield return null;
        }
    }

    void ControlAnimation()
    {

        Animator animator = GetComponent<Animator>();
        switch (enemyState)
        {
            case EnemyState.Idle:
                animator.SetFloat(Animator.StringToHash("speed"), 1, animationDampTime, Time.deltaTime);
                animator.SetBool(Animator.StringToHash("attack"), false);
                break;
            case EnemyState.TurnAround:
            case EnemyState.Walk:
                animator.SetFloat(Animator.StringToHash("speed"), 2, animationDampTime, Time.deltaTime);
                animator.SetBool(Animator.StringToHash("attack"), false);
                break;
            case EnemyState.Run:
                animator.SetFloat(Animator.StringToHash("speed"), 3, animationDampTime, Time.deltaTime);
                animator.SetBool(Animator.StringToHash("attack"), false);
                break;
            case EnemyState.Attack:
                animator.SetBool(Animator.StringToHash("attack"), true);
                animator.SetFloat(Animator.StringToHash("speed"), 0);
                break;
            case EnemyState.Dead:
                animator.SetBool(Animator.StringToHash("dead"), true);
                animator.SetFloat(Animator.StringToHash("speed"), 0);
                break;
        }
    }

    void ControlPosition()
    {
        CharacterController charaController = GetComponent<CharacterController>();
        switch (enemyState)
        {
            case EnemyState.Walk:
                charaController.SimpleMove(transform.forward * Time.deltaTime * walkSpeed);
                break;
            case EnemyState.Run:
                charaController.SimpleMove(transform.forward * Time.deltaTime * runSpeed);
                break;
        }
    }

    void ControlRotation()
    {
        switch (enemyState)
        {
            case EnemyState.TurnAround:
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                break;
            case EnemyState.Run:
            case EnemyState.Attack:
                Vector3 center = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                transform.LookAt(center);
                break;
        }
    }

    Quaternion ThinkOfRotation()
    {
        RotationMap map = GetComponent<RotationMap>();
        List<int> array = map.GetArrayFromMap();

        int index = array[Random.Range(0, array.Count)];
        map.SetPositionToNext(index);

        Vector3 result = Vector3.zero;
        result.y += index * RotateAngle;
        return Quaternion.Euler(result);
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            enemyState = EnemyState.Dead;
            Destroy(GetComponent<CharacterController>());
            Destroy(gameObject, destroyDelayTime);
            transform.position = new Vector3(transform.position.x, -1, transform.position.z);
        }
    }

}