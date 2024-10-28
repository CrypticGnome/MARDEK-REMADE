using UnityEngine;

namespace MARDEK.Progress
{
    using Event;
    using Inventory;
    public class RemovePlotItem : Command
    {
        [SerializeField] PlotItem item;

        public override void Trigger()
        {
            Party.Instance.plotItems.Remove(item);
        }
    }
}