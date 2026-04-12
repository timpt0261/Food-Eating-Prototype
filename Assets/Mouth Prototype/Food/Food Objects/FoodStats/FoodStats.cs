using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodStats", menuName = "FoodStats")]
[Serializable]
public class FoodStats : ScriptableObject
{
    public int id;
    public string name;
    public float scale;
    public float weight; 
    public List<FoodEffect> effects;
    
}
