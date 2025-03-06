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
/// - Hard AI will
/// </summary>
public class TicTacToeAI : MonoBehaviour
{
	private int _aiLevel; // 0 = easy | 1 = hard
	public TicTacToeIcon[,] boardState {get; private set;}
	private ClickTrigger[,] _triggers;
	[SerializeField] private Messages messager;
	[SerializeField] private GameObject _xPrefab;
	[SerializeField] private GameObject _oPrefab;
	[field:SerializeField] public TicTacToeIcon playerIcon {get; private set;}= TicTacToeIcon.circle;
	public TicTacToeIcon AiIcon {get; private set;} = TicTacToeIcon.cross;
	private Turn currentTurn = Turn.Player;
	public bool SpotSelected {get; set;} = false;
	public bool GameWon  {get; set;} = false;
	[SerializeField] private GameObject retryButton;
	public List<AIAlgorithm> aiDifficulties;
	private AIAlgorithm currentAiDifficulty;
	
	[Header("EVENTS")]
	[HideInInspector] public UnityEvent OnAiTurn;
	[HideInInspector] public UnityEvent OnPlayerTurn;
	[HideInInspector] public UnityEvent OnGameEnd;
	public UnityEvent onGameStarted;

    private void Start() {
		AiIcon = playerIcon == TicTacToeIcon.cross ? TicTacToeIcon.circle : TicTacToeIcon.cross; // makes the ai use the opposite of the player icon
    }
	private void Update() {
		messager.ShowCurrentTurn(currentTurn);
	}
	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		currentAiDifficulty = _aiLevel == 0 ? aiDifficulties[0] : aiDifficulties[1];
		retryButton.SetActive(false);
		StartGame();
	}
	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3]; // init triggers 2D array
		boardState = new TicTacToeIcon[3,3]; // init boardState 2D array
		GameWon = false;
		currentTurn = Turn.Player;
		onGameStarted.Invoke();
		StartCoroutine(TurnLoop());
	}
	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger) { // this for all triggers at the beginning
		_triggers[myCoordX, myCoordY] = clickTrigger;
		clickTrigger.messager = messager;
	}
	public void PlayerSelects(int coordX, int coordY) {
		SetVisual(coordX, coordY, playerIcon);
		currentAiDifficulty.PlayerLastSelectedSpace = new Vector2Int(coordX, coordY);
		SpotSelected = true;
	}
	public void AiSelects(int coordX, int coordY){
		SetVisual(coordX, coordY, AiIcon);
		currentAiDifficulty.AiSelectedSpaces[coordX, coordY] = AiIcon;
	}
	// Mini Turn State Machine:
	private IEnumerator TurnLoop() {
		int winner = -2;
		while (!GameWon) {
			currentTurn = Turn.Player;
			SpotSelected = false;
			OnPlayerTurn.Invoke(); // enables player input
			yield return new WaitUntil(CheckSpotSelected);
			winner = CheckWinner();

			OnAiTurn.Invoke(); // disables player input
			yield return new WaitForSeconds(1f); // delay for visual purposes
			if (GameWon) break; // early return if game is won ; stops loop

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
	public int CheckWinner() {
		for (int x = 0; x < boardState.GetLength(0); x++) {
			// all rows
			if (boardState[x, 0] == playerIcon && boardState[x, 1] == playerIcon && boardState[x, 2] == playerIcon) {
				GameWon = true;
				return 0;
			}
			if (boardState[x, 0] == AiIcon && boardState[x, 1] == AiIcon && boardState[x, 2] == AiIcon) {
				GameWon = true;
				return 1;
			}
			// all columns
			if (boardState[0, x] == playerIcon && boardState[1, x] == playerIcon && boardState[2, x] == playerIcon) {
				GameWon = true;
				return 0;
			}
			if (boardState[0, x] == AiIcon && boardState[1, x] == AiIcon && boardState[2, x] == AiIcon) {
				GameWon = true;
				return 1;
			}			
		}
		// all diagonals
		return CheckDiagonals();
    }
	private int CheckDiagonals() {
		if (boardState[0, 0] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 2] == playerIcon) {
			GameWon = true;
			return 0;
		}
		if (boardState[0, 0] == AiIcon && boardState[1, 1] == AiIcon && boardState[2, 2] == AiIcon) {
			GameWon = true;
			return 1;
		}
		if (boardState[0, 2] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 0] == playerIcon) {
			GameWon = true;
			return 0;
		}
		if (boardState[0, 2] == AiIcon && boardState[1, 1] == AiIcon && boardState[2, 0] == AiIcon) {
			GameWon = true;
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
		SetBoardState(coordX, coordY, targetIcon);
		messager.UpdateMiniBoard(boardState);
	}
	private void SetBoardState(int coordX, int coordY, TicTacToeIcon targetIcon) {
		boardState[coordX, coordY] = targetIcon;
		_triggers[coordX, coordY].canClick = false;
	}
}
