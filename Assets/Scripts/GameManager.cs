using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // the turn of game
    public int turn = 1;

    // the stage of game
    public int stage = 0;

    // the manager of game board
    public GameBoardManager manager;

    // the player 
    public PlayerController player;


    private enum GameState
    {
        Draw,
        Place,
        Battle,
        Settlement
    }

    private GameState state;

    void Awake()
    {
        manager.WinHandler = WinHanler;
        manager.LossHandler = LossHander;
    }

    /// <summary>
    /// handler of the win
    /// </summary>
    public void WinHanler()
    {
        state = GameState.Settlement;
        ++turn;
    }

    /// <summary>
    /// handler of loss
    /// </summary>
    public void LossHander(int damage)
    {
        state = GameState.Settlement;
        player.TakeDamage(Mathf.RoundToInt(damage / 10f * turn));
        player.Gold += Mathf.FloorToInt(Mathf.Min(damage / 2, 10));
        ++turn;
        if (!player.Alive)
        {
            // end of game
        }
    }

    public void Reset()
    {
        turn = 0;
    }

}
