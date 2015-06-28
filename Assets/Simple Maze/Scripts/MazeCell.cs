using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MazeCell : MonoBehaviour
{
	public MazeCellDirection border;
	public MazeCellDirection wall;
	
	public int colIndex;
	public int rowIndex;
	
	public MazeCellDirection neighboredDirection;
	
	public bool visited;
	
	public List<GameObject> borders;
	public List<GameObject> walls;
	
	void Awake()
	{
		
	}
	
	public void Init()
	{
		wall = MazeCellDirection.East | MazeCellDirection.North | MazeCellDirection.South | MazeCellDirection.West;
		//Debug.Log("Cell wall set to all direction");
		//Debug.Log(wall);
	}
	
	private bool HasSomeDirection(MazeCellDirection mazeCellDirection)
	{
		return (mazeCellDirection != MazeCellDirection.None);
	}
	
	public bool HasBorderAtDirection(MazeCellDirection mazeCellDirection)
	{
		return HasSomeDirection(border & mazeCellDirection);
	}
	
	public void RemoveWallAtDirection(MazeCellDirection mazeCellDirection)
	{
		//Debug.Log("Before: " + wall);
		wall &= ~mazeCellDirection;
		//Debug.Log("After: " + wall);
	} 
	
	public void ActivateCell()
	{
		ActivateBorder();
		ActivateWall();
	}
	
	public void ActivateBorder()
	{
		borders.ForEach(x => x.SetActive(false));
		if (HasSomeDirection(border & MazeCellDirection.West))
		{
			borders[0].SetActive(true);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.South))
		{
			borders[1].SetActive(true);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.East))
		{
			borders[2].SetActive(true);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.North))
		{
			borders[3].SetActive(true);
		}
	}
	
	public void ActivateWall()
	{
		walls.ForEach(x => x.SetActive(true));
		
		if (HasSomeDirection(border & MazeCellDirection.West))
		{
			walls[0].SetActive(false);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.South))
		{
			walls[1].SetActive(false);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.East))
		{
			walls[2].SetActive(false);
		}
		
		if (HasSomeDirection(border & MazeCellDirection.North))
		{
			walls[3].SetActive(false);
		}
	}
	
	public void UpdateWallAccordingly()
	{
		if (!HasSomeDirection(wall & MazeCellDirection.West))
		{
			walls[0].SetActive(false);
		}

		if (!HasSomeDirection(wall & MazeCellDirection.South))
		{
			walls[1].SetActive(false);
		}

		if (!HasSomeDirection(wall & MazeCellDirection.East))
		{
			walls[2].SetActive(false);
		}

		if (!HasSomeDirection(wall & MazeCellDirection.North))
		{
			walls[3].SetActive(false);
		}
	}
}
