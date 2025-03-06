using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMessage : MonoBehaviour
{

	[SerializeField] private TMP_Text _playerMessage = null;
	[SerializeField] private TMP_Text _turnText = null;
	[SerializeField] private Color normalColor = Color.yellow;
	[SerializeField] private Color winColor = Color.green;
	[SerializeField] private Color loseColor = Color.red;
	[SerializeField] private Color tieColor = Color.grey;

    private void Start() {
		ShowMessage("", normalColor);
    }
    public void OnGameEnded(int winner)
	{
		//_playerMessage.text = winner == -1 ? "Tie" : winner == 1 ? "AI wins" : "Player wins";
		if (winner == -1) {
			ShowMessage("Tie", tieColor);
        } else if (winner == 1) {
			ShowMessage("Machine Wins", loseColor);
        } else if (winner == -2) {
			ShowMessage("No Win", normalColor);
		}
		else {
			ShowMessage("Player Wins", winColor);
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
}
