using UnityEngine;

namespace MARDEK.Progress
{
    using Inventory;
    public class AddPlotItem : Event.Command
    {
          [SerializeField] PlotItem item;
          [SerializeField] PlotItems plotItems;

        public override void Trigger()
        {
            plotItems.Add(item);
        }
    }
}