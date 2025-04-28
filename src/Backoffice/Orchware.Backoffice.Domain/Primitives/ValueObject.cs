namespace Orchware.Backoffice.Domain.Primitives
{
	public abstract class ValueObject : IEquatable<ValueObject>
	{
		public virtual bool Equals(ValueObject? other)
			=> other is not null && ValuesAreQueals(other);

		public override bool Equals(object? obj)
			=> obj is ValueObject valueObject && ValuesAreQueals(valueObject);

		public static bool operator ==(ValueObject obj1, ValueObject obj2)
		{
			if (obj1 is null && obj2 is null) return true;

			if (obj1 is null || obj2 is null) return false;

			return obj1.Equals(obj2);
		}

		public static bool operator !=(ValueObject obj1, ValueObject obj2)
		{
			return !(obj1 == obj2);
		}

		protected abstract IEnumerable<object> GetValues();

		private bool ValuesAreQueals(ValueObject obj)
			=> GetValues().SequenceEqual(obj.GetValues());

		public override int GetHashCode()
		{
			return GetValues()
				.Aggregate(0, (hashCode, value) => HashCode.Combine(hashCode, value));
		}
	}
}
