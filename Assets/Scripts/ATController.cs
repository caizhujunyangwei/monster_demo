using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ATController : MonoBehaviour
{

    [Tooltip("怪物关卡")]
    public GameObject[] MonsterLevel;

    [Tooltip("主角选择关卡")]
    public GameObject RoleSelect;

    [Tooltip("小结算面板")]
    public GameObject UIObject;

    private GameObject _PlayerSpawnObject;

    private GameObject[] _PlayerObjects;

    private GameObject _SpawnMonsterLevel = null;

    private int _Level = 0;

    private float _StartTime;
    private bool _bFinishLevel;
    private float _StartCount;

    // Start is called before the first frame update
    void Start()
    {
        _PlayerSpawnObject = GameObject.Find("SelfNode");


        ShowRoleSelect();
        _StartCount = 0;
    }

    //private void Update()
    //{
    //    if (_StartCount > 0)
    //    {
    //        _StartCount -= Time.deltaTime;
    //    }
    //}

    public bool IsStart()
    {
        return _StartCount <= 0;
    }

    public void SetPlayerCheckObject(GameObject []Objects)
    {
        _PlayerObjects = Objects;
    }
    

    public void ShowRoleSelect(bool bShow = true)
    {
        GameObject roleSelect = GameObject.Find("RoleSelect");
        if(roleSelect)
        {
            Destroy(roleSelect);

            GameObject.Find("UIRoleSelect").SetActive(false);
        }

        if (bShow)
        {
            roleSelect = Instantiate(RoleSelect);
            roleSelect.name = "RoleSelect";
            GameObject.Find("UIRoleSelect").SetActive(true);
        }

        UIObject.SetActive(false);
    }

    public void StartLevel(int Level)
    {
        //_StartCount = 2;
        ShowRoleSelect(false);
        _bFinishLevel = false;

        if (_SpawnMonsterLevel != null)
        {
            GameObject.DestroyImmediate(_SpawnMonsterLevel);
        }

        _Level = Level;

        int count = _PlayerSpawnObject.transform.childCount;
        for(int i = 0; i < count;++i)
        {
            Destroy(_PlayerSpawnObject.transform.GetChild(i).gameObject);
        }


        GameObject monsterLevel = MonsterLevel[( _Level - 1 )% MonsterLevel.Length];
        if(monsterLevel != null)
        {
            _SpawnMonsterLevel = Instantiate(monsterLevel);

            for(int i = 0;i < _SpawnMonsterLevel.transform.childCount; ++i)
            {
                _SpawnMonsterLevel.transform.GetChild(i).GetComponent<AttackBehaivor>().MarkStart(true);
            }
        }


        for(int i = 0;i < _PlayerObjects.Length;++i)
        {
            GameObject obj = Instantiate(_PlayerObjects[i], _PlayerSpawnObject.transform, false);
            obj.transform.localPosition = new Vector3(-25 * (i - 1), 0, 0);
            obj.GetComponent<AttackBehaivor>().CampStand = Camp.Normal;
            obj.GetComponent<AttackBehaivor>().MarkStart(true);
            
            obj.name = "Player__" + i;

        }

        _StartTime = Time.realtimeSinceStartup;
      
    }

    public void NextLevel()
    {
        StartLevel(_Level + 1);
    }


    public void LevelFinish(bool bWin)
    {
        if(_bFinishLevel)
        {
            return;
        }

        _bFinishLevel = true;

        UIObject.SetActive(true);


        int TotalAttack = 0;
        int TotalBeAttack = 0;

        for (int i = 0; i < _PlayerObjects.Length; ++i)
        {
            AttackBehaivor player = GameObject.Find("Player__" + i).GetComponent<AttackBehaivor>();
            TotalAttack += player.GetAttackCount();
            TotalBeAttack += player.GetBeAttackCount();
        }

        float EndTime = Time.realtimeSinceStartup;

        Transform tra = UIObject.transform.Find("Attack");
        UIObject.transform.Find("Attack").gameObject.GetComponent<Text>().text = "伤害: " + TotalAttack;
        UIObject.transform.Find("BeAttack").gameObject.GetComponent<Text>().text = "承伤: " + TotalBeAttack;
        UIObject.transform.Find("Time").gameObject.GetComponent<Text>().text = "耗时: " + (EndTime - _StartTime).ToString("f2") + " 秒";

        int count = _PlayerSpawnObject.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Destroy(_PlayerSpawnObject.transform.GetChild(i).gameObject);
        }

        if (_SpawnMonsterLevel != null)
        {
            Destroy(_SpawnMonsterLevel);
            _SpawnMonsterLevel = null;
        }
    }
}
