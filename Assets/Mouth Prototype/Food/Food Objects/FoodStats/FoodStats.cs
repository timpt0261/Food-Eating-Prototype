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
    public List<FoodEffect> effects;
    
}
