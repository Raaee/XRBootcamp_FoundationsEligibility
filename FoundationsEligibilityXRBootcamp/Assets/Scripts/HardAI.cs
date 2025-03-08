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
	private Vector2Int bestMove = -Vector2Int.one;
	public List<Vector2Int> playerSpacesTaken {get; set;} = new List<Vector2Int>();
	  
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		Vector2Int nextPossible;
		playerSpacesTaken.Add(PlayerLastSelectedSpace);

		if (availableSpaces.Count <= 0) { // ends game
			ticTacToeAI.SpotSelected = true;
			ticTacToeAI.GameComplete = true;
			return;
		}
		// selects a random point at the beginning to add some variety in the gameplay:
		if (availableSpaces.Count >= 8) {
			nextPossible = GetRandomSpace(availableSpaces);
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}
		nextPossible = CheckPossibleWins(); // CHECK WIN
//		nextPossible = CheckForPossibleBlock();
		
		if (nextPossible == -Vector2Int.one) { // BLOCK PLAYER
			Debug.Log("win with: " + nextPossible);
			/// current issue: this block is never called bc its never at nothing
		}


		/*if (availableSpaces.Contains(Vector2Int.one)) { // middle
			nextPossible = Vector2Int.one;
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}*/
		
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
	/**
	* check all possible solutions from each space thats already taken
	* returns the first best solution
	**/
	private Vector2Int CheckPossibleWins() {
		//bestMove = -Vector2Int.one;

		for (int i = 0; i < aiSpacesTaken.Count; i++) {

			if (CheckHorizontal(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
			if (CheckVertical(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
			if (CheckDiagonals(aiSpacesTaken[i], ticTacToeAI.AiIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
		}
		return bestMove;
	}
	/**
	* check all possible solutions from each space thats already taken
	* returns the first best solution
	**/
	private Vector2Int CheckForPossibleBlock() { // block player
		//bestMove = -Vector2Int.one;

		for (int i = 0; i < playerSpacesTaken.Count; i++) {

			if (CheckHorizontal(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
			if (CheckVertical(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
			if (CheckDiagonals(playerSpacesTaken[i], ticTacToeAI.playerIcon) == 2) {
				if (bestMove != -Vector2Int.one) {
					return bestMove;
				}
			}
		}
		return bestMove;
	}
	/**
	* Checks all horizontal positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that row.
	**/
	private int CheckHorizontal(Vector2Int current, TicTacToeIcon wantIcon) {
		int spacesTaken = 0;
		Vector2Int bestPossible = -Vector2Int.one;

		for (int offset = -2; offset <= 2; offset++) {
			Vector2Int offsetPos = new Vector2Int(current.x, Clamp(current.y+offset));

			if (offset == 0) { // skips same offset position as current, BUT counts itself as a taken space
				spacesTaken++;
				continue;
            }
			if (offsetPos == current) { // skips same offset positon as current
				continue;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == wantIcon) { // icon exists in the space
				spacesTaken++;
			}
			else if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == TicTacToeIcon.none) { // empty space
				bestPossible = offsetPos;
			}
		}
		bestMove = bestPossible;
		return spacesTaken;
	}
	/**
	* Checks all vertical positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that column.
	**/
	private int CheckVertical(Vector2Int current, TicTacToeIcon wantIcon) {
		int spacesTaken = 0;
		Vector2Int bestPossible = -Vector2Int.one;

		for (int offset = -2; offset <= 2; offset++) {
			Vector2Int offsetPos = new Vector2Int(Clamp(current.x+offset), current.y);

			if (offset == 0) { // skips same offset position as current, BUT counts itself as a taken space
				spacesTaken++;
				continue;
			}
			if (offsetPos == current) { // skips same current
				continue;
			}
			if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == wantIcon) { // icon exists in space
				spacesTaken++;
			}
			else if (ticTacToeAI.boardState[offsetPos.x, offsetPos.y] == TicTacToeIcon.none) { // empty space
				bestPossible = offsetPos;
			}
		}
		bestMove = bestPossible;
		return spacesTaken;
	}
	/**
	* Checks all diagonal positions from the current position based on the spaces already taken
	* Returns the count of spaces that have been taken and assigns the best move to the space that is empty from that diagonal line.
	**/
	private int CheckDiagonals(Vector2Int current, TicTacToeIcon wantIcon) {
		int spacesTaken = 0;
		Vector2Int bestPossible = -Vector2Int.one;

		for (int offset = -2; offset <= 2; offset++) {
			Vector2Int offsetPosTRBL = new Vector2Int(Clamp(current.x + offset), Clamp(current.y - offset));
			Vector2Int offsetPosTLBR = new Vector2Int(Clamp(current.x + offset), Clamp(current.y + offset));

			if (offset == 0) { // skips same current, BUT counts it as a taken space
				spacesTaken++;
				continue;
            }
			if (offsetPosTRBL == current) { // skip same current for TR-BL
				continue;
			}
			if (offsetPosTLBR == current) { // skip same current for TL-BR
				continue;
			}
			// Top-Left to Bottom-Right
			if (ticTacToeAI.boardState[offsetPosTLBR.x, offsetPosTLBR.y] == wantIcon) { // icon exists
				spacesTaken++;
			}
			else if (ticTacToeAI.boardState[offsetPosTLBR.x, offsetPosTLBR.y] == TicTacToeIcon.none) { // empty space
				bestPossible = offsetPosTLBR;
			}
			// Top-Right to Bottom-Left
			if (ticTacToeAI.boardState[offsetPosTRBL.x, offsetPosTRBL.y] == wantIcon) { // icon exists
				spacesTaken++;
			}
			else if (ticTacToeAI.boardState[offsetPosTRBL.x, offsetPosTRBL.y] == TicTacToeIcon.none) { // empty space
				bestPossible = offsetPosTRBL;
			}
		}
		bestMove = bestPossible;
		return spacesTaken;
	}
	// Helper/Simplifier function:
	private int Clamp(int value) {
		return Mathf.Clamp(value, 0, ticTacToeAI.boardState.GetLength(0)-1);
	}
}
