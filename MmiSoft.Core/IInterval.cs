using System;

namespace MmiSoft.Core
{
	public interface IInterval<T>
	{
		/// <summary>
		/// Checks if the value in the arguments is after the interval eg 5 is after [1,4]
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IsValueBefore(T value);

		/// <summary>
		/// Checks if the value in the arguments is before the interval eg -2 is before [1,4]
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IsValueAfter(T value);

		bool Contains(T value);
		T Closest(T value);
	}

	public abstract class InfiniteEndpointInterval<T> : IInterval<T> where T: IComparable<T>
	{
		public static readonly IInterval<T> Any = new Any<T>();
		public static readonly IInterval<T> None = new None<T>();

		private T endpoint;
		private string format;

		protected InfiniteEndpointInterval(T endpoint, string format)
		{
			this.endpoint = endpoint;
			this.format = format;
		}

		public T Endpoint => endpoint;

		public abstract bool IsValueBefore(T value);
		public abstract bool IsValueAfter(T value);
		public abstract bool Contains(T value);

		public virtual T Closest(T value)
		{
			return Contains(value) ? value : Endpoint;
		}

		public override string ToString()
		{
			return string.Format(format, endpoint);
		}
	}

	public class LessThanInterval<T> : InfiniteEndpointInterval<T> where T: IComparable<T>
	{
		public LessThanInterval(T endpoint, string format="(-\u221E,{0}]") : base(endpoint, format)
		{ }

		public override bool IsValueBefore(T value) => false;
		public override bool IsValueAfter(T value) => !Contains(value);

		public override bool Contains(T value)
		{
			return value.CompareTo(Endpoint) <= 0;
		}
	}

	public class GreaterThanInterval<T> : InfiniteEndpointInterval<T> where T: IComparable<T>
	{

		public GreaterThanInterval(T endpoint, string format="[{0},\u221E)") : base(endpoint, format)
		{ }

		public override bool IsValueBefore(T value) => !Contains(value);
		public override bool IsValueAfter(T value) => false;

		public override bool Contains(T value)
		{
			return value.CompareTo(Endpoint) >= 0;
		}
	}

	public class BoundedInterval<T> : IInterval<T> where T: IComparable<T>
	{
		private GreaterThanInterval<T> lower;
		private LessThanInterval<T> upper;
		private string format;

		public BoundedInterval(GreaterThanInterval<T> lower, LessThanInterval<T> upper, string format="[{0},{1}]")
		{
			this.lower = lower;
			this.upper = upper;
			this.format = format;
		}

		public BoundedInterval(T lower, T upper, string format="[{0},{1}]")
		{
			if (lower.CompareTo(upper) > 0) Util.Swap(ref lower, ref upper);
			this.lower = new GreaterThanInterval<T>(lower);
			this.upper = new LessThanInterval<T>(upper);
			this.format = format;
		}

		public bool IsValueBefore(T value) => lower.IsValueBefore(value);
		public bool IsValueAfter(T value) => upper.IsValueAfter(value);

		public bool Contains(T value)
		{
			return lower.Contains(value) && upper.Contains(value);
		}

		public T Closest(T value)
		{
			if (!lower.Contains(value)) return lower.Endpoint;
			if (!upper.Contains(value)) return upper.Endpoint;
			return value;
		}

		public override string ToString()
		{
			return string.Format(format, lower.Endpoint, upper.Endpoint);
		}
	}

	public class DegenerateInterval<T> : IInterval<T> where T: IComparable<T>
	{
		private T endpoint;
		private string format;

		public DegenerateInterval(T endpoint, string format="[{0},{0}]")
		{
			this.endpoint = endpoint;
			this.format = format;
		}

		public bool IsValueBefore(T value) => endpoint.CompareTo(value) < 0;
		public bool IsValueAfter(T value) => endpoint.CompareTo(value) > 0;

		public bool Contains(T value) => endpoint.CompareTo(value) == 0;

		public T Closest(T value) => endpoint;

		public override string ToString()
		{
			return string.Format(format, endpoint);
		}
	}

	/// <summary>
	/// Equivalent to (-∞,∞)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Any<T>: IInterval<T>
	{
		public bool IsValueBefore(T value) => false;

		public bool IsValueAfter(T value) => false;

		public bool Contains(T value) => true;

		public T Closest(T value) => value;
	}

	/// <summary>
	/// Equivalent to {∅}
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class None<T>: IInterval<T>
	{
		public bool IsValueBefore(T value) => true;

		public bool IsValueAfter(T value) => true;

		public bool Contains(T value) => false;

		public T Closest(T value) => default;
	}
}
