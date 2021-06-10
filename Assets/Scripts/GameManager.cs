using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameManager :MonoSingleton<GameManager>
{
    // the _turn of game
    private int _turn = 1;

    public int Turn
    {
        get
        {
            return _turn;
        }
        set
        {
            _turn = value;
            if (StageTurnText != null)
            {
                StageTurnText.text = _turn.ToString();
            }
        }
    }

    // the stage of game
    public int stage = 0;

    // the manager of game board
    public GameBoardManager manager;

    // the player 
    public PlayerController player;

    public Text StageTurnText;
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

    override public void Awake()
    {
        base.Awake();
        manager.WinHandler = WinHanler;
        manager.LossHandler = LossHander;
    }

    /// <summary>
    /// handler of the win
    /// </summary>
    public void WinHanler()
    {
        state = GameState.Settlement;
        ++Turn;
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
        player.TakeDamage(Mathf.RoundToInt(damage / 10f * _turn));
        player.Gold += Mathf.FloorToInt(Mathf.Min(damage / 2, 10));
        ++Turn;

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
        Turn = 0;
    }

    public void HideMessageImage()
    {
        message.SetActive(false);
    }
}
