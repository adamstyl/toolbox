using System;
using System.Globalization;
using NUnit.Framework;

namespace MmiSoft.Core.Test
{
	[TestFixture]
	public class PercentTest
	{
		[Test]
		public void DivideOp_2DividedBy8PerCent_Equals25()
		{
			double expectedD = 25;
			double expectedM = 25;
			Percent testValue = new Percent(8);
			Assert.AreEqual(expectedD, 2 / testValue, 0.0000000000001);
			Assert.AreEqual(expectedD, 2d / testValue, 0.000001);
			Assert.AreEqual(expectedM, 2m / testValue);
		}

		[Test]
		public void DivideOp_2DividedByZeroPerCent_EqualsInfinity()
		{
			Assert.AreEqual(double.PositiveInfinity, 2 / Percent.Zero);
			Assert.AreEqual(double.PositiveInfinity, 2d / Percent.Zero);
			Assert.Throws<DivideByZeroException>(() =>
			{
				var _ = 2m / Percent.Zero;
			});
		}

		[Test]
		public void Parser()
		{
			CultureInfo culture = CultureInfo.CurrentCulture;
			string dot = culture.NumberFormat.PercentDecimalSeparator;
			Assert.AreEqual(new Percent(2.35), Percent.Parse($"2{dot}35%", culture));
			Assert.AreEqual(new Percent(20), Percent.Parse("20%", culture));

			Assert.Throws<FormatException>(() => Percent.Parse($"{dot}5%", culture));
			Assert.Throws<FormatException>(() => Percent.Parse($"7{dot}%", culture));
		}

		[Test]
		public void EqualityOperator()
		{
			Assert.IsTrue(new Percent(10.0000000002) == new Percent(10));
			Assert.IsTrue(new Percent(10.002) != new Percent(10));
		}

		[Test]
		public void ComparisonOperators()
		{
			Assert.IsTrue(new Percent(25) < new Percent(50));
			Assert.IsTrue(new Percent(7) > new Percent(-10));
		}

		[Test]
		public void PlusOperators()
		{
			Assert.AreEqual(13, 10 + new Percent(30), 0.000001f);
			Assert.AreEqual(8.625, 7.5 + new Percent(15), 0.000001f);
		}

		[Test]
		public void FromDecimal_ConvertsCoefficientToPercentage()
		{
			Assert.AreEqual(new Percent(30), Percent.FromDecimal(1.3));
		}
	}
}
