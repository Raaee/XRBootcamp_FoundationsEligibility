using System;
using System.Collections;
using System.Collections.Generic;
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
	TicTacToeIcon[,] boardState;
	[SerializeField] private EndMessage messager;
	[SerializeField] private GameObject _xPrefab;
	[SerializeField] private GameObject _oPrefab;
	[SerializeField] private TicTacToeIcon playerIcon = TicTacToeIcon.cross;

	private TicTacToeIcon aiIcon = TicTacToeIcon.circle;
	private Turn currentTurn = Turn.Player;
	private bool spotSelected = false;
	private bool gameWon = false;
	public UnityEvent onGameStarted;
	[HideInInspector] public UnityEvent OnAiTurn;
	[HideInInspector] public UnityEvent OnPlayerTurn;
	[SerializeField] private GameObject retryButton;

	public UnityEvent OnGameEnd;

	private ClickTrigger[,] _triggers;
	
	private void Awake()
	{
	}
    private void Start() {
		aiIcon = playerIcon == TicTacToeIcon.cross ? TicTacToeIcon.circle : TicTacToeIcon.cross; // makes the ai use the opposite of the player icon
    }
	private void Update() {
		messager.ShowCurrentTurn(currentTurn);
	}
	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		retryButton.SetActive(false);
		StartGame();
	}
	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3]; // init triggers 2D array
		boardState = new TicTacToeIcon[3,3]; // init boardState 2D array
		gameWon = false;
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
		spotSelected = true;
	}
	private void AiSelects(int coordX, int coordY){
		SetVisual(coordX, coordY, aiIcon);
	}
	// Mini Turn State Machine:
	private IEnumerator TurnLoop() {
		int winner = -2;
		while (!gameWon) {
			currentTurn = Turn.Player;
			spotSelected = false;
			OnPlayerTurn.Invoke(); // enables player input
			yield return new WaitUntil(SpotSelected);
			winner = CheckWinner();

			OnAiTurn.Invoke(); // disables player input
			yield return new WaitForSeconds(1f); // delay for visual purposes
			if (gameWon) break; // early return if game is won ; stops loop

			currentTurn = Turn.Machine;
			yield return new WaitForSeconds(0.5f); 
			StartAI();

			yield return new WaitUntil(SpotSelected);
			yield return new WaitForSeconds(0.5f);

			winner = CheckWinner();
		}
		OnGameEnd.Invoke();
		messager.OnGameEnded(winner);
		retryButton.SetActive(true);
	}
	private int CheckWinner() {
		for (int x = 0; x < boardState.GetLength(0); x++) {
			// all rows
			if (boardState[x, 0] == playerIcon && boardState[x, 1] == playerIcon && boardState[x, 2] == playerIcon) {
				gameWon = true;
				return 0;
			}
			if (boardState[x, 0] == aiIcon && boardState[x, 1] == aiIcon && boardState[x, 2] == aiIcon) {
				gameWon = true;
				return 1;
			}
			// all columns
			if (boardState[0, x] == playerIcon && boardState[1, x] == playerIcon && boardState[2, x] == playerIcon) {
				gameWon = true;
				return 0;
			}
			if (boardState[0, x] == aiIcon && boardState[1, x] == aiIcon && boardState[2, x] == aiIcon) {
				gameWon = true;
				return 1;
			}			
		}
		// all diagonals
		return CheckDiagonals();
    }
	private int CheckDiagonals() {
		if (boardState[0, 0] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 2] == playerIcon) {
			gameWon = true;
			return 0;
		}
		if (boardState[0, 0] == aiIcon && boardState[1, 1] == aiIcon && boardState[2, 2] == aiIcon) {
			gameWon = true;
			return 1;
		}
		if (boardState[0, 2] == playerIcon && boardState[1, 1] == playerIcon && boardState[2, 0] == playerIcon) {
			gameWon = true;
			return 0;
		}
		if (boardState[0, 2] == aiIcon && boardState[1, 1] == aiIcon && boardState[2, 0] == aiIcon) {
			gameWon = true;
			return 1;
		}
		return -1;
	}
	private void StartAI() {
		spotSelected = false;

		switch (_aiLevel) {
			case 0:
				EasyAI();
				break;
			case 1:
				HardAI();
				break;
		}
    }
	private bool SpotSelected() {
		return spotSelected == true;
    }
	/*
	 *  Easy AI Algorithm: Random Selection
	 * - create a list of available coords based on empty board spaces
	 * - selects a random coord based on the available list
	 */
	private void EasyAI() {
		List<Vector2Int> availableSpaces = new List<Vector2Int>();
		for (int x = 0; x < _triggers.GetLength(0); x++) {
			for (int y = 0; y < _triggers.GetLength(1); y++) {
				if (boardState[x, y] == TicTacToeIcon.none) {
					availableSpaces.Add(new Vector2Int(x, y));
                }
			}
		}
		if (availableSpaces.Count > 0) {
			Vector2Int randomPos = availableSpaces[UnityEngine.Random.Range(0, availableSpaces.Count)];
			AiSelects(randomPos.x, randomPos.y);
			spotSelected = true;	
		}
		if (availableSpaces.Count <= 0) {
			gameWon = true;
			spotSelected = true;
        }
	}
	/*
	 * Hard AI Algorithm: First Best Choice
	 * - explain steps
	 *  
	 */
	private void HardAI() {
		spotSelected = true;
	}
	private void SetVisual(int coordX, int coordY, TicTacToeIcon targetIcon)
	{
		Instantiate(
			targetIcon == TicTacToeIcon.circle ? _oPrefab : _xPrefab, 
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
		SetBoardState(coordX, coordY, targetIcon);
	}
	private void SetBoardState(int coordX, int coordY, TicTacToeIcon targetIcon) {
		boardState[coordX, coordY] = targetIcon;
		_triggers[coordX, coordY].canClick = false;
	}
}
