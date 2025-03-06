using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// This was the EndMessage class. Renamed to properly fit its purpose.
/// </summary>
public class Messages : MonoBehaviour
{

	[SerializeField] private TMP_Text miniBoard = null;
	[SerializeField] private TMP_Text _playerMessage = null;
	[SerializeField] private TMP_Text _turnText = null;
	[SerializeField] private Color normalColor = Color.yellow;
	[SerializeField] private Color winColor = Color.green;
	[SerializeField] private Color loseColor = Color.red;
	[SerializeField] private Color tieColor = Color.grey;

    private void Start() {
		miniBoard.gameObject.SetActive(true);
		_playerMessage.gameObject.SetActive(true);
		_turnText.gameObject.SetActive(true);

		ShowMessage("", normalColor);
		InitMiniBoard();
    }
    public void OnGameEnded(int winner)
	{
		//_playerMessage.text = winner == -1 ? "Tie" : winner == 1 ? "AI wins" : "Player wins";
		if (winner == -1) {
			ShowMessage("Tie", tieColor);
			miniBoard.color = tieColor;
        } else if (winner == 1) {
			ShowMessage("Machine Wins", loseColor);
			miniBoard.color = loseColor;
        } else if (winner == -2) {
			ShowMessage("No Win", normalColor);
			miniBoard.color = normalColor;
		}
		else {
			ShowMessage("Player Wins", winColor);
			miniBoard.color = winColor;
		}
	}
	public void ShowCurrentTurn(Turn currTurn) {
		_turnText.text = currTurn + "'s Turn";
    }
	public void ShowMessage(string message, Color color) {
		_playerMessage.color = color;
		_playerMessage.text = message;
	}
	public void ShowSpotTakenMessage() {
		StartCoroutine(SpotTakenMessage());
    }
	private IEnumerator SpotTakenMessage() {
		ShowMessage("Spot Taken! Try Another.", normalColor);
		yield return new WaitForSeconds(1f);
		ShowMessage("", normalColor);
	}
	private void InitMiniBoard() {
		miniBoard.text = "[_][_][_]\n[_][_][_]\n[_][_][_]";
	}
	public void UpdateMiniBoard(TicTacToeIcon[,] boardState) {
		miniBoard.text = "";
		for (int x = 0; x < boardState.GetLength(0); x++) {
				for (int y = 0; y < boardState.GetLength(1); y++) {
					miniBoard.text += "[";
					if (boardState[x, y] == TicTacToeIcon.none) {
						miniBoard.text += "_";
					} else {
						miniBoard.text += GetIconSymbol(boardState[x, y]);
					}
					miniBoard.text += "]";	
				}
			miniBoard.text += "\n";
		}
	}
	private string GetIconSymbol(TicTacToeIcon icon) {
		return icon == TicTacToeIcon.cross ? "x" : "o";
	}
}
