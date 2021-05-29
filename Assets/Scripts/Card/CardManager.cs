using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardBaseController> Cards;

    private void Awake()
    {
        Cards = GetComponentsInChildren<CardBaseController>().ToList();
        Debug.Log($"Cards Count:{Cards.Count}", gameObject);
        LoadCardData();
    }

    public void ResetCard()
    {
        foreach (var card in Cards)
        {
            if (card.cardItem && card.cardItem.transform.parent == card.transform)
            {
                Destroy(card.cardItem);
            }
            var cardItem = GetRandomCard(1);
            var item = Instantiate(cardItem);
            item.transform.localScale = new Vector3(.3f, .3f, 1);
            item.transform.parent = card.gameObject.transform;
            item.transform.position = card.transform.position;
            ChangeLayer(item.transform, 3);
            var controller = item.GetComponent<PieceController>();
            controller.placeable = true;
            controller.SetOriginPosition();
            controller.Reset();
            card.cardItem = item;
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
            ChangeLayer(item.transform, 3);
            var controller = item.GetComponent<PieceController>();
            controller.placeable = true;
            controller.SetOriginPosition();
            controller.Reset();
            card.cardItem = item;
        }
    }

    private void ChangeLayer(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        var children = obj.GetComponentsInChildren<Transform>(true);
        foreach (var trans in children)
        {
            trans.gameObject.layer = 3;
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
            Debug.LogWarning($"{star} piece not found");
            return null;
        }
    }
}
