using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeIcon{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}
/// <summary>
/// Foundations Eligibility Test for XR Bootcamp
/// March 5, 2025 | Raeus Aranguren-Viegas
/// </summary>
public class TicTacToeAI : MonoBehaviour
{
	int _aiLevel;
	TicTacToeIcon[,] boardState;
	[SerializeField] private bool _isPlayerTurn;
	[SerializeField] private int _gridSize = 3;
	[SerializeField] private GameObject _xPrefab;
	[SerializeField] private GameObject _oPrefab;
	[SerializeField] private TicTacToeIcon playerState = TicTacToeIcon.cross;
	private TicTacToeIcon aiState = TicTacToeIcon.circle;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY){
		SetVisual(coordX, coordY, playerState);
	}
	public void AiSelects(int coordX, int coordY){
		SetVisual(coordX, coordY, aiState);
	}
	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger) {
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}
	private void SetVisual(int coordX, int coordY, TicTacToeIcon targetState)
	{
		Instantiate(
			targetState == TicTacToeIcon.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}
}
