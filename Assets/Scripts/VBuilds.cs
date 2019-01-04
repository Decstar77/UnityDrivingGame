using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VBuilds
{

	/*
	 Could have much better privacy status for structs. Made use funtions;
	 Better random choosing of next edge
	 Most of the class can never have a vert with the degree with more than 4. Espiecally FillCylce(); If the if of positions.x
	 Ativevert() TEMPLATE PLZ!!!
	 -finish cycle funcion with next vert
	 -Choosing direction is very biased. FIX!!
	*/


	private uint RoadCountWidth = 10;
	private uint RoadCountHeight = 10;
	private uint PathLength = 50;
	public struct Edges
	{
		//Stores Global indexs of vertices
		public int indexFrom;
 		public int indexTo;
	}
	public struct Vertices
	{
		public Edges[] edge;
		public Vector2 pos;
		public bool active;
		public int degree;
		public int index;
		public int pathIndex;
		public int activeEdges;
		public Vertices(bool act, int amountOfedges, float x, float y, int m_index = -1)
		{
			edge = new Edges[amountOfedges];
			degree = amountOfedges;
			pos.x = x;
			pos.y = y;
			active = act;
			index = m_index;
			pathIndex = -1;
			activeEdges = 0;
		}
	}
	//AllVerts
	private Vertices[] verts; 
	//Verts in the path
	private Vertices[] vertsPath;
	private Vertices lastVert;
	private float sizeOfUnit;
	private int midIndex = 0;
	//Can used to get the amount of active verts or to get the next empty index in pathArray. HOWEVER BEFORE INC'ING IT, inc after activating another vert. eg Init()
	private int pathVertAmount = 0;                                                                               
	private int activeVertAmount = 0;

	private void InitializeVerts()
	{
		//In short, make a grid of vertices
		for (int x = 0; x < RoadCountWidth; x++)
		{
			for (int y = 0; y < RoadCountHeight; y++)
			{
				bool left = true;
				bool right = true;
				bool up = true;
				bool down = true;
				int amountOfEdge = 4;
				int index = GetIndex(x, y);
				if (x - 1 < 0)
				{
					amountOfEdge--;
					left = false;
				}
				if (x + 1 == RoadCountWidth)
				{
					amountOfEdge--;
					right = false;
				}
				if (y - 1 < 0)
				{
					amountOfEdge--;
					down = false;
				}
				if (y + 1 == RoadCountHeight)
				{
					amountOfEdge--;
					up = false;
				}
				verts[index] = new Vertices(false, amountOfEdge, x, y, index);
				for (int i = 0; i < amountOfEdge; i++)
				{
					verts[index].edge[i].indexFrom = index;
					if (left)
					{
						verts[index].edge[i].indexTo = GetIndex(x - 1, y);
						left = false;
						continue;
					}
					if (right)
					{
						verts[index].edge[i].indexTo = GetIndex(x + 1, y);
						right = false;
						continue;
					}
					if (up)
					{
						verts[index].edge[i].indexTo = GetIndex(x, y + 1);
						up = false;
						continue;
					}
					if (down)
					{
						verts[index].edge[i].indexTo = GetIndex(x, y - 1);
						down = false;
						continue;
					}
				}

			}
		}
	}
	public bool CompareVerts(Vertices vert1, Vertices vert2)
	{
		//Compares to verts. Returns true of they are considerd the same
		if (vert1.active == vert2.active &&
			vert1.index == vert2.index &&
			vert1.pos == vert2.pos)
			return true;
		return false;
	}                                                   
	public bool CheckVecArray(Vector2 needsChecking, Vector2[] array)
	{
		//Returns true if a Vector is in the array
		for (int i = 0; i < array.Length; i++)
		{
			if (needsChecking == array[i])
				return true;
		}
		return false;
	}                                        
	public bool CheckVertArray(Vertices needChecking, Vertices[] array)
	{
		//Return true if a Vert is in the array
		for (int i = 0; i < array.Length; i++)
		{
			if (CompareVerts(needChecking, array[i]))
				return true;
		}
		return false;
	}                                        
	public void CheckSize(ref uint Height, ref uint Width, ref uint pathLength)
	{
		//Ensure thats the pathLength is at most half the size of the grid
		if ((Height * Width) / 2 < pathLength)
		{
			pathLength = (Height * Width) / 2;
		}
	}
	public void Init(uint Height = 10, uint Width = 10, uint pathLength = 50)
	{
		//Initializer function
		RoadCountWidth = Width;
		RoadCountHeight = Height;
		PathLength = pathLength;
		CheckSize(ref RoadCountHeight, ref RoadCountWidth, ref PathLength);
		verts = new Vertices[RoadCountWidth * RoadCountHeight];
		vertsPath = new Vertices[PathLength];
		InitializeVerts();
		Vector2 middle = new Vector2(RoadCountWidth / 2, RoadCountHeight / 2);
		midIndex = GetIndex((int)middle.x, (int)middle.y);
		verts[midIndex].active = true;
		activeVertAmount++;
		verts[midIndex].pathIndex = pathVertAmount;
		pathVertAmount++;
		verts[midIndex].index = midIndex;
		ActivateConnectedEdges(ref verts[midIndex]);
		vertsPath[verts[midIndex].pathIndex] = verts[midIndex];
		lastVert = verts[midIndex];
	}                                 
	public void ActivateInbetweens(Vertices vert1, Vertices vert2, bool AddToPath = true)
	{ 
		//Activates All verts between vert1 and vert2 in a str line.
		if (vert1.pos.x == vert2.pos.x && vert1.pos.y == vert2.pos.y)
		{
			Debug.Log("Verts have no Inbetweens");
			return;
		}
		bool dir = false;//False is the end vert is to the left or right; true is the end vert in above or below 
		int incDir = 0;
		if (vert1.pos.x == vert2.pos.x)
			dir = true;

		if (dir)
		{
			int amounts = Mathf.Abs((int)vert1.pos.y - (int)vert2.pos.y) - 1;
			if (amounts == 0)
				return;

			if (vert1.pos.y > vert2.pos.y)
			{
				incDir = 1;
			}
			else
			{
				incDir = -1;
			}
			for (int i = 1; i <= amounts; i++)
			{
				Vector2 vec = new Vector2(vert2.pos.x, vert2.pos.y + incDir * i);
				int index = GetIndex(vec);
				if (verts[index].active)
					continue;
				ActivateVert(index, false, AddToPath);
			}
		}
		else
		{
			int amounts = Mathf.Abs((int)vert1.pos.x - (int)vert2.pos.x) - 1;
			Debug.Log("sd" + amounts);
			if (amounts == 0)
				return;

			if (vert1.pos.x > vert2.pos.x)
			{
				incDir = 1;
			}
			else
			{
				incDir = -1;
			}
			for (int i = 1; i <= amounts; i++)
			{
				Vector2 vec = new Vector2(vert2.pos.x + incDir * i, vert2.pos.y);
				int index = GetIndex(vec);
				if (verts[index].active)
					continue;
				ActivateVert(index, false, AddToPath);
			}
		}
	}                    
	public void FillCylce(int indexStart, int indexEnd)
	{
		//Fills a cycle --Work in progress
		int cycleLenth = Mathf.Abs(verts[indexStart].pathIndex - verts[indexEnd].pathIndex) + 1; //Haven't acrtivated the indexStart vert thus plus 1
		Debug.Log("CycleLength: " + cycleLenth);
		if (cycleLenth < 8)
			return;
		Vertices[] cycle = new Vertices[cycleLenth];
		for (int i = 0; i < cycleLenth; i++) // Prehaps ???
		{
			cycle[i] = GetVertPath(verts[indexStart].pathIndex - i);
		}

		Vertices vertStart = GetVert(indexStart);
		Vertices vertEnd = GetVert(indexEnd);

		Debug.Log("Starting fill");
		for (int i = 0; i < cycleLenth; i++)
		{
			for (int ii = 0; ii < cycleLenth; ii++)
			{
				Vector2 posUp = new Vector2(cycle[i].pos.x, cycle[i].pos.y + 1 + ii);
				Vector2 posDown = cycle[i].pos + new Vector2(0, -1 - ii);
				if (posUp.y >= RoadCountHeight)
				{
					Debug.Log("BreakingRoadCountHeight");
					break;
				}
				if (posDown.y < 0)
				{
					Debug.Log("BreakingRoadCountHeight");
					break;
				}
				if (CheckVertArray(GetVert(posUp), cycle))
				{
					if (ii == 0)
						break;
					Vertices vert = GetVert(GetIndex(posUp));
					ActivateInbetweens(vert, cycle[i], false);
					Debug.Log("FoundUp: pos ");
					break;
				}
				if (CheckVertArray(GetVert(posDown), cycle))
				{
					if (ii == 0)
						break;
					Vertices vert = GetVert(GetIndex(posDown));
					ActivateInbetweens(vert, cycle[i], false);
					Debug.Log("FoundDown");
					break;
				}

			}

		}

	}                                                         
	public void NextVert()
	{
		//Auto chooses next vert from lastVert will necessary calculations
		if (pathVertAmount >= PathLength)
			return;
		int dir = Random.Range(0, lastVert.degree);
		int index = lastVert.edge[dir].indexTo;
		int incDir = 0;
		while (verts[index].active == true)
		{
			if (incDir == lastVert.degree)
			{
				RemoveFromPath(lastVert.index);
				incDir = 0;
			}
			dir = incDir;
			index = lastVert.edge[dir].indexTo;
			incDir++;
		}
		ActivateVert(index);
		/*if (verts[index].activeEdges > 1)
		{
			int indexEnd = GetNeighbourVert(true, verts[index], lastVert);
			//Debug.Log("Cycle, index");
			ActivateVert(index);
			FillCylce(index, indexEnd);
		}
		else
		{
			ActivateVert(index);
		}*/
	}                                                                                     
	public void DisplayVertsArray(Vertices[] Verts = null)
	{
		//Displays a vert array
		if (Verts != null)
		{
			for (int i = 0; i < RoadCountWidth * RoadCountHeight; i++)
			{
				Debug.Log(Verts[i].pos);
				Debug.Log(Verts[i].degree);
			}
		}
		else
		{
			for (int i = 0; i < verts.Length; i++)
			{
				Debug.Log(verts[i].pos);
				Debug.Log(verts[i].degree);
			}
		}
	}                                                     
	public void DisplayActiveVerts(bool activeStatus)
	{
		//Displays all active verts
		for (int i = 0; i < RoadCountWidth * RoadCountHeight; i++)
		{
			if (verts[i].active == activeStatus)
			{
				DisplayVert(verts[i]);
			}
		}
	}                                                         
	public void DisplayVert(Vertices vert)
	{
		//Displays a in-depth view of a vert
		Debug.Log("//////////////////////START////////////////////////////////");
		Debug.Log("PathIndex: " + vert.pathIndex + " TrueIndex: " + vert.index);
		Debug.Log("Position: " + vert.pos + " Active: " + vert.active);
		Debug.Log("Degree: " + vert.degree + " ActiveEdge: " + vert.activeEdges);
		Debug.Log("//////////////////////END//////////////////////////////////");
	}                                                                     
	public void DisplayEdge(Vertices vert)
	{
		//Display a in-depth view of a edge
		for (int i = 0; i < vert.degree; i++)
		{
			Debug.Log("Edge indexfrom: " + vert.edge[i].indexFrom + " Edge indexTo: " + vert.edge[i].indexTo + " i:" + i);
		}
	}                                                                     
	public void ActivateConnectedEdges(ref Vertices vert)
	{
		//Sets the vert to it's max activeAmount of edges as well as the adjacent vert's edges connected to it
		for (int i = 0; i < vert.degree; i++)
		{
			int indexTo = vert.edge[i].indexTo;
			if (verts[indexTo].activeEdges < verts[indexTo].degree) // To stop active edge overlapping the max degree of 4
				verts[indexTo].activeEdges++;
			//Debug.Log("TEstsss" + indexTo);
		}
		vert.activeEdges = vert.degree;
	}                                                    
	public void ChangeVert(int index, Vertices vert)
	{
		verts[index] = vert;
	}
	public void RemoveFromPath(int index)
	{
		verts[index].pathIndex = -1;
		pathVertAmount--;
		lastVert = GetVertPath(pathVertAmount - 1); // -1 because pathVertAmount is the next index in the path not the current. Cause init at 0
	}
	public int GetNeighbourVert(bool statusVert, Vertices vertCase, Vertices vertIgnor, bool lowestPath = true)
	{
		//Returns the global index of a vert which had the lowest path index off all verts connected to vertCase and is not the index of the ignoredVert
		int lowestIndex = 9999;
		for (int i = 0; i < vertCase.degree; i++)
		{
			int index = vertCase.edge[i].indexTo;
			if (index != vertIgnor.index && verts[index].active == statusVert)
			{
				if (lowestPath)
					lowestIndex = (vertCase.pathIndex <= lowestIndex) ? index : lowestIndex;
				else
					return index;
			}
		}
		return lowestIndex;
	}     
	public int ActivateVert(Vertices vert, bool SetLastVert = true, bool AddToPath = true)
	{
		//Activates a vert will all need inc's and setup. Use highly!
		int index = vert.index;
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}                    
	public int ActivateVert(int index, bool SetLastVert = true, bool AddToPath = true)
	{
		//Activates a vert will all need inc's and setup. Use highly!
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}                      
	public int ActivateVert(Vector2 pos, bool SetLastVert = true, bool AddToPath = true)
	{
		//Activates a vert will all need inc's and setup. Use highly!
		int index = GetIndex((int)pos.x, (int)pos.y);
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}                    
	public int GetIndex(int x, int y)
	{
		//Get the global index of a vert
		if (x >= RoadCountWidth || y >= RoadCountHeight)
			return -1;
		return x * (int)RoadCountWidth + y;
	}                                                                    
	public int GetIndex(Vector2 vec)
	{
		//Get the global index of a vert
		if (vec.x >= RoadCountWidth || vec.y >= RoadCountHeight)
			return -1;
		return (int)vec.x * (int)RoadCountWidth + (int)vec.y;
	}                                                                           
	public int GetPathIndex(Vector2 vec)
	{
		//Warning Expensive function. Advised to use get GetVert instead.
		//Get the path Index of a vert via a position, returns -1 if cannot find such a vert. Ie there is no such vert in the path
		if (vec.x >= RoadCountWidth || vec.y >= RoadCountHeight)
			return -1;
		for (int i = 0; i < pathVertAmount; i++)
		{
			if (vertsPath[i].pos == vec)
				return i;
		}
		return -1;
	}
	public int GetStatusVertAmount(bool PathRelative)
	{
		//Get the amount of verts relative to array
		if (PathRelative)
			return pathVertAmount;
		else
			return (int)RoadCountHeight * (int)RoadCountWidth;
	}                                                             
	public Vertices GetVertConnectedToEdge(bool statusVert, Vertices vertCase, Vertices vertIgnor)
	{
		//Gets a random vert connected to the vert case. Can ignore one vert
		for (int i = 0; i < vertCase.degree; i++)
		{
			int index = vertCase.edge[i].indexTo;
			if (index != vertIgnor.index && verts[index].active == statusVert)
			{
				return vertIgnor;
			}
		}
		return new Vertices();
	}             
	public Vertices ActivateRandomAdjacentVert(Vertices vert)
	{
		int dir = Random.Range(0, vert.degree);
		int indexTo = vert.edge[dir].indexTo;
		ActivateVert(indexTo);
		return GetVert(indexTo);
	}                                               
	public Vertices GetVertPath(int pathIndex)
	{
		//Get the vert via the path index
		if (pathIndex < 0 || pathIndex > pathVertAmount)
			return new Vertices();
		return vertsPath[pathIndex];
	}                                                                   
	public Vertices GetVert(int index)
	{
		//Get the vert via the Global index
		return verts[index];
	} //No checks															 
	public Vertices GetVert(Vector2 pos)
	{
		//Get the vert via a postion
		return verts[GetIndex((int)pos.x, (int)pos.y)];
	}   //No checks														 
	public Vertices GetRootVert()
	{
		//Get the first ever initilzed vert
		return verts[midIndex];
	}                                                                             
	public Vertices GetLastVert()
	{
		return lastVert;
	}
	public Vertices[] GetStatusVertsArr(bool activeState)
	{
		//Get an array of verts with the desired active status
		Vertices[] rVerts;
		int lastCount = 0;
		if (activeState)
			rVerts = new Vertices[activeVertAmount];
		else
			rVerts = new Vertices[RoadCountHeight * RoadCountWidth - activeVertAmount];

		for (int i = 0; i < activeVertAmount; i++)
		{
			for (int ii = lastCount; ii < RoadCountHeight * RoadCountWidth; ii++)
			{
				if (verts[ii].active == activeState)
				{
					rVerts[i] = verts[ii];
					lastCount = ii + 1;
					break;
				}
			}
		}
		return rVerts;
	}                                                    
	public Vector2[] GetPositionOfAllVerts()
	{
		uint size = RoadCountWidth * RoadCountHeight;
		Vector2[] temp = new Vector2[size];
		for(int i = 0; i < size; i++)
		{
			temp[i] = verts[i].pos;
		}
		return temp;
	}
	public Vector2[] GetPositionOfPathVerts()
	{
		Vector2[] temp = new Vector2[pathVertAmount];
		for (int i = 0; i < pathVertAmount; i++)
		{
			temp[i] = vertsPath[i].pos;
		}
		return temp;
	}
	public Vector2 GetRealativePosition(Vector2 origin, Vector2 other)
	{
		//Gets the position relative to orinign
		return other - origin;
	}                                        
	public Vector2 GetRealativePosition(Vertices origin, Vertices other)
	{
		return other.pos - origin.pos;
	}
	public Vector2 ResultantDirectionOfAdjacentPathVertices(int pathIndex)
	{
		if (pathIndex <= 0 || pathIndex >= pathVertAmount)
		{
			Debug.Log("Invalid PathIndex");
			return Vector2.zero;
		}
		Vector2 originVec = vertsPath[pathIndex].pos;
		Vector2 preVec = verts[pathIndex - 1].pos;
		Vector2 postVec = verts[pathIndex + 1].pos;
		Vector2 rel1 = GetRealativePosition(originVec, preVec);
		Debug.Log(rel1);
		Vector2 rel2 = GetRealativePosition(originVec, postVec);
		Debug.Log(rel2);
		return rel1 - rel2;
	}
	public Vector2 ResultantDirectionOfAdjacentPathVertices(Vector2 vec)
	{
		int pathIndex = GetPathIndex(vec);
		if (pathIndex <= 0 || pathIndex >= pathVertAmount)
		{
			Debug.Log("Invalid PathIndex");
			return Vector2.zero;
		}
		Vector2 originVec = vertsPath[pathIndex].pos;
		Vector2 preVec = verts[pathIndex - 1].pos;
		Vector2 postVec = verts[pathIndex + 1].pos;
		Vector2 rel1 = GetRealativePosition(originVec, preVec);
		Vector2 rel2 = GetRealativePosition(originVec, postVec);
		return rel1 + rel2;

	}
	public Vector2 ResultantDirectionOfAdjacentPathVertices(Vertices vert)
	{
		int pathIndex = vert.pathIndex;
		if (pathIndex <= 0 || pathIndex >= pathVertAmount)
		{
			Debug.Log("Invalid PathIndex");
			return Vector2.zero;
		}
		Vector2 originVec = vertsPath[pathIndex].pos;
		Vector2 preVec = verts[pathIndex - 1].pos;
		Vector2 postVec = verts[pathIndex + 1].pos;
		Vector2 rel1 = GetRealativePosition(originVec, preVec);
		Vector2 rel2 = GetRealativePosition(originVec, postVec);
		return rel1 + rel2;
	}
	public int[] GetStatusIntArr(bool activeState)
	{
		//Get an array of global index's of verts with the desired active status
		int[] arr;
		int lastCount = 0;
		if (activeState)
			arr = new int[activeVertAmount];
		else
			arr = new int[RoadCountHeight * RoadCountWidth - activeVertAmount];

		for (int i = 0; i < arr.Length; i++)
		{
			for (int ii = lastCount; ii < RoadCountHeight * RoadCountWidth; ii++)
			{
				if (verts[ii].active == activeState)
				{
					arr[i] = verts[ii].index;
					lastCount = ii + 1;
					break;
				}
			}
		}
		return arr;
	}                                                             
	public int[] GetIndexArr()
	{
		//Get an array of Global index's from the vertPath array of verts	
		int[] arr = new int[pathVertAmount];
		for (int i = 0; i < pathVertAmount; i++)
		{
			arr[i] = vertsPath[i].index;
		}
		return arr;
	}                                                                                				
	public uint GetRoadCountWidth()
	{
		return RoadCountWidth;
	}                                                                             
	public uint GetRoadCountHeight()
	{
		return RoadCountHeight;
	}                                                                           
	public uint GetPathLength()
	{
		return PathLength;
	}


}
