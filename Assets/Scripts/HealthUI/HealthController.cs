using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public PieceController Target;


    public RectTransform bar;
    public Slider slider;

    public Vector2 offset;

    public void Init(PieceController target)
    {
        gameObject.SetActive(true);
        Target = target;
        target.OnHealthUpdate += Target_OnHealthUpdate;
        target.OnDie += Target_OnDie;
        StartCoroutine(FollowTarget());
    }

    private IEnumerator FollowTarget()
    {
        while (true)
        {
            if (Target != null)
            {
                var pos = Target.transform.position;
                bar.transform.position = new Vector2(pos.x, pos.y) + offset;
            }
            yield return null;
        }
    }

    private void Target_OnDie()
    {
        gameObject.SetActive(false);
    }

    private void Target_OnHealthUpdate(int preValue, int value)
    {
        slider.value = value * 1f / Target.maxHealth;
    }
}
