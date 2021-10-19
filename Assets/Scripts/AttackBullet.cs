using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBullet : MonoBehaviour
{

    public AttackSkill Skill;
    public AttackBehaivor Attacker;
    public AttackBehaivor BeAttacker;


    private float _time;
    // Start is called before the first frame update
    void Start()
    {
        _time = 0.0f;
        if(Skill.AttackCalc <= 0)
        {
            checkTimeForAttack();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        flyToTarget();
    }


    private AttackBehaivor[] FindNearByMonster()
    {
        Vector3 pos = BeAttacker.transform.position;
        pos.y = 0;
        float SkillCicrle = Skill.HitCicrle;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Monster");

        ArrayList arrayList = new ArrayList();

        for (int i = 0;i < objects.Length;++i)
        {
            Vector3 now = objects[i].transform.position;
            now.y = 0;
            float dis = (pos - now).magnitude;
            if(dis <= SkillCicrle)
            {
                arrayList.Add(objects[i].GetComponent<AttackBehaivor>());
            }
        }

        return (AttackBehaivor[])arrayList.ToArray(typeof(AttackBehaivor));
    }


    private void checkTimeForAttack()
    {
        if (BeAttacker.IsAlive())
        {
            BeAttacker.gameObject.GetComponent<AnimationController>().BeAttack();

            //计算伤害 
            {
                int Attack = Attacker.Attack_Power;
                float CriticalAttackWeight = Attacker.BeAttack_Weight;
                float SkillAddAttack = Skill.AttackWeight;

                Attack = (Random.Range(0.0f, 1.0f) < CriticalAttackWeight) ? (int)(Attack * 1.5f) : Attack;
                Attack += (int)(Attack * SkillAddAttack);

                if (Skill.HitCicrle > 0)
                {
                    AttackBehaivor[] attcks = FindNearByMonster();
                    for(int i = 0; i < attcks.Length;++i)
                    {
                        Attacker.SetAttackCount(Attack);
                        attcks[i].SetBeAttackCount(Attack);

                        if (!attcks[i].GiveHp(-Attack))
                        {
                            Attacker.TryFindMonster();
                        }
                    }
                }
                else
                {
                    Attacker.SetAttackCount(Attack);
                    BeAttacker.SetBeAttackCount(Attack);

                    if (!BeAttacker.GiveHp(-Attack))
                    {
                        Attacker.TryFindMonster();
                    }
                }
            }

            if(Skill.HitParticle)
            {
                GameObject Hit = GameObject.Instantiate(Skill.HitParticle,BeAttacker.hitTransform,false);
                Destroy(Hit, Skill.HitParticleLifeTime);
            }
        }

        Destroy(gameObject);
    }

    private bool flyToTarget()
    {
        if (BeAttacker != null && BeAttacker.isActiveAndEnabled)
        {
            Vector3 Now = gameObject.transform.position;
            Vector3 End = BeAttacker.hitTransform.position;

            Vector3 Dir = End - Now;


            if (Dir.magnitude <= .1f)
            {
                gameObject.transform.position = End;
                checkTimeForAttack();
            }
            else
            {
                gameObject.transform.Translate(Skill.AttackCalc * Dir.normalized * Time.deltaTime);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        
        return true;
    }
}
