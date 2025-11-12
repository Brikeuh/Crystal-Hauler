using UnityEngine;

[CreateAssetMenu(fileName = "FloatScriptableObject", menuName = "Scriptable Objects/FloatScriptableObject")]
public class FloatScriptableObject : ScriptableObject
{
	[SerializeField] private float _value;

	public float Value
	{
		get { return _value; }
		set { _value = value; }
	}

}
