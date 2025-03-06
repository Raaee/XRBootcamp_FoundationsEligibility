using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	TicTacToeAI _ai;
	public EndMessage messager { get; set; }
	[SerializeField] private int _myCoordX = 0;
	[SerializeField] private int _myCoordY = 0;
	[field: SerializeField] public bool canClick { get; set; }
	private bool aiTurn = false;

	private void Awake() {
		_ai = FindObjectOfType<TicTacToeAI>();
	}
	private void Start() { 
		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEnabled(true));
		_ai.OnGameEnd.AddListener(() => SetInputEnabled(false));

		_ai.OnPlayerTurn.AddListener(() => IsAiTurn(false));
		_ai.OnAiTurn.AddListener(() => IsAiTurn(true));
	}

	private void SetInputEnabled(bool enabled){
		canClick = enabled;
	}
	private void IsAiTurn(bool isTurn) {
		aiTurn = isTurn;
    }

	private void AddReference() // adds ref of all possible plays positions to the trigger board at the beginning:
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
		canClick = true;
	}

	private void OnMouseDown()
	{
		if (aiTurn) return;
		if(!canClick){
			messager?.ShowSpotTakenMessage();
			return;
		}
		_ai.PlayerSelects(_myCoordX, _myCoordY);
		SetInputEnabled(false);
	}
	
}
