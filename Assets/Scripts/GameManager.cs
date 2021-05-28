using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // the turn of game
    public int turn = 0;

    // the stage of game
    public int stage = 0;

    // the manager of game board
    private GameBoardManager manager;

    // the player 
    private PlayerController player;

    private enum GameState{
        Draw,
        Place,
        Battle,
        Settlement
    }

    private GameState state;

    void Awake() {
        Debug.Log("Awake",gameObject);
        manager = GameObject.Find("GameBoard").GetComponent<GameBoardManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        manager.WinHandler = WinHanler;
        manager.LossHandler = LossHander;
    }

    /// <summary>
    /// handler of the win
    /// </summary>
    public void WinHanler(){
        state = GameState.Settlement;
    }

    /// <summary>
    /// handler of loss
    /// </summary>
    public void LossHander(){
        state = GameState.Settlement;
        player.TakeDamage(5);
        if (!player.Alive){
            // end of game
        }
    }

    public void Reset(){
        turn = 0;
    }
  
}
