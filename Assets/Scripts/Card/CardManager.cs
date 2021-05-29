using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Dictionary<int, List<GameObject>> CardData;

    public DataManager manager;

    public List<CardBaseController> Cards;

    private void Awake()
    {
        CardData = manager.piecePrefabs;
        Cards = GetComponentsInChildren<CardBaseController>().ToList();
        Debug.Log($"Cards Count:{Cards.Count}", gameObject);
        StartCoroutine("LoadCardData");
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

    IEnumerator LoadCardData()
    {
        while (!CardData.Any() || Cards == null)
        {
            yield return new WaitForSeconds(.3f);
        }
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
        yield break;
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
        if (CardData.ContainsKey(star))
            return CardData[star][Random.Range(0, CardData[star].Count)];
        else
        {
            Debug.LogWarning($"{star} piece not found");
            return null;
        }
    }
}
