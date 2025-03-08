using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.SceneManagement;
using UnityEngine;

   /// <summary>
   /// Hard AI Algorithm: First Best Choice
   /// - if there are 8 or more spaces available, it will select a random point to start
   /// - it then checks if there is a possible win position
   /// - if there is not, it will check if there is a possible block position
   /// - if there is not, it will select a random point from the available spaces
   /// </summary>
public class HardAI : AIAlgorithm
{
	private	List<Vector2Int> aiSpacesTaken = new List<Vector2Int>();
	private	Vector2Int bestMove = -Vector2Int.one;
	private int verticalSpaces = 0;
	private int horizontalSpaces = 0;
	private int diagonalSpaces = 0;
	private Vector2Int bestVertical = -Vector2Int.one;
	private Vector2Int bestHorizontal = -Vector2Int.one;
	private Vector2Int bestDiagonal = -Vector2Int.one;
	 
	public List<Vector2Int> playerSpacesTaken {get; set;} = new List<Vector2Int>();
	  
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		Vector2Int nextPossible;
		playerSpacesTaken.Add(PlayerLastSelectedSpace);

		if (availableSpaces.Count <= 0) { // ends game
			ticTacToeAI.SpotSelected = true;
			ticTacToeAI.GameWon = true;
			return;
		}
		// selects a random point to add some variety in the gameplay:
		if (availableSpaces.Count >= ticTacToeAI.boardState.Length * ticTacToeAI.boardState.GetLength(0) - 1) { // beginning of game
			nextPossible = GetRandomSpace(availableSpaces);
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}

		//nextPossible = CheckPossibleWins(); // CHECK WIN
		nextPossible = CheckForPossibleBlock();
		
		if (nextPossible == -Vector2Int.one) { // BLOCK PLAYER
			Debug.Log("Blocked with: " + nextPossible);
		}
		
		if (availableSpaces.Contains(Vector2Int.one)) { // middle
			nextPossible = Vector2Int.one;
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}
		
