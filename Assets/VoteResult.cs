using System.Collections.Generic;
using UnityEngine;

public class VoteResult
{
	private string message;
	public string Message
	{
		get { return message; }
		set { message = value; }
	}

	private int realValue;
	public int RealValue
	{
		get { return realValue; }
		set { realValue = value; }
	}

	private int score;
	public int Score
	{
		get { return score; }
		set { score = value; }
	}

}

