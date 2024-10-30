using UnityEngine;

namespace MARDEK.Progress
{
    using Inventory;
    public class AddPlotItem : Event.Command
    {
        [SerializeField] PlotItem item;

        public override void Trigger()
        {
            Party.Instance.plotItems.Add(item);
        }
    }
}