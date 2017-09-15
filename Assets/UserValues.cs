using System.Collections.Generic;
using UnityEngine;

public class UserValues
{
	private int id;
	public int Id
	{
		get { return id; }
		set { id = value; }
	}

	private CredentialValues credentials;
	public CredentialValues Credentials
	{
		get { return credentials; }
		set { credentials = value; }
	}

}

