using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardBaseController> Cards;

    public PlayerController player;

    public int cost;

    private void Awake()
    {
        Cards = GetComponentsInChildren<CardBaseController>().ToList();
        Debug.Log($"Cards Count:{Cards.Count}", gameObject);
        LoadCardData();
    }

    public void ResetCard()
    {
        if (player.SpendMoney(cost))
        {
            foreach (var card in Cards)
            {
                if (card.cardItem && card.cardItem.transform.parent == card.transform)
                {
                    Destroy(card.cardItem);
                }
                var ui = card.GetComponent<CardBaseController>().ui;
                if (ui != null )
                {
                    Animator ui_Animator = ui.GetComponent<Animator>();
                    if (ui_Animator.GetCurrentAnimatorStateInfo(0).IsName("FrontToBack"))
                    {
                        ui_Animator.SetTrigger("BackToFront");
                    }
                }
                var cardItem = GetRandomCard(1);
                var item = Instantiate(cardItem);
                item.transform.localScale = new Vector3(.3f, .3f, 1);
                item.transform.parent = card.gameObject.transform;
                item.transform.position = card.transform.position;
                LayerTool.ChangeLayer(item.transform, 3);
                var controller = item.GetComponent<PieceController>();
                controller.placeable = true;
                controller.OriginPos = item.transform.position;
                controller.Reset();
                card.cardItem = item;
            }
        }
    }

    public void LoadCardData()
    {
        foreach (var card in Cards)
        {
            var cardItem = GetRandomCard(1);
            var item = Instantiate(cardItem);
            item.transform.localScale = new Vector3(.3f, .3f, 1);
            item.transform.parent = card.gameObject.transform;
            item.transform.position = card.transform.position;
            LayerTool.ChangeLayer(item.transform, 3);
            var controller = item.GetComponent<PieceController>();
            controller.placeable = true;
            controller.OriginPos = item.transform.position;
            controller.Reset();
            card.cardItem = item;
        }
    }

    public void SetPlaceable(bool flag = false)
    {
        foreach (var card in Cards)
        {
            if (card.cardItem != null)
            {
                card.cardItem.GetComponent<PieceController>().placeable = flag;
            }
        }
    }

    GameObject GetRandomCard(int star)
    {
        var cardData = DataManager.Instance.piecePrefabs;
        if (cardData.ContainsKey(star))
        {
            return cardData[star][Random.Range(0, cardData[star].Count)];
        }
        else
        {
            Debug.LogWarning($"star {star} of piece not found");
            return null;
        }
    }
}
