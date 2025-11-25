using UnityEngine;

[CreateAssetMenu(fileName = "IntScriptableObject", menuName = "Scriptable Objects/IntScriptableObject")]
public class IntScriptableObject : ScriptableObject
{
    [SerializeField] private int _value;

	public int Value
	{
		get { return _value; }
		set { _value = value; }
	}
}
