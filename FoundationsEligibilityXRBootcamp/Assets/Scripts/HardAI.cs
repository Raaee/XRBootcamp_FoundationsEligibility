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
	 *  
	 */
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		List<Vector2Int> cornerSpaces = InitCornerSpaces();
		Vector2Int nextPossible = new Vector2Int(-1, -1);
		

		if (availableSpaces.Count >= ticTacToeAI.boardState.Length - 1) {
			nextPossible = GetRandomSpace(availableSpaces);
			ticTacToeAI.AiSelects(nextPossible.x, nextPossible.y);
			ticTacToeAI.SpotSelected = true;
			return;
		}
		//nextPossible = CheckAdjacent(playerLastSelectedSpace);
		if (nextPossible == new Vector2Int(-1,-1)) {
			nextPossible = GetRandomSpace(availableSpaces);
		}
		ticTacToeAI.AiSelects(nextPossible.x, nextPossible.y);
		ticTacToeAI.SpotSelected = true;
	}
    private Vector2Int CheckPossibleWin(Vector2Int nextPossibleWin) {
		Vector2Int nextPossible = new Vector2Int(-1, -1);
		

		return nextPossible;
	}
	private Vector2Int CheckDiagonal() {
		return Vector2Int.zero;
	}
	private List<Vector2Int> InitCornerSpaces() {
		List<Vector2Int> cornerSpaces = new List<Vector2Int>();
		cornerSpaces.Add(Vector2Int.zero); // 00
		cornerSpaces.Add(new Vector2Int(0, ticTacToeAI.boardState.GetLength(1)-1)); //02
		cornerSpaces.Add(new Vector2Int(ticTacToeAI.boardState.GetLength(0)-1, 0)); // 20
		cornerSpaces.Add(new Vector2Int(ticTacToeAI.boardState.GetLength(0)-1, ticTacToeAI.boardState.GetLength(1)-1)); // 22
		return cornerSpaces;
	}
}
