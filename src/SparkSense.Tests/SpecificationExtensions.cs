using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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

        public static T ShouldBe<T>(this T actualValue, T expectedValue)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue));
            return actualValue;
        }

        public static T ShouldBeOfType<T>(this object value) where T : class
        {
            Assert.That(value.GetType(), Is.EqualTo(typeof(T)));
            return value as T;
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> values, int count)
        {
            Assert.AreEqual(count, values.Count());
            return values;
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> values, int count, Func<T, bool> predicate)
        {
            ShouldHaveCount(values.Where(predicate), count);
            return values;
        }

        public static T ShouldContain<T>(this IEnumerable<T> values, Func<T, bool> predicate)
        {
            var found = values.FirstOrDefault(predicate);
            Assert.IsNotNull(found, string.Format("Expected item was not found in the values supplied."));
            return found;
        }

        public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> values, Func<T, bool> predicate)
        {
            var found = values.FirstOrDefault(predicate);
            Assert.IsNull(found, "Unexpected item was found in the values supplied.");
            return values;
        }
    }
}