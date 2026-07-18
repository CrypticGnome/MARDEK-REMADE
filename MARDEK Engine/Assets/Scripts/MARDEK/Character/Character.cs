using UnityEngine;

namespace MARDEK.CharacterSystem
{
	using System;
	using Inventory;
	using Stats;

	[CreateAssetMenu(menuName = "MARDEK/Character/Character")]
	public class Character : ScriptableObject
	{
		[SerializeField] public bool isRequired;
		[field: SerializeField] public CharacterProfile Profile { get; private set; }
		[field: SerializeField] public EquippedItems ItemsEquipped { get; private set; }
		[field: SerializeField] public Inventory Inventory { get; private set; }
		public CoreStats BaseStats { get { return Profile.Stats; } }
		[field: SerializeField] public ActionSkillset ActionSkillset { get; private set; }
		public delegate void StatChanged();
		public event StatChanged OnStatChanged;
		public Character()
		{
			Inventory = new Inventory();
		}

		[SerializeField] int attack;
		public int Attack
		{
			get
			{
				attack = BaseStats.Attack;
				for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
				{
					EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
					if (item is null)
						continue;
					attack += item.Stats.Attack;
				}
				return attack;
			}
		}

		[SerializeField] int defense;
		public int Defense
		{
			get
			{
				defense = BaseStats.Defense;
				for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
				{
					EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
					if (item is null)
						continue;
					defense += item.Stats.Defense;
				}
				return defense;
			}
		}
		[SerializeField] int magicDefense;
		public int MagicDefense
		{
			get
			{
				magicDefense = BaseStats.MagicDefense;
				for (int itemIndex = 0; itemIndex < EquippedItems.Count; itemIndex++)
				{
					EquippableItem item = ItemsEquipped.Slots[itemIndex].item as EquippableItem;
					if (item is null)
						continue;
					magicDefense += item.Stats.MagicDefense;
				}
				return magicDefense;
			}
		}
		[SerializeField] int _currentHP;
		public int CurrentHP
		{
			get
			{
				if (_currentHP == -1 || _currentHP > MaxHP)
				{
					_currentHP = MaxHP;
				}
				return _currentHP;
			}
			set
			{
				_currentHP = value;
				OnStatChanged?.Invoke();
			}
		}

		[SerializeField] int _currentMP;
		public int CurrentMP
		{
			get
			{
				if (_currentMP == -1 || _currentMP > MaxMP)
					_currentMP = MaxMP;
				return _currentMP;
			}
			set
			{
				_currentMP = value;
				OnStatChanged?.Invoke();
			}
		}
		public int MaxHP { get { return BaseStats.GetMaxHP(this); } }
		public int MaxMP { get { return BaseStats.GetMaxMP(this); } }


		public Absorbtions Absorbtions => BaseStats.Absorbtions;
		public StatusEffects Resistances => BaseStats.Resistances;
		public int Agility => BaseStats.Agility;
		public int Accuracy => BaseStats.Accuracy;
		public int CritRate => BaseStats.CritRate;
		public int Strength => BaseStats.Strength;
		public int Vitality => BaseStats.Vitality;
		public int Spirit => BaseStats.Spirit;
		public StatusEffects StatusBuildup;
		public int Level;
		public int Experience;
		// Virtual so CharacterUnplayable can return a clone that's still a
		// CharacterUnplayable, carrying over its own fields (Drops, ExperienceReward) -
		// otherwise EncounterSet.InstantiateEncounter's clone would silently downgrade to a
		// plain Character and lose them.
		public virtual Character Clone(int level)
		{
			var clone = CreateInstance<Character>();
			CopyBaseFieldsTo(clone, level);
			return clone;
		}

		protected void CopyBaseFieldsTo(Character clone, int level)
		{
			clone.Profile = Profile;
			clone.Level = level;
			clone.ActionSkillset = Profile.LearnableSkillset;
		}
		public void TickStatusEffects()
		{
			if (StatusBuildup.Poison > 0)
			{
				Debug.Log($"{Profile.displayName} is poisoned");
				StatusBuildup.Poison -= Resistances.Poison + 1;
				CurrentHP -= Mathf.RoundToInt((float)MaxHP / 20 + 0.5f);
			}
		}

		[Serializable]
		public class EquippedItems
		{
			public InventorySlot MainHand, OffHand, Head, Body, Accessory1, Accessory2;
			public InventorySlot[] Slots { get { return new InventorySlot[] { MainHand, OffHand, Head, Body, Accessory1, Accessory2 }; } }
			public static int Count = 6;
		}
	}
}