using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEngine;


public class GameTimeManager : MonoSingelton<GameTimeManager>
{
    public delegate void OnTimeScaleChanged(float newTimeScale, float oldTimeScale);
    public OnTimeScaleChanged onTimeScaleChanged;

    float currentTimeMultiplier = 1f;

    Dictionary<string, float> timeManipulators = new Dictionary<string, float>();

    public void ClearAllTimeManipulations()
    {
        timeManipulators.Clear();

        CurrentTimeMultiplier = 1f;
    }

    public float CurrentTimeMultiplier
    {
        get { return currentTimeMultiplier; }
        private set
        {
            if (currentTimeMultiplier != value)
            {
                float oldValue = currentTimeMultiplier;
                currentTimeMultiplier = value;
                Time.timeScale = currentTimeMultiplier;

                if (onTimeScaleChanged != null) onTimeScaleChanged(currentTimeMultiplier, oldValue);
            }
        }
    }

    void Update()
    {
        
    }

    public void ToggleTimeManipulation(string name, float timeMultiplier)
    {
        if (timeManipulators.ContainsKey(name))
        {
            var manipulation = timeManipulators[name];
            timeManipulators.Remove(name);
            RemoveManipulation(manipulation);

        }
        else
        {
            timeManipulators.Add(name, timeMultiplier);
            AddManipulation(timeMultiplier);
        }
    }

    void AddManipulation(float manipulation)
    {
        CurrentTimeMultiplier *= manipulation;
    }

    void RemoveManipulation(float manipulation)
    {
        if (manipulation == 0)
        {
            CurrentTimeMultiplier = 1f;
            return;
        }
        CurrentTimeMultiplier /= manipulation;
    }
}
