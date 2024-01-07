using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.ConstructorTests
{
   [TestFixture]
   internal class DefaultConstructorTests
   {
      [Test]
      public void Default_Constructor_Returns_Neither()
      {
         Either<Unit, string> sut = new Either<Unit, string>();

         sut.DoLeftOrNeither(
            _ => Assert.Fail(),
            () => Assert.IsTrue(true));
         sut.DoRight(_ => Assert.Fail());

         bool isNeither = sut.Match(
            left: _ => false,
            right: _ => false,
            neither: true);

         Assert.IsTrue(isNeither);

         Assert.IsTrue(sut.IsNeither);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
      }
   }
}