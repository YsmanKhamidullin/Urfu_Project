using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    [SerializeField]
    private float _min, _max;

    public float Min { get { return _min; } }
    public float Max { get { return _max;} }

    public float RandomValueInRange
    {
        get
        {
            return Random.Range(this._min, this._max);
        }
    }

    public FloatRange(float value)
    {
        this._min = this._max = value;
    }

    public FloatRange(float minValue, float maxValue)
    {
        this._min = minValue;
        this._max = maxValue;
    }
}

public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }
}
