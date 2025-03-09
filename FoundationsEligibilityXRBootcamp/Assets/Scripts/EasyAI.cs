using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Easy AI Algorithm: Random Selection
/// - create a list of available coords based on empty board spaces
///	- selects a random coord based on the available list
///	
/// Notes:
/// This was the most simple AI that does not use the player or its own pieces to calculate its next move.
/// It selects a random spot to place its pawn every turn.
/// Though, sometimes the rng seems a little too good.....
/// </summary>
public class EasyAI : AIAlgorithm
{	 
    public override void StartAI() {
		List<Vector2Int> availableSpaces = GetAvailableSpaces();
		
		if (availableSpaces.Count > 0) { // If there is a space available, select a random spot
			Vector2Int randomPos = GetRandomSpace(availableSpaces);
			ticTacToeAI.AiSelects(randomPos.x, randomPos.y);
			ticTacToeAI.SpotSelected = true;	
		}
		if (availableSpaces.Count <= 0) { // ends game
			ticTacToeAI.GameComplete = true;
			ticTacToeAI.SpotSelected = true;
        }
	}
}