		Debug.Log("NP: " + nextPossible);
		if (nextPossible == -Vector2Int.one) { // if no space found
			nextPossible = GetRandomSpace(availableSpaces);
			Debug.Log("random");
		}
		FinalSelect(nextPossible.x, nextPossible.y);		
	}
	private void FinalSelect(int x, int y) {
		aiSpacesTaken.Add(new Vector2Int(x,y));
		ticTacToeAI.AiSelects(x, y);
		ticTacToeAI.SpotSelected = true;
	}
	/*
	* check all possible solutions from each space thats already taken
	* returns the first best solution
	*/
	private Vector2Int CheckForPossibleBlock() { // block player
		Vector2Int bestPossible = -Vector2Int.one;		
		for (int i = 0; i < playerSpacesTaken.Count; i++) {
			if (CheckHorizontal(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				bestMove = bestHorizontal;
			}
			/*if (CheckDiagonals(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				bestMove = bestDiagonal;
			}*/
			if (CheckVertical(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				bestMove = bestVertical;
			}
			Debug.Log("i: " + i + " | " + playerSpacesTaken[i]);

			//if (bestPossible != -Vector2Int.one) { // prevents immediate return
			//	return bestPossible;
			//}
		}
		ResetSpacesTaken();
		Debug.Log("Block H : " + bestHorizontal);
		//Debug.Log("Block D : " + bestDiagonal);
		//Debug.Log("Block V : " + bestVertical);
		return bestMove;
	}
	private void ResetSpacesTaken() {
		verticalSpaces = 0;
		horizontalSpaces = 0;
 		diagonalSpaces = 0;
	}
	private Vector2Int DetermineBestMove() {
		if (bestHorizontal != -Vector2Int.one) return bestHorizontal;
		if (bestDiagonal != -Vector2Int.one) return bestHorizontal;
		if (bestVertical != -Vector2Int.one) return bestHorizontal;
		return -Vector2Int.one;
	}
	/*
	* check all possible solutions from each space thats already taken
	* returns the first best solution
	*/
	private Vector2Int CheckPossibleWins() {
		Vector2Int bestPossible = -Vector2Int.one;
		
		for (int i = 0; i < aiSpacesTaken.Count; i++) {

			if (CheckHorizontal(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				bestPossible = bestMove;
			}
			if (CheckVertical(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				bestPossible = bestMove;
			}
			if (CheckDiagonals(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				bestPossible = bestMove;
			}

			if (bestPossible != -Vector2Int.one) { // prevents immediate return
				return bestPossible;
			}
			Debug.Log("wins: " + i + " | " + bestPossible);
		}
			Debug.Log("End BP " + bestPossible);

		return bestPossible;
	}
	/*
	* Checks all horizontal positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that column.
	*/
	private int CheckHorizontal(Vector2Int current, TicTacToeIcon wantIcon) {
		ResetSpacesTaken();

		for (int offset = -1; offset <= 1; offset++) {
			Vector2Int offsetPos = new Vector2Int(current.x, Clamp(current.y+offset));
			Debug.Log(offset + " | " + offsetPos + " | " + current);
			if (offsetPos == current) {
				Debug.Log("current");
				continue;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == wantIcon) {
				horizontalSpaces++;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == TicTacToeIcon.none) {
				bestHorizontal = offsetPos;
				break;
			}

		}
		Debug.Log("Spaces Taken H: " + horizontalSpaces);
		return horizontalSpaces;
	}
	/*
	* Checks all vertical positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that column.
	*/
	private int CheckVertical(Vector2Int current, TicTacToeIcon wantIcon) {
		ResetSpacesTaken();

		for (int offset = -1; offset <= 1; offset++) {
			Vector2Int offsetPos = new Vector2Int(Clamp(current.x+offset), current.y);

			if (offsetPos == current) {
				continue;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == wantIcon) {
				verticalSpaces++;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == TicTacToeIcon.none) {
				bestVertical = offsetPos;
				break;
			}
		}
		Debug.Log("Spaces Taken V: " + verticalSpaces);
		return verticalSpaces;
	}
	/*
	* Checks all diagonal positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that column.
	*/
	private int CheckDiagonals(Vector2Int current, TicTacToeIcon wantIcon) {
		int spacesTaken = 0;	
		bestMove = -Vector2Int.one;

		for (int offset = -1; offset <= 1; offset++) {
			if (new Vector2Int(Clamp(current.x+offset), Clamp(current.y-offset)) == current) { // skip same current
				continue;
			}
			if (new Vector2Int(Clamp(current.x+offset), Clamp(current.y+offset)) == current) { // skip same current
				continue;
			}
			// top-left to bottom-right
			if (ticTacToeAI.boardState[Clamp(current.x+offset), Clamp(current.y+offset)] == wantIcon) { 
				spacesTaken++;
			}
			if (ticTacToeAI.boardState[Clamp(current.x+offset), Clamp(current.y+offset)] == TicTacToeIcon.none) {
				bestMove = new Vector2Int(Clamp(current.x+offset), Clamp(current.y+offset));
			}
			// top-right to bottom-left
			if (ticTacToeAI.boardState[Clamp(current.x+offset), Clamp(current.y-offset)] == wantIcon) { 
				spacesTaken++;
			}
			if (ticTacToeAI.boardState[Clamp(current.x+offset), Clamp(current.y-offset)] == TicTacToeIcon.none) {
				bestMove = new Vector2Int(Clamp(current.x+offset), Clamp(current.y-offset));
			}
		}
		Debug.Log("Spaces Taken D: " + diagonalSpaces);
		return diagonalSpaces;
	}
	// Helper/Simplifier function:
	private int Clamp(int value) {
		return Mathf.Clamp(value, 0, ticTacToeAI.boardState.GetLength(0)-1);
	}
}
