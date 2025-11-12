using UnityEngine;

[CreateAssetMenu(fileName = "BoolScriptableObject", menuName = "Scriptable Objects/BoolScriptableObject")]
public class BoolScriptableObject : ScriptableObject
{
    [SerializeField]
    private bool _value;

	public bool Value
	{
		get { return Value; }
		set { Value = value; }
	}

}
