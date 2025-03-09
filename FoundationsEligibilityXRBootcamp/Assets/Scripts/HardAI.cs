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
   /// It does this by checking each row/column/diagonal from each space that has already been taken.
   /// 
   /// Notes:
   /// I am aware this code is complicated. But I really wanted to challenge myself in making an algorithm that does NOT 
   ///		loop through the entire board every single time it wants to check for a possible space to place a pawn (unless the board is full ig).
   /// There are definitely improvements to be made AND it does sometimes mess up. But the system works to the best of its abilities.
   /// </summary>
public class HardAI : AIAlgorithm
{
	private	List<Vector2Int> aiSpacesTaken = new List<Vector2Int>();
	private Vector2Int bestHorizontal = -Vector2Int.one;
	private Vector2Int bestVertical = -Vector2Int.one;
	private Vector2Int bestDiagonal = -Vector2Int.one;
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
		
		if (nextPossible == -Vector2Int.one) { // BLOCK PLAYER
			nextPossible = CheckForPossibleBlock();
		}
		
		if (nextPossible == -Vector2Int.one) { // if no space found
			nextPossible = GetRandomSpace(availableSpaces);
		}

		FinalSelect(nextPossible.x, nextPossible.y);
	}
	private void FinalSelect(int x, int y) {
		aiSpacesTaken.Add(new Vector2Int(x,y));
		ticTacToeAI.AiSelects(x, y);
		ticTacToeAI.SpotSelected = true;
	}
	/**
	* checks all possible solutions from each space thats already taken
	* returns the first best solution
	**/
	private Vector2Int CheckPossibleWins() { // checks for a win condition for the ai

		for (int i = 0; i < aiSpacesTaken.Count; i++) {
			CheckDiagonals(aiSpacesTaken[i], ticTacToeAI.AiIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestDiagonal;
			}
			CheckHorizontal(aiSpacesTaken[i], ticTacToeAI.AiIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestHorizontal;
			}
			CheckVertical(aiSpacesTaken[i], ticTacToeAI.AiIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestVertical;
			}
		}
		return CheckBest(); // final catch
	}
	/**
	* checks all possible solutions from each space thats already taken
	* returns the first best solution
	**/
	private Vector2Int CheckForPossibleBlock() { // block player

		for (int i = 0; i < playerSpacesTaken.Count; i++) {

			CheckDiagonals(playerSpacesTaken[i], ticTacToeAI.playerIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestDiagonal;
			}
			CheckHorizontal(playerSpacesTaken[i], ticTacToeAI.playerIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestHorizontal;
            }
			CheckVertical(playerSpacesTaken[i], ticTacToeAI.playerIcon);
			if (CheckBest() != -Vector2Int.one) {
				return bestVertical;
			}
		}
		return CheckBest(); // final catch
	}
	private Vector2Int CheckBest() {
		if (bestDiagonal != -Vector2Int.one) return bestDiagonal;
		if (bestHorizontal != -Vector2Int.one) return bestHorizontal;
		if (bestVertical != -Vector2Int.one) return bestVertical;
		return -Vector2Int.one;
	}
	/**
	* Checks all horizontal positions from the current position based on the spaces already taken
	* Returns the the best move to the space that is empty from that row.
	**/
	private Vector2Int CheckHorizontal(Vector2Int current, TicTacToeIcon wantIcon) {
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
		if (spacesTaken == 2) {
			bestHorizontal = bestPossible;
			return bestHorizontal;
		}
		bestHorizontal = -Vector2Int.one;
		return bestHorizontal;
	}
	/**
	* Checks all vertical positions from the current position based on the spaces already taken
	* Returns the best move to the space that is empty from that column.
	**/
	private Vector2Int CheckVertical(Vector2Int current, TicTacToeIcon wantIcon) {
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
		if (spacesTaken == 2) {
			bestVertical = bestPossible;
			return bestVertical;
		}
		
		bestVertical = -Vector2Int.one;
		return -Vector2Int.one;
	}
	/**
	* Checks all diagonal positions from the current position based on the spaces already taken
	* Returns the best move to the space that is empty from a diagonal line.
	**/
	private Vector2Int CheckDiagonals(Vector2Int current, TicTacToeIcon wantIcon) {
		int spacesTaken = 0;
		Vector2Int bestPossible = -Vector2Int.one;

		for (int offset = -2; offset <= 2; offset++) {
			// these are not clamped because they are diagonal. Clamping these offsets would cause it to check the incorrect space:
			Vector2Int offsetPosTRBL = new Vector2Int(current.x + offset, current.y - offset);
			Vector2Int offsetPosTLBR = new Vector2Int(current.x + offset, current.y + offset);

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
			/// because they are not clamped, this makes sure the offsetPos is not out of bounds of the board before checking that space
			if (!IsOutOfBounds(offsetPosTLBR.x) && !IsOutOfBounds(offsetPosTLBR.y)) { 
				if (ticTacToeAI.boardState[offsetPosTLBR.x, offsetPosTLBR.y] == wantIcon) { // icon exists
					spacesTaken++;
				}
				else if (ticTacToeAI.boardState[offsetPosTLBR.x, offsetPosTLBR.y] == TicTacToeIcon.none) { // empty space
					bestPossible = offsetPosTLBR;
					continue;
				}
			}
			// Top-Right to Bottom-Left
			/// because they are not clamped, this makes sure the offsetPos is not out of bounds of the board before checking that space
			if (!IsOutOfBounds(offsetPosTRBL.x) && !IsOutOfBounds(offsetPosTRBL.y)) {
				if (ticTacToeAI.boardState[offsetPosTRBL.x, offsetPosTRBL.y] == wantIcon) { // icon exists
					spacesTaken++;
				}
				else if (ticTacToeAI.boardState[offsetPosTRBL.x, offsetPosTRBL.y] == TicTacToeIcon.none) { // empty space
					bestPossible = offsetPosTRBL;
					continue;
				}
			}
		}
		if (spacesTaken == 2) {
			bestDiagonal = bestPossible;
			return bestDiagonal;
		}
		bestDiagonal = -Vector2Int.one;
		return -Vector2Int.one;
	}
	// helper/simplifier functions:
	private int Clamp(int value) {
		return Mathf.Clamp(value, 0, ticTacToeAI.boardState.GetLength(0)-1);
	}
	private bool IsOutOfBounds(int value) {
		int clampedValue = Clamp(value);
		return clampedValue != value;
    }
}
