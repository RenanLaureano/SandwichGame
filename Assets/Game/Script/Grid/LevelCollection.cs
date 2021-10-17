using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelCollection", menuName = "SandwichGame/Level/LevelCollection", order = 0)]
public class LevelCollection : ScriptableObject
{
    public List<LevelData> levels;
}
