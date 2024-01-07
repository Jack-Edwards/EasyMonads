using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.StaticTests
{
   [TestFixture]
   internal class StaticNeitherTests
   {
      [Test]
      public void Static_Neither_Returns_Neither()
      {
         Either<Unit, string> sut = Either<Unit, string>.Neither;
         Assert.IsTrue(sut.IsNeither);
      }
   }
}