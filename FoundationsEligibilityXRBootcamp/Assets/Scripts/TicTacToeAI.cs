using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeIcon{none, cross, circle}
public enum Turn{none, Player, Machine }

/// <summary>
/// Foundations Eligibility Test for XR Bootcamp
/// Author: Raeus Aranguren-Viegas
/// Date: March 5, 2025
///  
/// Tic Tac Toe AI:
/// - Easy AI will check the available spaces on the board and select a random spot from those spaces.
/// - Hard AI will check best first space based on already taken spots, while blocking player moves accordingly.
/// </summary>
public class TicTacToeAI : MonoBehaviour
{
	private int _aiLevel; // 0 = easy | 1 = hard
	public TicTacToeIcon[,] boardState {get; private set;}
	private ClickTrigger[,] _triggers;

	[SerializeField] private GameObject retryButton;
	[SerializeField] private Messages messager;
	[SerializeField] private GameObject _xPrefab;
	[SerializeField] private GameObject _oPrefab;
	[field:SerializeField] public TicTacToeIcon playerIcon {get; private set;}= TicTacToeIcon.circle;
	public TicTacToeIcon AiIcon {get; private set;} = TicTacToeIcon.cross;
	private Turn currentTurn = Turn.Player;
	public bool SpotSelected {get; set;} = false;
	public bool GameComplete  {get; set;} = false;
	public List<AIAlgorithm> aiDifficulties;
	private AIAlgorithm currentAiDifficulty;
	
	[HideInInspector] public UnityEvent OnAiTurn; // to disable player input on ai turn
	[HideInInspector] public UnityEvent OnPlayerTurn; // to enable player input on player turn
	[HideInInspector] public UnityEvent OnGameEnd; // ends player input
	[HideInInspector] public UnityEvent onGameStarted; // starts player input

    private void Start() {
		AiIcon = playerIcon == TicTacToeIcon.cross ? TicTacToeIcon.circle : TicTacToeIcon.cross; // makes the ai use the opposite of the player icon
    }
	private void Update() {
		messager.ShowCurrentTurn(currentTurn);
	}
	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		currentAiDifficulty = _aiLevel == 0 ? aiDifficulties[0] : aiDifficulties[1]; // selects an AI algorithm based on the selected AI Level
		retryButton.SetActive(false);
		StartGame();
	}
	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3]; // init triggers 2D array
		boardState = new TicTacToeIcon[3,3]; // init boardState 2D array
		GameComplete = false;
		currentTurn = Turn.Player;
		onGameStarted.Invoke();
		StartCoroutine(TurnLoop()); // state machine loop
	}
	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger) { // this for all triggers at the beginning
		_triggers[myCoordX, myCoordY] = clickTrigger;
		clickTrigger.messager = messager;
	}
	public void PlayerSelects(int coordX, int coordY) {
		SetVisual(coordX, coordY, playerIcon);
		currentAiDifficulty.PlayerLastSelectedSpace = new Vector2Int(coordX, coordY); // saves player selection to ai algorithm
		SpotSelected = true;
	}
	public void AiSelects(int coordX, int coordY){
		SetVisual(coordX, coordY, AiIcon);
		currentAiDifficulty.AiSelectedSpaces[coordX, coordY] = AiIcon; // saves ai selection to ai algorithm
	}
	// Mini Turn State Machine:
	private IEnumerator TurnLoop() {
		int winner = -2;
		while (!GameComplete) {
			currentTurn = Turn.Player;
			SpotSelected = false;
			OnPlayerTurn.Invoke(); // enables player input
			yield return new WaitUntil(CheckSpotSelected);
			winner = CheckWinner();

			OnAiTurn.Invoke(); // disables player input
			yield return new WaitForSeconds(1f); // delay for visual purposes
			if (GameComplete) break; // early return if game is won ; stops loop

			currentTurn = Turn.Machine;
			yield return new WaitForSeconds(0.5f); 
			StartAI();

			yield return new WaitUntil(CheckSpotSelected);
			yield return new WaitForSeconds(0.5f);

			winner = CheckWinner();
		}
		OnGameEnd.Invoke();
		messager.OnGameEnded(winner);
		retryButton.SetActive(true);
	}
	public int CheckWinner() { // hardcoded row/column/diagonal checks
		for (int x = 0; x < boardState.GetLength(0); x++) {
			// all rows
			if (boardState[x, 0] == playerIcon && boardState[x, 1] == playerIcon && boardState[x, 2] == playerIcon) {
				GameComplete = true;
				return 0;
			}
			if (boardState[x, 0] == AiIcon && boardState[x, 1] == AiIcon && boardState[x, 2] == AiIcon) {
				GameComplete = true;
				return 1;
			}
			// all columns
			if (boardState[0, x] == playerIcon && boardState[1, x] == playerIcon && boardState[2, x] == playerIcon) {
				GameComplete = true;
				return 0;
			}
			if (boardState[0, x] == AiIcon && boardState[1, x] == AiIcon && boardState[2, x] == AiIcon) {
				GameComplete = true;
				return 1;
			}			
		}
		// all diagonals
		return CheckDiagonals();
    }
	private int CheckDiagonals() {
		if (boardState[0, 0] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 2] == playerIcon) {
			GameComplete = true;
			return 0;
		}
		if (boardState[0, 0] == AiIcon && boardState[1, 1] == AiIcon && boardState[2, 2] == AiIcon) {
			GameComplete = true;
			return 1;
		}
		if (boardState[0, 2] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 0] == playerIcon) {
			GameComplete = true;
			return 0;
		}
		if (boardState[0, 2] == AiIcon && boardState[1, 1] == AiIcon && boardState[2, 0] == AiIcon) {
			GameComplete = true;
			return 1;
		}
		return -1;
	}
	private void StartAI() {
		SpotSelected = false;
		currentAiDifficulty.StartAI();
    }
	private bool CheckSpotSelected() {
		return SpotSelected == true;
    }
	private void SetVisual(int coordX, int coordY, TicTacToeIcon targetIcon)
	{
		Instantiate(
			targetIcon == TicTacToeIcon.circle ? _oPrefab : _xPrefab, 
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
		SetBoardState(coordX, coordY, targetIcon); // sets board and disables trigger click privilages for that space
		messager.UpdateMiniBoard(boardState); // updates mini board everytime a selection is made
	}
	private void SetBoardState(int coordX, int coordY, TicTacToeIcon targetIcon) {
		boardState[coordX, coordY] = targetIcon;
		_triggers[coordX, coordY].canClick = false;
	}
}
