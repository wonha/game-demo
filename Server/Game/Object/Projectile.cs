using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
	public class Projectile : GameObject
	{
		public Skill Data { get; set; }

		public Projectile()
		{
			ObjectType = GameObjectType.Projectile;
		}

		public override void Update()
		{

		}
	}
}
