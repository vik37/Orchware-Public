namespace Orchware.Backoffice.Domain.Primitives
{
	public abstract class Entity<TID> : IEquatable<Entity<TID>>
	{
		public TID Id { get; private set; }

		protected Entity()
		{}

		public override bool Equals(Object obj)
		{
			if(obj is not Entity<TID> other) return false;

			if(ReferenceEquals(this, other)) return true;

			if(obj.GetType() != typeof(TID)) return false;

			return Id?.Equals(other.Id) ?? false;
		}

		public bool Equals(Entity<TID> other)
		{
			return Equals((object)other);
		}

		public static bool operator ==(Entity<TID> obj1, Entity<TID> obj2)
		{
			if (obj1 is null && obj2 is null) return true;

			if (obj1 is null || obj2 is null) return false;

			return obj1.Equals(obj2);
		}

		public static bool operator !=(Entity<TID> obj1, Entity<TID> obj2)
		{
			return !(obj1 == obj2);
		}

		public override int GetHashCode()
			=> Id?.GetHashCode() ?? 0;
	}
}
