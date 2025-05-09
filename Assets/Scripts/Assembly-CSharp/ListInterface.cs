using UnityEngine;

public class ListInterface : MonoBehaviour
{
	public virtual object[] ListAccess
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public virtual void AddRow()
	{
	}

	public virtual void Duplicate(int index)
	{
	}

	public virtual void Remove(int index)
	{
	}

	public virtual int? GetColumnWidth(string name)
	{
		return null;
	}
}
