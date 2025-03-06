using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAI : AIAlgorithm
{
    /*
	 *  Easy AI Algorithm: Random Selection
	 * - create a list of available coords based on empty board spaces
	 * - selects a random coord based on the available list
	 */
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		
		if (availableSpaces.Count > 0) {
			Vector2Int randomPos = GetRandomSpace(availableSpaces);
			ticTacToeAI.AiSelects(randomPos.x, randomPos.y);
			ticTacToeAI.SpotSelected = true;	
		}
		if (availableSpaces.Count <= 0) {
			ticTacToeAI.GameWon = true;
			ticTacToeAI.SpotSelected = true;
        }
	}
}
