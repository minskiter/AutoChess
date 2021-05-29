using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    public GameBoardManager manager;

    void OnTriggerEnter2D(Collider2D other){    
        if (manager!=null){
            var controller = other.GetComponent<PieceController>();
            manager._piecesList.Remove(controller);
            manager.map.PutPiece(Vector3Int.RoundToInt(controller.OriginPos-controller.offset),null);
            Destroy(controller.gameObject);
        }
    }
}
