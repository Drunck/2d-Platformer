using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public Transform damagePopUpPref;

    public float moveYSpeed = 10f, textDisappearSpeed = 3f, textDisappearTimer = 1f;

    private TextMeshPro textMesh;
    private Color textColor;
    private const float DISAPPEAR_TIMER_MAX = 1f;
    private static int sortingOrder;

    public DamagePopUp Create(float damage, Vector3 position)
    {
        Transform damagePopUpTransform = Instantiate(damagePopUpPref, position, Quaternion.identity);

        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetUp(damage);

        return damagePopUp;
    }

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void SetUp(float damage)
    {
        int dmg = (int)damage;
        textMesh.SetText(dmg.ToString());
        textColor = textMesh.color;
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        if(textDisappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            transform.localScale += Vector3.one * 1f * Time.deltaTime;
        } 
        else
        {
            transform.localScale -= Vector3.one * 1f * Time.deltaTime;
        }

        textDisappearTimer -= Time.deltaTime;
        if(textDisappearTimer < 0)
        {
            textColor.a -= textDisappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
