using MiCake.Core.Util.Reflection;
using System.Collections.Generic;

namespace MiCake.Core.Tests.Util
{
    public class TypeHelper_Tests
    {
        public TypeHelper_Tests()
        {

        }

        [Fact]
        public void IsInstantiable_Test()
        {
            List<string> s = new();
            var r1 = TypeHelper.IsInstantiable(s.GetType());

            Assert.True(r1);

            Base<string, string> s1 = new();
            var r2 = TypeHelper.IsInstantiable(s1.GetType());

            Assert.True(r2);
        }
    }

    public class Base<T, U> { }

    public class Derived<V> : Base<string, V>
    {
        public G<Derived<V>> F;

        public class Nested { }
    }

    public class G<T> { }
}
