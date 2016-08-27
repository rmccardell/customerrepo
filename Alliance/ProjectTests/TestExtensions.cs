using NUnit.Framework;

namespace ProjectTests
{ 
   
    public class Assert : NUnit.Framework.Assert
    {
        public static void IsNotNullOrEmpty(object item)
        {
            Assert.That(item, Is.Not.Null.Or.Empty);
        }
        public static void IsNullOrEmpty(object item)
        {
            Assert.That(item, Is.Null.Or.Empty);
        }
    }
}
