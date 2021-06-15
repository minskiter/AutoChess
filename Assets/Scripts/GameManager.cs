using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public CardManager cardManager;

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
        int checkpoint = int.Parse(manager.currentMap.Name);
        player.Gold += (10 + checkpoint * manager.GetTeam(1)) * 2;
        // NextMap
        var map = DataManager.Instance.MapLists.FirstOrDefault(e =>
            e.Name == (int.Parse(manager.currentMap.Name) + 1).ToString());
        if (map == null)
        {
            SceneManager.Instance.BackMenu();
            return;
        }
        manager.NextMap(map);
        cardManager.ResetCard();
    }

    /// <summary>
    /// handler of loss
    /// </summary>
    public void LossHander(int damage)
    {
        int checkpoint = int.Parse(manager.currentMap.Name);
        state = GameState.Settlement;
        player.TakeDamage(checkpoint+damage*checkpoint*(1<<(_turn-1)));
        player.Gold += 10 + (manager.GetTeam(1)-damage) * checkpoint; 
        ++Turn;
        message.SetActive(true);
        messageText.GetComponent<Text>().text = "LOSE";
        Invoke("HideMessageImage", 1f);
        if (!player.Alive)
        {
            // end of game
        }
        cardManager.ResetCard();
    }

    public void Reset()
    {
        Turn = 0;
    }

    public void HideMessageImage()
    {
        if (message.gameObject!=null &&  message.gameObject.activeSelf) 
            message.SetActive(false);
    }
}
