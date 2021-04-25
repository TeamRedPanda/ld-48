using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipable Item List")]
public class EquipableItemList : ScriptableObject
{
    public List<GameObject> Prefabs;

    public int FindIndex(GameObject prefab)
    {
        return Prefabs.FindIndex(go => go == prefab);
    }
}