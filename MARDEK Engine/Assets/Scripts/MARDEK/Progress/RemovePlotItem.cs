using UnityEngine;

namespace MARDEK.Progress
{
    using Event;
    using Inventory;
    public class RemovePlotItem : Command
    {
        [SerializeField] PlotItem item;
        [SerializeField] PlotItems plotItems;
        public override void Trigger()
        {
            plotItems.Remove(item);
        }
    }
}