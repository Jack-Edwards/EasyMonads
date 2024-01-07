using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.ConstructorTests
{
   [TestFixture]
   internal class LeftConstructorTests
   {
      [Test]
      public void Left_Constructor_Returns_Left()
      {
         const string value = "foo";
         Either<string, Unit> sut = new Either<string, Unit>(value);
         Assert.IsTrue(sut.IsLeft);
         Assert.That(sut.LeftOrDefault("not_foo"), Is.EqualTo(value));
      }

      [Test]
      public void Left_Constructor_Returns_Neither_When_Null()
      {
         string? value = null;
         Either<string, Unit> sut = new Either<string, Unit>(value);
         Assert.IsTrue(sut.IsNeither);
      }
   }
}