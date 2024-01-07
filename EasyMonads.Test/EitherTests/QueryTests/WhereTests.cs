using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.QueryTests
{
   [TestFixture]
   internal class WhereTests
   {
      [Test]
      public void Where_Works_For_Right_Either()
      {
         const string value = "test";
         Either<Unit, string> sut = value;

         Either<Unit, string> eitherRight = sut.Where(x => x == value);
         Assert.IsTrue(eitherRight.IsRight);
         eitherRight.DoRight(x => Assert.AreEqual(value, x));

         Either<Unit, string> eitherNeither = sut.Where(x => x == "foo");
         Assert.IsTrue(eitherNeither.IsNeither);
      }

      [Test]
      public void Where_Works_For_Left_Either()
      {
         const string value = "test";
         Either<string, int> sut = value;

         Either<string, int> eitherNeither = sut.Where(x => x == 3);
         Assert.IsTrue(eitherNeither.IsNeither);
      }

      [Test]
      public void Where_Works_For_Neither_Either()
      {
         Either<int, string> sut = Either<int, string>.Neither;

         Either<int, string> eitherNeither = sut.Where(x => x == "test");
         Assert.IsTrue(eitherNeither.IsNeither);
      }
   }
}