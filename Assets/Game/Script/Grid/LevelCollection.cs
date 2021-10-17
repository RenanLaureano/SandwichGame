using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelCollection", menuName = "SandwichGame/LevelCollection", order = 1)]
public class LevelCollection : ScriptableObject
{
    public List<LevelData> levels;
}
