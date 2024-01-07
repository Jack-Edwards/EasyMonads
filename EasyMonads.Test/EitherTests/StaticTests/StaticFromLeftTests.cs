using System.Threading.Tasks;
using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.StaticTests
{
   [TestFixture]
   internal class StaticFromLeftTests
   {
      [Test]
      public void FromLeft_Works()
      {
         const string value = "test";
         Either<string, Unit> sut = Either<string, Unit>.FromLeft(value);
         Assert.IsTrue(sut.IsLeft);
      }

      [Test]
      public void FromLeftNullable_Returns_Neither_If_Null_Provided()
      {
         string? value = null;
         Either<string, Unit> sut = Either<string, Unit>.FromLeftNullable(value);
         Assert.IsTrue(sut.IsNeither);
      }
      
      [Test]
      public async Task FromLeftAsync_Works()
      {
         const string value = "test";
         Task<string> task = Task.FromResult(value);

         Task<Either<string, Unit>> eitherTask = Either<string, Unit>.FromLeftAsync(task);
         Either<string, Unit> sut = await eitherTask;
         Assert.IsTrue(sut.IsLeft);
      }
      
      [Test]
      public async Task FromLeftAsync_Neithers_If_Null()
      {
         Task<string?> task = Task.FromResult((string?)null);

         Task<Either<string, Unit>> eitherTask = Either<string, Unit>.FromLeftNullableAsync(task);
         Either<string, Unit> sut = await eitherTask;
         Assert.IsTrue(sut.IsNeither);
      }
   }
}