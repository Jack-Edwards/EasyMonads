using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.StaticTests
{
   [TestFixture]
   internal class ImplicitRightTests
   {
      [Test]
      public void Implicit_Right_Operator_Works()
      {
         const string value = "test";
         Either<Unit, string> sut = value;
         Assert.IsTrue(sut.IsRight);
      }
      
      [Test]
      public void Implicit_Right_Nullable_Operator_Works()
      {
         object? value = (object?)new object();
         Either<Unit, object> sut1 = value;
         Assert.IsTrue(sut1.IsRight);
      }

      [Test]
      public void Implicit_Right_Operator_Neithers_If_Null()
      {
         Either<Unit, string> sut = null;
         Assert.IsTrue(sut.IsNeither);
      }
      
      [Test]
      public void Implicit_Right_Nullable_Operator_Neithers_If_Null()
      {
         object? value = null;
         Either<Unit, object> sut1 = value;
         Assert.IsTrue(sut1.IsNeither);
      }
   }
}