using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject StageTurnText;
    public GameObject message;
    public GameObject messageText;

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
       
        StageTurnText.GetComponent<Text>().text = stage+ "-" + turn;
        message.SetActive(true);
        messageText.GetComponent<Text>().text = "WIN";
        Invoke("HideMessageImage", 1f);
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

        StageTurnText.GetComponent<Text>().text = stage + "-" + turn;
        message.SetActive(true);
        messageText.GetComponent<Text>().text = "LOSE";
        Invoke("HideMessageImage", 1f);
        if (!player.Alive)
        {
            // end of game
        }
    }

    public void Reset()
    {
        turn = 0;
    }
    public void HideMessageImage()
    {
        message.SetActive(false);
    }
}
