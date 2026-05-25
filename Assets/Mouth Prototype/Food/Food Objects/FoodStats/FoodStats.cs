using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "FoodStats", menuName = "FoodStats")]
public class FoodStats : ScriptableObject
{
    public int id;
    public string name;
    public float scale;
    public float weight;
    public float value;
    public List<Effect> effects; // treat effects as queue or stack
    
}
