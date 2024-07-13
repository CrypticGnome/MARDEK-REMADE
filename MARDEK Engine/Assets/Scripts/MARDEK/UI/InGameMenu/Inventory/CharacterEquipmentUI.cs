namespace MARDEK.UI
{
    using Progress;
    using CharacterSystem;
     using System;

     public class CharacterEquipmentUI : InventoryUI
    {
        public Character Character
        {
            get
            {
                var index = transform.GetSiblingIndex();
                if (Party.Instance.Characters.Count <= index)
                    return null;
                return Party.Instance.Characters[index];
            }
        }
        private void OnEnable()
        {
            var index = transform.GetSiblingIndex();
               if (Character != null)
                    throw new NotImplementedException();
                //AssignInventoryToUI(Character.ItemsEquipped);
        }
    }
}