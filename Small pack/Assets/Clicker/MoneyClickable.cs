using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyClickable : MonoBehaviour
{
    public float rotateSpeed = 2;
    public float scaleAdder = 1;
    public float scaleReduceSpeed = 1;
    public float baseScale;
    Coroutine lastScaleUp;
    private void Awake()
    {
        baseScale = transform.localScale.x;
        StartCoroutine(Rotate());
    }
    IEnumerator Rotate()
    {
        while (true)
        {
            yield return null;
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }
    public void OnMouseDown()
    {
        ClickerManager.instance.Click();
        if (lastScaleUp != null) StopCoroutine(lastScaleUp);
        lastScaleUp = StartCoroutine(ScaleUp());
    }
    IEnumerator ScaleUp()
    {
        transform.localScale = Vector3.one * baseScale + Vector3.one * scaleAdder;
        while(transform.localScale.x > baseScale)
        {
            transform.localScale -= Vector3.one * scaleReduceSpeed * Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * baseScale;
    }
}
