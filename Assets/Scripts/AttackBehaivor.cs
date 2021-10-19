using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Camp
{
    Normal = 0,
    Monster = 1,
}

[System.Serializable]
public struct AttackSkill
{
    [Range(0, 1), Tooltip("技能释放概率")]
    public float AttackWeight;

    [Tooltip("弹道速度 0表示瞬发")]
    public float AttackCalc;

    [Tooltip("动画Trigger名称")]
    public string AttackTriggerName;

    [Tooltip("攻击力加成")]
    public float AttackCritical;

    [Tooltip("CD")]
    public float SkillCD;

    [Tooltip("技能施法距离")]
    public float Attack_Distance;

    [Tooltip("弹道模块")]
    public GameObject AttackBullet;

    [Tooltip("命中特效")]
    public GameObject HitParticle;

    [Tooltip("命中特效")]
    public float HitParticleLifeTime;

    [Tooltip("命中范围半径 0表示单体攻击")]
    public float HitCicrle;

    [HideInInspector]
    public float SkillTime;
}


public class AttackBehaivor : MonoBehaviour
{
    [Tooltip("技能表,普攻放最后")]
    public AttackSkill []Attacks;

    [Tooltip("基础攻击力")]
    public int Attack_Power = 2;

    [Tooltip("暴击概率")]
    public float CriticalAttack_Weight = 0.1f;

    [Tooltip("阵营")]
    public Camp CampStand = Camp.Normal;

    [Range(0,1),Tooltip("被攻击权重，权重越高，越先被攻击")]
    public float BeAttack_Weight = 0.1f;

    [Tooltip("移动速度")]
    public float MoveSpeed = 20.0f;

    [Tooltip("攻击间隔")]
    public float HitInterval = 3.0f;
    private float LastHitTime = 0.0f;

    [Tooltip("血量")]
    public int HP = 100;
    private int CurrentHp;

    [Tooltip("被攻击的位置坐标")]
    public Transform hitTransform;

    [Tooltip("发射弹道的位置坐标")]
    public Transform fireTransform;

    [Tooltip("死亡时间消融时间")]
    public float DieTime;

   

    private AnimationController animationController;

    private ATController aTController;

    private AttackSkill SkillFire;
    private AttackBehaivor AttackTarget;
    private bool bMovingAndAttack = false;


    private int TotalAttack = 0;
    private int TotalBeAttack = 0;

    private bool bStart = false;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHp = HP;
        this.gameObject.tag = "Monster";
        animationController = this.gameObject.GetComponent<AnimationController>();

        aTController = GameObject.Find("ATController").GetComponent<ATController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bStart && aTController.IsStart())
        {
            if (IsAlive())
            {
                LastHitTime -= Time.deltaTime;
                for (int i = 0; i < Attacks.Length; ++i)
                {
                    if (Attacks[i].SkillTime > 0)
                    {
                        Attacks[i].SkillTime -= Time.deltaTime;
                    }
                }

                if (!AttackTarget || !AttackTarget.IsAlive())
                {
                    bMovingAndAttack = false;
                }

                if (bMovingAndAttack)
                {
                    attackOther(SkillFire, AttackTarget);
                }
                else
                {
                    if (LastHitTime <= 0.0f)
                    {
                        doAttack();
                    }
                }
            }
            else
            {
                DieTime -= Time.deltaTime;
                if (DieTime <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void MarkStart(bool InStart)
    {
        bStart = InStart;
    }

    public bool IsAlive()
    {
        return CurrentHp > 0;
    }

    public void SetAttackCount(int attack)
    {
        TotalAttack += attack;
    }
    public int GetAttackCount()
    {
        return TotalAttack;
    }

    public void SetBeAttackCount(int attack)
    {
        TotalBeAttack += attack;
    }
    public int GetBeAttackCount()
    {
        return TotalBeAttack;
    }

    public bool GiveHp(int hp)
    {
        if (IsAlive())
        {
            CurrentHp += hp;

            CurrentHp = Mathf.Clamp(CurrentHp, 0, HP);

            if (CurrentHp <= 0)
            {
                animationController.Die();
                return false;
            }
            return true;
        }

        return false;
    }

    public float GetHpPercent()
    {
        return (CurrentHp * 1.0f) / HP;
    }

    private bool moveAndAttack()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(AttackTarget.transform.position - gameObject.transform.position);

        Vector3 dir = AttackTarget.transform.position - gameObject.transform.position;
        dir.y = 0;
        float distance = dir.magnitude;

        if(distance > SkillFire.Attack_Distance)
        {
            gameObject.transform.position += dir.normalized * MoveSpeed * Time.deltaTime;
            animationController.Move(true);
            return true;

        }
        animationController.Move(false);
        return false;
    }

    public AttackBehaivor TryFindMonster()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Monster");

        AttackBehaivor FindObject = null;
        float weight = 0.0f;
        for(int i = 0; i< objects.Length;++i)
        {
            AttackBehaivor Temp = objects[i].gameObject.GetComponent<AttackBehaivor>();

            if(!Temp || Temp.gameObject == gameObject || Temp.CampStand == CampStand || !Temp.IsAlive())
            {
                continue;
            }

            if (FindObject == null)
            {
                FindObject = Temp;
                weight = FindObject.BeAttack_Weight;
            }
            else
            {
                if(Temp.BeAttack_Weight > weight)
                {
                    FindObject = Temp;
                    weight = FindObject.BeAttack_Weight;
                }
                else if(Temp.BeAttack_Weight == weight && Random.Range(0.0f,1.0f) >= 0.5f)
                {
                    FindObject = Temp;
                    weight = FindObject.BeAttack_Weight;
                }
            }
        }

        if(FindObject == null)
        {
           aTController.LevelFinish(CampStand == Camp.Normal);
        }

        return FindObject;
    }

    private void doAttack()
    {
        AttackBehaivor beAttacker = TryFindMonster();
        if (beAttacker && beAttacker != this)
        {
            float weights = 0.0f;
            for(int i = 0;i < Attacks.Length;++i)
            {
                AttackSkill skill = Attacks[i];
                if (skill.SkillTime <= 0)
                {
                    weights += skill.AttackWeight;
                }
            }
            float t = Random.Range(0.0f, weights);
            weights = 0.0f;
            for (int i = 0; i < Attacks.Length; ++i)
            {
                AttackSkill skill = Attacks[i];
                if (skill.SkillTime <= 0)
                {
                    if (t <= skill.AttackWeight + weights)
                    {
                        attackOther(skill, beAttacker);
                        break;
                    }
                    else
                    {
                        weights += skill.AttackWeight;
                    }
                }
            }
        }
    }


    private void SetCD(AttackSkill skill)
    {
        for(int i = 0;i < Attacks.Length;++i)
        {
            if (skill.Equals(Attacks[i]))
            {
                Attacks[i].SkillTime = Attacks[i].SkillCD;
                break;
            }
        }
    }

    private void attackOther(AttackSkill skill, AttackBehaivor other)
    {
        SkillFire = skill;
        AttackTarget = other;

        bMovingAndAttack = moveAndAttack();

        if(!bMovingAndAttack)
        {
            SetCD(SkillFire);
            LastHitTime = HitInterval;
            animationController.Attack(SkillFire.AttackTriggerName);

            GameObject Bullet = GameObject.Instantiate(SkillFire.AttackBullet);
            Bullet.transform.position = hitTransform.position;
            AttackBullet attack = Bullet.AddComponent<AttackBullet>();
            attack.Skill = SkillFire;
            attack.BeAttacker = other;
            attack.Attacker = this;
        }
        
    }


}
