using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    public GameBoardManager manager;

    public PlayerController player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (manager != null)
        {
            var controller = other.GetComponent<PieceController>();
            if (controller != null)
            {
                if (controller.draggable)
                {
                    manager._piecesList.Remove(controller);
                    manager.map.PutPiece(Vector3Int.RoundToInt(controller.OriginPos - controller.offset), null);
                    player.Gold += controller.cost / 2;
                    Destroy(controller.gameObject);
                }
            }
        }
    }
}
