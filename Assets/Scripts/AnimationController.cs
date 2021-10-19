using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EAnimaStatus
{
    Idle = 0,
    Die = 1,
    Move = 2
}

public class AnimationController : MonoBehaviour
{

    [Tooltip("免疫受击动画概率"), Range(0, 1)]
    public float IgnoreHitAnimation = .15f;

    private Animator _Animator = null;

    private EAnimaStatus _AnimStatus;

    // Start is called before the first frame update
    void Start()
    {
        _Animator = this.gameObject.GetComponent<Animator>();

        _AnimStatus = EAnimaStatus.Idle;

    }

    private bool isIdle()
    {
        return _AnimStatus == EAnimaStatus.Idle;
    }

    private bool isDie()
    {
        return _AnimStatus == EAnimaStatus.Die;
    }

    private bool isMoving()
    {
        return _AnimStatus == EAnimaStatus.Move;
    }

    public void Attack(string name)
    {
        if (!isDie() && isIdle())
        {
            _Animator.SetTrigger(name);
        }
    }

    public void BeAttack()
    {
        if (!isDie())
        {
            if (Random.Range(0.0f, 1.0f) > IgnoreHitAnimation)
            {
                _Animator.SetTrigger("BeAttack");
            }
        }
    }

    public void Move(bool bMoving)
    {
        if (!isDie())
        {
            if (_AnimStatus != (bMoving ? EAnimaStatus.Move : EAnimaStatus.Idle))
            {
                _Animator.SetBool("Move", bMoving);
                _AnimStatus = bMoving ? EAnimaStatus.Move : EAnimaStatus.Idle;
            }
        }
    }

    public void Die()
    {
        if (!isDie())
        {
            _Animator.SetTrigger("Die");
            _AnimStatus = EAnimaStatus.Die;
        }
    }
}
