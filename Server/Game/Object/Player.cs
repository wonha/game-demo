using Google.Protobuf.Protocol;
using Server.DB;

namespace Server.Game
{
	public class Player : GameObject
	{
		public int PlayerDbId { get; set; }
		public ClientSession Session { get; set; }
		public Inventory Inven { get; private set; } = new Inventory();

		public int WeaponDamage { get; private set; }
		public int ArmorDefence { get; private set; }

		public override int TotalAttack { get { return Stat.Attack + WeaponDamage; } }
		public override int TotalDefence { get { return ArmorDefence; } }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

		public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}

		public void OnLeaveGame()
		{
			DbTransaction.SavePlayerStatus_Step1(this, Room);
		}

		public void HandleEquipItem(C_EquipItem equipPacket)
		{
			Item item = Inven.Get(equipPacket.ItemDbId);
			if (item == null)
				return;

			if (item.ItemType == ItemType.Consumable)
				return;

			if (equipPacket.Equipped)
			{
				Item unequipItem = null;

				if (item.ItemType == ItemType.Weapon)
				{
					unequipItem = Inven.Find(
						i => i.Equipped && i.ItemType == ItemType.Weapon);
				}
				else if (item.ItemType == ItemType.Armor)
				{
					ArmorType armorType = ((Armor)item).ArmorType;
					unequipItem = Inven.Find(
						i => i.Equipped && i.ItemType == ItemType.Armor
							&& ((Armor)i).ArmorType == armorType);
				}

				if (unequipItem != null)
				{
					unequipItem.Equipped = false;

					DbTransaction.EquipItemNoti(this, unequipItem);

					S_EquipItem equipOkItem = new S_EquipItem();
					equipOkItem.ItemDbId = unequipItem.ItemDbId;
					equipOkItem.Equipped = unequipItem.Equipped;
					Session.Send(equipOkItem);
				}
			}

			{
				item.Equipped = equipPacket.Equipped;

				DbTransaction.EquipItemNoti(this, item);

				S_EquipItem equipOkItem = new S_EquipItem();
				equipOkItem.ItemDbId = equipPacket.ItemDbId;
				equipOkItem.Equipped = equipPacket.Equipped;
				Session.Send(equipOkItem);
			}

			RefreshAdditionalStat();
		}

		public void RefreshAdditionalStat()
		{
			WeaponDamage = 0;
			ArmorDefence = 0;

			foreach (Item item in Inven.Items.Values)
			{
				if (item.Equipped == false)
					continue;

				switch (item.ItemType)
				{
					case ItemType.Weapon:
						WeaponDamage += ((Weapon)item).Damage;
						break;
					case ItemType.Armor:
						ArmorDefence += ((Armor)item).Defence;
						break;
				}
			}
		}
	}
}
