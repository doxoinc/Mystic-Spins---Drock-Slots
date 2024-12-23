using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolDataList", menuName = "MagicSlots/SymbolDataList")]
public class SymbolDataList : ScriptableObject
{
    public List<SymbolData> symbols = new List<SymbolData>();
}
