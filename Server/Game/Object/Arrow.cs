using Google.Protobuf.Protocol;

namespace Server.Game
{
	public class Arrow : Projectile
	{
		public GameObject Owner { get; set; }

		public override void Update()
		{
			if (Data == null || Data.projectile == null || Owner == null || Room == null)
				return;

			int tick = (int)(1000 / Data.projectile.speed);
			Room.PushAfter(tick, Update);

			Vector2Int destPos = GetFrontCellPos();
			if (Room.Map.CanGo(destPos))
			{
				CellPos = destPos;

				S_Move movePacket = new S_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;
				Room.Broadcast(movePacket);

			}
			else
			{
				GameObject target = Room.Map.Find(destPos);
				if (target != null)
				{
					target.OnDamaged(this, Data.damage + Owner.TotalAttack);
				}

				Room.Push(Room.LeaveGame, Id);
			}
		}

		public override GameObject GetOwner()
		{
			return Owner;
		}
	}
}
