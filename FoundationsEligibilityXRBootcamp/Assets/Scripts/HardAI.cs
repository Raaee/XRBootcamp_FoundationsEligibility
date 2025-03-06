using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HardAI : AIAlgorithm
{
    /*
	 * Hard AI Algorithm: First Best Choice
	 * - if there are 8 or more spaces available, it will select a random point to start
	 * - 
	 * win
	 block
	 middle
	 corners
	 */
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		List<Vector2Int> cornerSpaces = InitCornerSpaces();
		Vector2Int nextPossible = new Vector2Int(-1, -1);

		if (availableSpaces.Count <= 0) {
			ticTacToeAI.SpotSelected = true;
			ticTacToeAI.GameWon = true; // ends game
			return;
		}
		if (availableSpaces.Count >= ticTacToeAI.boardState.Length - 1) { // beginning of game
			nextPossible = GetRandomSpace(availableSpaces);
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}
		if (availableSpaces.Contains(Vector2Int.one)) { // middle
			nextPossible = Vector2Int.one;
			FinalSelect(nextPossible.x, nextPossible.y);
			return;
		}
		nextPossible = GetAvailableCorner(cornerSpaces, availableSpaces); // corners

		if (nextPossible ==  -Vector2Int.one) {
			nextPossible = GetRandomSpace(availableSpaces);
		}

		FinalSelect(nextPossible.x, nextPossible.y);
	}
	private void FinalSelect(int x, int y) {
		ticTacToeAI.AiSelects(x, y);
		ticTacToeAI.SpotSelected = true;
	}
    private Vector2Int CheckPossibleWin(Vector2Int nextPossibleWin) {
		Vector2Int nextPossible = -Vector2Int.one;
		bool winning = ticTacToeAI.CheckWinner() == 1 ? true : false;



		return nextPossible;
	}
	private Vector2Int CheckSides() {
		return Vector2Int.zero;
	}
    private Vector2Int GetAvailableCorner(List<Vector2Int> cornerSpaces, List<Vector2Int> availableSpaces) {
        Vector2Int possibleSpace = -Vector2Int.one;
		foreach (Vector2Int cornerSpace in cornerSpaces) {
				if (ticTacToeAI.boardState[cornerSpace.x, cornerSpace.y] == ticTacToeAI.playerIcon) {
					possibleSpace = CheckForPossibleBlock(cornerSpace);
					return possibleSpace;
				}
				else if (availableSpaces.Contains(cornerSpace)) {
					possibleSpace = CheckForPossiblelWin(cornerSpace);

				}
				else {

				}
			}
		return possibleSpace;
	}
	private Vector2Int CheckForPossiblelWin(Vector2Int cornerSpace) {
		// down
		if (ticTacToeAI.boardState[Clamp(cornerSpace.x+1), cornerSpace.y] == TicTacToeIcon.none && 
			ticTacToeAI.boardState[Clamp(cornerSpace.x+2), cornerSpace.y] == ticTacToeAI.AiIcon)	{ // middle space available
			return new Vector2Int(Clamp(cornerSpace.x+1), cornerSpace.y);
		}
		else if (ticTacToeAI.boardState[Clamp(cornerSpace.x+1), cornerSpace.y] == ticTacToeAI.AiIcon && 
			ticTacToeAI.boardState[Clamp(cornerSpace.x+2), cornerSpace.y] == TicTacToeIcon.none)	{ // last space available
			return new Vector2Int(Clamp(cornerSpace.x+2), cornerSpace.y);
		}
		// up
		if (ticTacToeAI.boardState[Clamp(cornerSpace.x-1), cornerSpace.y] == TicTacToeIcon.none && 
			ticTacToeAI.boardState[Clamp(cornerSpace.x-2), cornerSpace.y] == ticTacToeAI.AiIcon)	{  // middle space available
			return new Vector2Int(Clamp(cornerSpace.x-1), cornerSpace.y);
		}
		else if (ticTacToeAI.boardState[Clamp(cornerSpace.x-1), cornerSpace.y] == ticTacToeAI.AiIcon && 
			ticTacToeAI.boardState[Clamp(cornerSpace.x-2), cornerSpace.y] == TicTacToeIcon.none)	{ // last space available
			return new Vector2Int(Clamp(cornerSpace.x-2), cornerSpace.y);
		}
		// left
		

		// right

		// diagonal up / down

		return -Vector2Int.one;
	}
	private Vector2Int CheckHorizontal(int startX) {
		return Vector2Int.one;
	}
	private int Clamp(int value) {
		return Mathf.Clamp(value, 0, ticTacToeAI.boardState.GetLength(0)-1);
	}
	private Vector2Int CheckForPossibleBlock(Vector2Int cornerSpace) { // block player
		Debug.Log("Attempting block");
		return -Vector2Int.one;
	}
	private List<Vector2Int> InitCornerSpaces() {
		List<Vector2Int> cornerSpaces = new List<Vector2Int>() {
            Vector2Int.zero, // 00
            new Vector2Int(0, ticTacToeAI.boardState.GetLength(1)-1), //02
            new Vector2Int(ticTacToeAI.boardState.GetLength(0)-1, 0), // 20
            new Vector2Int(ticTacToeAI.boardState.GetLength(0)-1, ticTacToeAI.boardState.GetLength(1)-1) // 22
        };
		return cornerSpaces;
	}
}
