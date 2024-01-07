using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.StaticTests
{
   [TestFixture]
   internal class ImplicitLeftTests
   {
      [Test]
      public void Implicit_Left_Operator_Works()
      {
         const int value = 5;
         Either<int, Unit> sut = value;
         Assert.IsTrue(sut.IsLeft);
      }
      
      [Test]
      public void Implicit_Left_Nullable_Operator_Works()
      {
         object? value = (object?)new object();
         Either<object, Unit> sut1 = value;
         Assert.IsTrue(sut1.IsLeft);
      }

      [Test]
      public void Implicit_Left_Operator_Neithers_If_Null()
      {
         Either<string, Unit> sut = null;
         Assert.IsTrue(sut.IsNeither);
      }
      
      [Test]
      public void Implicit_Left_Nullable_Operator_Neithers_If_Null()
      {
         object? value = null;
         Either<object, Unit> sut1 = value;
         Assert.IsTrue(sut1.IsNeither);
      }
   }
}