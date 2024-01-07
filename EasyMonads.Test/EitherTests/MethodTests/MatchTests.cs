using System.Threading.Tasks;
using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.MatchTests
{
   [TestFixture]
   internal class MatchTests
   {
      [Test]
      public async Task MatchAsync_Works_For_Left_Async()
      {
         Either<int, string> leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            _ => "right",
            "neither");

         Assert.AreEqual("left", leftSut);

         Either<int, string> rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            _ => "right",
            "neither");

         Assert.AreEqual("right", rightSut);

         Either<int, string> neitherEither = Either<int, string>.Neither;
         string neitherSut = await neitherEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            _ => "right",
            "neither");

         Assert.AreEqual("neither", neitherSut);
      }

      [Test]
      public async Task MatchAsync_Works_For_Right_Async()
      {
         Either<int, string> leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            _ => "left",
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("left", leftSut);

         Either<int, string> rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            _ => "left",
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("right", rightSut);

         Either<int, string> neitherEither = Either<int, string>.Neither;
         string neitherSut = await neitherEither.MatchAsync(
            _ => "left",
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("neither", neitherSut);
      }

      [Test]
      public async Task MatchAsync_Works_For_Left_And_Right_Async()
      {
         Either<int, string> leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("left", leftSut);

         Either<int, string> rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("right", rightSut);

         Either<int, string> neitherEither = Either<int, string>.Neither;
         string neitherSut = await neitherEither.MatchAsync(
            async _ => await Task.FromResult("left"),
            async _ => await Task.FromResult("right"),
            "neither");

         Assert.AreEqual("neither", neitherSut);
      }
   }
}