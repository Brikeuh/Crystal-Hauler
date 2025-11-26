using UnityEngine;

[CreateAssetMenu(fileName = "NewBoolValue", menuName = "ScriptableObjects/Bool Value")]
public class BoolValue : ScriptableObject
{
    public bool Value;

    public void SetValue(bool newValue)
    {
        Value = newValue;
    }

    public void ToggleValue()
    {
        Value = !Value;
    }
}
