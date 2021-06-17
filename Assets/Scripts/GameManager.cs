using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
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
    public GameObject WinMessage;
    public Text WinMessageText;
    public GameObject LoseMessage;
    public Text LoseMessageText;

    private enum GameState
    {
        Draw,
        Place,
        Battle,
        Settlement
    }

    private GameState state;

    public void Awake()
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
        ++Turn;
        WinMessage.SetActive(true);

        int checkpoint = int.Parse(manager.currentMap.Name);
        var gold = (10 + checkpoint * manager.GetTeam(1)) * 2;
        player.Gold += gold;
        PlayerPrefs.SetString($"{manager.currentMap.Name}Checkpoint", "true");
        // NextMap
        var map = DataManager.Instance.MapLists.FirstOrDefault(e =>
            e.Name == (int.Parse(manager.currentMap.Name) + 1).ToString());
        if (map == null)
        {
            WinMessageText.text = $"恭喜你通关!!";
            Turn = 1;
            Invoke("Back", 3f);
            return;
        }
        else
        {
            WinMessageText.text = $"进入下一个关卡\n能量增加${gold}";
        }
        Invoke("HideMessageImage", 1.5f);
        manager.NextMap(map);
        cardManager.FreeDraw();
    }

    public void Back()
    {
        SceneManager.Instance.BackMenu();
    }

    /// <summary>
    /// handler of loss
    /// </summary>
    public void LossHander(int damage)
    {
        int checkpoint = int.Parse(manager.currentMap.Name);
        state = GameState.Settlement;
        var total_damage = checkpoint + damage * checkpoint * (1 << Mathf.CeilToInt(Mathf.Max(_turn / 3f - 1, 1)));
        player.TakeDamage(total_damage);
        var gold = 10 + (manager.GetTeam(1) - damage) * checkpoint;
        player.Gold += gold;
        ++Turn;

        LoseMessage.SetActive(true);
        if (!player.Alive)
        {
            LoseMessageText.text = $"闯关失败";
            Invoke("Back", 3f);
            return;
        }
        else
        {
            LoseMessageText.text = $"生命值减少{total_damage}\n能量增加{gold}";
            Invoke("HideMessageImage", 1.5f);
        }
        cardManager.ResetCard();
    }

    public void Reset()
    {
        Turn = 0;
    }

    public void HideMessageImage()
    {
        if (WinMessage.gameObject != null && WinMessage.gameObject.activeSelf)
            WinMessage.SetActive(false);
        if (LoseMessage.gameObject != null && LoseMessage.gameObject.activeSelf)
        {
            LoseMessage.SetActive(false);
        }
    }
}
