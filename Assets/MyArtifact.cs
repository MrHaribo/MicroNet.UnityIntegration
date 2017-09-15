using System.Collections.Generic;
using UnityEngine;

public class MyArtifact
{
	private int myInt;
	public int MyInt
	{
		get { return myInt; }
		set { myInt = value; }
	}

	private List<string> myList;
	public List<string> MyList
	{
		get { return myList; }
		set { myList = value; }
	}

	private Dictionary<string,int> myMap;
	public Dictionary<string,int> MyMap
	{
		get { return myMap; }
		set { myMap = value; }
	}

	private Vector2 myVector2;
	public Vector2 MyVector2
	{
		get { return MyVector2; }
		set { MyVector2 = value; }
	}

}

