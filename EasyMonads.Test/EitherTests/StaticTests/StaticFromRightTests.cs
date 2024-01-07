using System.Threading.Tasks;
using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.StaticTests
{
   [TestFixture]
   internal class StaticFromRightTests
   {
      [Test]
      public void FromRight_Works()
      {
         const string value = "test";
         Either<Unit, string> sut = Either<Unit, string>.FromRight(value);
         Assert.IsTrue(sut.IsRight);
      }

      [Test]
      public void FromRightNullable_Returns_Neither_If_Null_Provided()
      {
         string? value = null;
         Either<Unit, string> sut = Either<Unit, string>.FromRightNullable(value);
         Assert.IsTrue(sut.IsNeither);
      }
      
      [Test]
      public async Task FromRightAsync_Works()
      {
         const string value = "test";
         Task<string> task = Task.FromResult(value);

         Task<Either<Unit, string>> eitherTask = Either<Unit, string>.FromRightAsync(task);
         Either<Unit, string> sut = await eitherTask;
         Assert.IsTrue(sut.IsRight);
      }

      [Test]
      public async Task FromRightAsync_Neithers_If_Null()
      {
         Task<string?> task = Task.FromResult((string?)null);

         Task<Either<Unit, string>> eitherTask = Either<Unit, string>.FromRightNullableAsync(task);
         Either<Unit, string> sut = await eitherTask;
         Assert.IsTrue(sut.IsNeither);
      }
      
      [Test]
      public async Task FromRightAsync_Works_With_Fallback_Left()
      {
         Task<string> task = Task.FromResult("foo");

         Task<Either<int, string>> eitherTask = Either<int, string>.FromRightAsync(task, 3);
         Either<int, string> sut = await eitherTask;
         Assert.IsTrue(sut.IsRight);
      }
      
      [Test]
      public async Task FromRightNullableAsync_Works_With_Fallback_Left()
      {
         const int fallbackLeft = 3;
         Task<string?> task = Task.FromResult((string?)null);

         Task<Either<int, string>> eitherTask = Either<int, string>.FromRightNullableAsync(task, fallbackLeft);
         Either<int, string> sut = await eitherTask;
         Assert.IsTrue(sut.IsLeft);
         Assert.AreEqual(sut.LeftOrDefault(4), fallbackLeft);
      }
   }
}