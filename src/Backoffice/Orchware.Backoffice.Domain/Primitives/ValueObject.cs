namespace Orchware.Backoffice.Domain.Primitives
{
	public abstract class ValueObject : IEquatable<ValueObject>
	{
		public virtual bool Equals(ValueObject? other)
		=> other is not null && ValuesAreEqual(other);

		public override bool Equals(object? obj)
			=> obj is ValueObject valueObject && ValuesAreEqual(valueObject);

		public static bool operator ==(ValueObject? obj1, ValueObject? obj2)
		{
			if (ReferenceEquals(obj1, obj2)) return true;
			if (obj1 is null || obj2 is null) return false;
			return obj1.Equals(obj2);
		}

		public static bool operator !=(ValueObject? obj1, ValueObject? obj2)
			=> !(obj1 == obj2);

		protected abstract IEnumerable<object?> GetValues();

		private bool ValuesAreEqual(ValueObject other)
		{
			var thisValues = GetValues().GetEnumerator();
			var otherValues = other.GetValues().GetEnumerator();

			while (thisValues.MoveNext() && otherValues.MoveNext())
			{
				if (thisValues.Current is null ^ otherValues.Current is null)
					return false;

				if (thisValues.Current is not null &&
					!thisValues.Current.Equals(otherValues.Current))
					return false;
			}

			return !thisValues.MoveNext() && !otherValues.MoveNext();
		}

		public override int GetHashCode()
		{
			return GetValues()
				.Where(v => v is not null)
				.Aggregate(0, (hash, value) => HashCode.Combine(hash, value));
		}
	}
}
