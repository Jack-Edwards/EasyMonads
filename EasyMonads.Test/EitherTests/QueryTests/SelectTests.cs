using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.QueryTests
{
   [TestFixture]
   internal class SelectTests
   {
      [Test]
      public void Select_Works_For_Right_Either()
      {
         const string value = "test";
         Either<Unit, string> sut = value;

         Either<Unit, string> eitherRightUppercase = sut.Select(x => x.ToUpper());
         Assert.IsTrue(eitherRightUppercase.IsRight);
         eitherRightUppercase.DoRight(x => Assert.AreEqual(value.ToUpper(), x));
      }

      [Test]
      public void Select_Works_For_Left_Either()
      {
         const string value = "test";
         Either<string, int> sut = value;

         Either<string, bool> eitherLeft = sut.Select(x => x == 5);
         Assert.IsTrue(eitherLeft.IsLeft);
         eitherLeft.DoLeftOrNeither(
            left => Assert.AreEqual(value, left),
            Assert.Fail);
      }

      [Test]
      public void Select_Works_For_Neither_Either()
      {
         Either<Unit, string> sut = Either<Unit, string>.Neither;

         Either<Unit, bool> eitherNeither = sut.Select(x => x == "foo");
         Assert.IsTrue(eitherNeither.IsNeither);
      }
   }
}