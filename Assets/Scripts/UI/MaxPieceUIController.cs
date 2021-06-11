using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxPieceUIController : MonoBehaviour
{
    [SerializeField]
    private Text _currentPiece;

    [SerializeField]
    private Text _maxPiece;

    [SerializeField]
    private MapEditor editor;

    public void OnEnable()
    {
        StartCoroutine(UpdatePiece());
    }

    public int CurrentPiece
    {
        get
        {
            return int.Parse(_currentPiece.text);
        }
        set
        {
            _currentPiece.text = value.ToString();
        }
    }

    public int MaxPiece
    {
        get
        {
            return int.Parse(_maxPiece.text);
        }
        set
        {
            _maxPiece.text = value.ToString();
        }
    }

    IEnumerator UpdatePiece()
    {
        while (true)
        {
            if (editor != null)
            {
                MaxPiece = editor.maxPieces;
                CurrentPiece = editor.GetPieceCount();
            }
            else
            {
                Debug.Log("MapEditor not found");
            }
            yield return new WaitForSeconds(1);
        }
    }
}
