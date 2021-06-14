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

    public List<GameObject> stars;

    public int Star
    {
        set
        {
            switch (value)
            {
                case 1:
                    stars[1].SetActive(true);
                    stars[0].SetActive(false);
                    stars[2].SetActive(false);
                    break;
                case 2:
                    stars[1].SetActive(true);
                    stars[0].SetActive(false);
                    stars[2].SetActive(true);
                    break;
                case 3:
                    stars[1].SetActive(true);
                    stars[0].SetActive(true);
                    stars[2].SetActive(true);
                    break;
            }
        }
    }

    public Vector2 offset;

    private bool init = false;

    public void Init(PieceController target)
    {
        gameObject.SetActive(true);
        Target = target;
        target.OnHealthUpdate += Target_OnHealthUpdate;
        target.OnDie += Target_OnDie;
        StartCoroutine(FollowTarget());
        init = true;
    }

    private void OnEnable() {
        if (init){
            StartCoroutine(FollowTarget());
        }
    }

    private void OnDestroy()
    {
        if (init)
        {
            Target.OnHealthUpdate -= Target_OnHealthUpdate;
            Target.OnDie -= Target_OnDie;
        }
    }

    private IEnumerator FollowTarget()
    {
        while (true)
        {
            if (Target != null)
            {
                if (Target.GetComponent<PieceController>().Placeable)
                {
                    gameObject.SetActive(false);
                    yield break;
                }
                var pos = Target.transform.position;
                bar.transform.position = new Vector2(pos.x, pos.y) + offset;
                Star = Target.star;
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
