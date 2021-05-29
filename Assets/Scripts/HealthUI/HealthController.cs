using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public PieceController target;


    public RectTransform bar;
    public Slider slider;

    public Vector2 offset;

    public bool init = true;

    private void Start()
    {
        init = false;
    }

    private void OnEnable()
    {
        Debug.Log("enable");
        StartCoroutine("UpdateHealth");
    }

    IEnumerator UpdateHealth()
    {
        slider.value = slider.maxValue;
        while (true)
        {
            while (target && target.Alive)
            {
                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);
                var pos = target.transform.position;
                bar.transform.position = new Vector2(pos.x, pos.y) + offset;
                slider.value = target.Health * 1f / target.maxHealth;
                yield return null;
            }
            if (target && !target.Alive)
                gameObject.SetActive(false);
            if (!init && target == null)
            {
                Destroy(gameObject, .1f);
                yield break;
            }
            yield return null;
        }
    }

}
