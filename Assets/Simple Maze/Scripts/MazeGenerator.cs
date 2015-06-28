using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum MazeCellDirection
{
	None = 0,
	
	West = 1,
	South = 1 << 1,
	East = 1 << 2,
	North = 1 << 3,
	
	WestSouth = West | South,
	WestEast = West | East,
	WestNorth = West | North,
	SouthEast = South | East,
	SouthNorth = South | North,
	EastNorth = East | North,
	WestSouthEast = West | South | East,
	WestSouthNorth = West | South | North,
	WestEastNorth = West | East | North,
	SouthEastNorth = South | East | North,
	
	All = West | South | East | North	
}

public class MazeGenerator : MonoBehaviour
{
	public int colCount;
	public int rowCount;
	
	public MazeCell mazeCellPrefab;
	
	public GameObject startIndicationPrefab;
	
	public GameObject midwayIndicationPrefab;
	
	private List<List<MazeCell>> _mazeCells = null;
		
	private Stack<MazeCell> stackedCells;
	
	private GameObject _startIndication = null;
		
	void Awake()
	{
		//  mazeCells = new List<List<MazeCell>>();
		//  for (int i = 0; i < rowCount; ++ i)
		//  {
		//  	mazeCells.Add(new List<MazeCell>());
		//  }
		
		//  stackedCells = new Stack<MazeCell>();
	}
	
	void Start()
	{
		//  CreateMazeCells();
		//  Carve();
		//  PresentCells();
		Regenerate();
	}
	
	public void Regenerate()
	{
		if (_mazeCells != null)
		{
			for (int i = 0; i < rowCount; ++ i)
			{
				for (int j = 0; j < colCount; ++ j)
				{
					GameObject.Destroy(_mazeCells[i][j].gameObject);
				}
			}			
		}
		
		if (_startIndication != null)
		{
			GameObject.Destroy(_startIndication);
		}
		
		_mazeCells = new List<List<MazeCell>>();
		for (int i = 0; i < rowCount; ++ i)
		{
			_mazeCells.Add(new List<MazeCell>());
		}
		
		stackedCells = new Stack<MazeCell>();
		CreateMazeCells();
		Carve();
		PresentCells();		
	}
	
	void CreateMazeCells()
	{
		for (int i = 0; i < rowCount; ++ i)
		{
			for (int j = 0; j < colCount; ++ j)
			{
				var cellPosition = new Vector3(j, 0, i);
				var cell = Instantiate<MazeCell>(mazeCellPrefab);
				cell.transform.position = cellPosition;
				
				if (i == 0)
				{
					cell.border |= MazeCellDirection.South;
				}
				else if (i == rowCount - 1)
				{
					cell.border = MazeCellDirection.North;					
				}

				if (j == 0)
				{
					cell.border |= MazeCellDirection.West;
				}
				else if (j == colCount - 1)
				{
					cell.border |= MazeCellDirection.East;						
				}
				
				cell.colIndex = j;
				cell.rowIndex = i;
				
				cell.Init();
				
				cell.ActivateCell();
				
				_mazeCells[i].Add(cell);
			}
		}
	}
	
