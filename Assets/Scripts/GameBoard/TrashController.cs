using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){    
        var manager =  other.gameObject.GetComponentInParent<Grid>()?.GetComponent<GameBoardManager>();
        if (manager!=null){
            var controller = other.GetComponent<PieceController>();
            manager.map.PutPiece(Vector3Int.RoundToInt(controller.OriginPos-controller.offset),null);
            Destroy(controller.gameObject);
        }
    }
}
