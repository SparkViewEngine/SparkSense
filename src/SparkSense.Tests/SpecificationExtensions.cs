using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace SparkSense.Tests
{
    public static class SpecificationExtensions
    {
        public static void ShouldBeTrue(this bool value)
        {
            Assert.That(value);
        }

        public static T ShouldNotBeNull<T>(this T value) where T : class
        {
            Assert.IsNotNull(value);
            return value;
        }

        public static void ShouldBe<T>(this T actualValue, T expectedValue)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        public static T ShouldBeOfType<T>(this object value) where T : class
        {
            Assert.That(value.GetType(), Is.EqualTo(typeof (T)));
            return value as T;
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> values, int count)
        {
            Assert.AreEqual(count, values.Count());
            return values;
        }
    }
}