using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Abstract class for each AI algorithm
/// </summary>
public abstract class AIAlgorithm : MonoBehaviour
{
    protected TicTacToeAI ticTacToeAI;
    public TicTacToeIcon[,] AiSelectedSpaces {get; set;}
	public Vector2Int PlayerLastSelectedSpace {get; set;} = Vector2Int.zero;
    public abstract void StartAI();
    void Start()
    {
        ticTacToeAI = FindObjectOfType<TicTacToeAI>();
		AiSelectedSpaces = new TicTacToeIcon[3,3]; // init aiSelectedSpaces 2D array
    }
    protected List<Vector2Int> GetAvailableSpaces() {
		List<Vector2Int> availableSpaces = new List<Vector2Int>();
			for (int x = 0; x < ticTacToeAI.boardState.GetLength(0); x++) {
				for (int y = 0; y < ticTacToeAI.boardState.GetLength(1); y++) {
					if (ticTacToeAI.boardState[x, y] == TicTacToeIcon.none) {
						availableSpaces.Add(new Vector2Int(x, y));
					}
				}
			}
		return availableSpaces;
	}
    protected Vector2Int GetRandomSpace(List<Vector2Int> availableSpaces) {
		return availableSpaces[UnityEngine.Random.Range(0, availableSpaces.Count)];
	}
}