	void Carve()
	{
		Func<MazeCell> pickupOneCell = () => {
			int randomCol = UnityEngine.Random.Range(0, colCount);
			int randomRow = UnityEngine.Random.Range(0, rowCount);
			
			return _mazeCells[randomRow][randomCol];
		};
		
		//Func<MazeCellDirection, bool> hasSomeDirection = (MazeCellDirection mazeCellDirection) => {
		//	return (mazeCellDirection != MazeCellDirection.None);
		//};
		
		Func<MazeCell, List<MazeCell>> fetchNeighborCells = (MazeCell mazeCell) => {
			List<MazeCell> neighborCells = new List<MazeCell>();
			
			//Debug.Log("Fetch neighbor cells");
			
			if (!mazeCell.HasBorderAtDirection(MazeCellDirection.West))
			{
				//Debug.Log("No border at west");
				var neighborCell = _mazeCells[mazeCell.rowIndex][mazeCell.colIndex - 1];
				neighborCell.neighboredDirection = MazeCellDirection.West;
				
				if (!neighborCell.visited)
				{
					//neighborCell.visited = true;
					neighborCells.Add(neighborCell);					
				}				
			}

			if (!mazeCell.HasBorderAtDirection(MazeCellDirection.East))
			{
				//Debug.Log("No border at east");
				var neighborCell = _mazeCells[mazeCell.rowIndex][mazeCell.colIndex + 1];
				neighborCell.neighboredDirection = MazeCellDirection.East;
				
				if (!neighborCell.visited)
				{
					//neighborCell.visited = true;
					neighborCells.Add(neighborCell);					
				}				
			}

			if (!mazeCell.HasBorderAtDirection(MazeCellDirection.South))
			{
				//Debug.Log("No border at south");

				var neighborCell = _mazeCells[mazeCell.rowIndex - 1][mazeCell.colIndex];
				neighborCell.neighboredDirection = MazeCellDirection.South;
				
				if (!neighborCell.visited)
				{
					//neighborCell.visited = true;
					neighborCells.Add(neighborCell);					
				}				
			}

			if (!mazeCell.HasBorderAtDirection(MazeCellDirection.North))
			{
				//Debug.Log("No border at north");

				var neighborCell = _mazeCells[mazeCell.rowIndex + 1][mazeCell.colIndex];
				neighborCell.neighboredDirection = MazeCellDirection.North;
				
				if (!neighborCell.visited)
				{
					//neighborCell.visited = true;
					neighborCells.Add(neighborCell);					
				}				
			}
			
			return neighborCells;	
		};
		
		Action<MazeCell, MazeCell> knockOutWall = (MazeCell current, MazeCell neighor) => {
			var neighboredDirection = neighor.neighboredDirection;
			if (neighboredDirection == MazeCellDirection.West)
			{
				current.RemoveWallAtDirection(MazeCellDirection.West);
				neighor.RemoveWallAtDirection(MazeCellDirection.East);
			}

			if (neighboredDirection == MazeCellDirection.South)
			{
				current.RemoveWallAtDirection(MazeCellDirection.South);
				neighor.RemoveWallAtDirection(MazeCellDirection.North);
			}

			if (neighboredDirection == MazeCellDirection.East)
			{
				current.RemoveWallAtDirection(MazeCellDirection.East);
				neighor.RemoveWallAtDirection(MazeCellDirection.West);
			}

			if (neighboredDirection == MazeCellDirection.North)
			{
				current.RemoveWallAtDirection(MazeCellDirection.North);
				neighor.RemoveWallAtDirection(MazeCellDirection.South);
			}
			
			current.UpdateWallAccordingly();
			neighor.UpdateWallAccordingly();
			
			//current.ActivateWall();
			//neighor.ActivateWall();
		};
		
		int totalCellCount = colCount * rowCount;
		var currentCell = pickupOneCell();
		currentCell.visited = true;
		//stackedCells.Push(currentCell);
		int visitedCellCount = 1;
		
		_startIndication = Instantiate<GameObject>(startIndicationPrefab);
		_startIndication.transform.SetParent(currentCell.transform);
		_startIndication.transform.localPosition = new Vector3(0, 0.5f, 0);
		_startIndication.transform.SetParent(null);	

		//Debug.Log("Start the loop");
		while ((visitedCellCount < totalCellCount) && (currentCell != null))
		{
			//Debug.Log("Current cell: " + currentCell.rowIndex + ", " + currentCell.colIndex);
		    List<MazeCell> neighborCells = fetchNeighborCells(currentCell);
			
			if (neighborCells.Count > 0)
			{
				//Debug.Log("Has neighbor: " + neighborCells.Count);
				int randomIndex = UnityEngine.Random.Range(0, neighborCells.Count);
				knockOutWall(currentCell, neighborCells[randomIndex]);
				stackedCells.Push(currentCell);
				currentCell = neighborCells[randomIndex];
				currentCell.visited = true;
				visitedCellCount ++;
			}
			else
			{
				//Debug.Log("Has no neighbor");
				
				if (stackedCells.Count > 0)
				{
					currentCell = stackedCells.Pop();										
				}
				else
				{
					Debug.Log("Nothing in stack");
					currentCell = null;
				}
			}
		}
	}
	
	void PresentCells()
	{
		for (int i = 0; i < rowCount; ++ i)
		{
			for (int j = 0; j < colCount; ++ j)
			{
				_mazeCells[i][j].UpdateWallAccordingly();
			}
		}
	}
}
