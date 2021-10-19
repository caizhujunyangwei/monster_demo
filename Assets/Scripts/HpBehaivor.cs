using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBehaivor : MonoBehaviour
{

    [Tooltip("血条调整")]
    public Vector2 offset;

    GameObject UIObject;

    RectTransform transform;

    Slider slider;

    AttackBehaivor Behaivor;

    // Start is called before the first frame update
    void Start()
    {
        Behaivor = gameObject.GetComponent<AttackBehaivor>();
        UIObject = gameObject.transform.Find("Hp").gameObject;

        slider = UIObject.transform.Find("HpSlider").gameObject.GetComponent<Slider>();

        transform = slider.gameObject.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        if(!Behaivor.IsAlive())
        {
            Destroy(UIObject);
            Destroy(this);
            return;
        }
        slider.value = Behaivor.GetHpPercent();
        transform.transform.position = offset + RectTransformUtility.WorldToScreenPoint(Camera.main, gameObject.transform.position);
    }
}
