using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtils : MonoBehaviour
{
    private GameObject[] SelectsObjects;
    private int SelectMonsterIndex;

    // Start is called before the first frame update
    void Start()
    {
        this.SelectsObjects = new GameObject[3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectRole(GameObject InObj)
    {
        //this.SelectsObjects[0] = InObj;
        //this.ShowSelectSelfMonsterUI();
        //SelectMonsterIndex = 1;

        ATController AT = GameObject.Find("ATController").GetComponent<ATController>();

        GameObject[] Objs = new GameObject[1];
        Objs[0] = InObj;
        AT.SetPlayerCheckObject(Objs);

        AT.StartLevel(1);
    }


    public void SetMonsterSelectIndex(int idnex)
    {
        SelectMonsterIndex = idnex;
    }

    public void ShowSelectSelfMonsterUI()
    {
        showUIMonstershasSelect();
        GameObject.Find("UIRoleSelect").SetActive(false);

        //展示选怪物UI

    }


    public void OnSelectSelfMonster(GameObject Monster)
    {
        this.SelectsObjects[SelectMonsterIndex] = Monster;

        showUIMonstershasSelect();
    }

    private void showUIMonstershasSelect()
    {

    }

    public void OnClickStartLevel()
    {
        ATController AT = GameObject.Find("ATController").GetComponent<ATController>();

        AT.SetPlayerCheckObject(this.SelectsObjects);

        AT.StartLevel(1);
    }

}
