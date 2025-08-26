using MARDEK.Core;
using MARDEK.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlotItems", menuName = "Scriptable Objects/PlotItems")]
public class PlotItems : AddressableScriptableObject, IEnumerable<PlotItem>
{
     public HashSet<PlotItem> Items;
     public void Add(PlotItem item) => Items.Add(item);

     public IEnumerator<PlotItem> GetEnumerator() => Items.GetEnumerator();


     public void Remove(PlotItem item) => Items.Remove(item);

     IEnumerator IEnumerable.GetEnumerator()
     {
          return GetEnumerator();
     }
}
