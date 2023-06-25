using NUnit.Framework;
using System.Threading.Tasks;

namespace EasyMonads.Test
{
   [TestFixture]
   internal class EitherTests
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

      [Test]
      public void StaticNeither_Works()
      {
         Either<Unit, string> sut = Either<Unit, string>.Neither;

         sut.DoLeftOrNeither(
            _ => Assert.Fail(),
            () => Assert.True(true));
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

      [Test]
      public void FromRight_Works()
      {
         string value = "test";
         bool doRightInvoked = false;

         Either<Unit, string> sut = Either<Unit, string>.FromRight(value);
         sut.DoLeftOrNeither(Assert.Fail);
         sut.DoRight(right =>
         {
            doRightInvoked = true;
            Assert.AreEqual(value, right);
         });

         Assert.IsTrue(doRightInvoked);

         Assert.IsTrue(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
         Assert.IsFalse(sut.IsNeither);

         Assert.AreEqual(value, sut.RightOrDefault("bar"));
         Assert.AreEqual(Unit.Default, sut.LeftOrDefault(Unit.Default));
      }

      [Test]
      public void FromRight_Returns_Neither_If_Null_Provided()
      {
         Either<Unit, string> sut = Either<Unit, string>.FromRight(null);
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

         Assert.AreEqual("bar", sut.RightOrDefault("bar"));
         Assert.AreEqual(Unit.Default, sut.LeftOrDefault(Unit.Default));
      }

      [Test]
      public void FromLeft_Works()
      {
         int value = 5;
         bool doLeftInvoked = false;

         Either<int, Unit> sut = Either<int, Unit>.FromLeft(value);
         sut.DoLeftOrNeither(left =>
         {
            doLeftInvoked = true;
            Assert.AreEqual(value, left);
         },
         Assert.Fail);

         sut.DoRight(_ => Assert.Fail());

         Assert.IsTrue(doLeftInvoked);

         Assert.IsTrue(sut.IsLeft);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsNeither);

         Assert.AreEqual(5, sut.LeftOrDefault(123));
         Assert.AreEqual(Unit.Default, sut.RightOrDefault(Unit.Default));
      }

      [Test]
      public void FromLeft_Returns_Neither_If_Null_Provided()
      {
         Either<object, Unit> sut = Either<object, Unit>.FromLeft(null);
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

         object testObject = new object();
         Assert.AreEqual(testObject, sut.LeftOrDefault(testObject));
         Assert.AreEqual(Unit.Default, sut.RightOrDefault(Unit.Default));
      }

      [Test]
      public async Task FromRightAsync_Works()
      {
         string value = "test";
         Task<string> task = Task.FromResult(value);

         Task<Either<Unit, string>> eitherTask = Either<Unit, string>.FromRightAsync(task);
         Either<Unit, string> sut = await eitherTask;

         bool doRightInvoked = false;
         sut.DoLeftOrNeither(Assert.Fail);
         sut.DoRight(right =>
         {
            doRightInvoked = true;
            Assert.AreEqual(value, right);
         });

         Assert.IsTrue(doRightInvoked);

         Assert.IsTrue(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
         Assert.IsFalse(sut.IsNeither);

         Assert.AreEqual(value, sut.RightOrDefault("bar"));
         Assert.AreEqual(Unit.Default, sut.LeftOrDefault(Unit.Default));
      }

      [Test]
      public async Task FromRightAsync_Neithers_If_Null()
      {
         Task<string> task = Task.FromResult((string)null);

         Task<Either<Unit, string>> eitherTask = Either<Unit, string>.FromRightAsync(task);
         Either<Unit, string> sut = await eitherTask;

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

      [Test]
      public async Task FromRightAsync_Works_With_Default_Left()
      {
         Task<string> task = Task.FromResult((string)null);

         Task<Either<int, string>> eitherTask = Either<int, string>.FromRightAsync(task, 3);
         Either<int, string> sut = await eitherTask;

         bool doLeftInvoked = true;
         sut.DoLeftOrNeither(left =>
         {
            doLeftInvoked = true;
            Assert.AreEqual(3, left);
         },
         Assert.Fail);

         sut.DoRight(_ => Assert.Fail());

         Assert.IsTrue(doLeftInvoked);

         Assert.IsTrue(sut.IsLeft);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsNeither);

         Assert.AreEqual(3, sut.LeftOrDefault(123));
         Assert.AreEqual("three", sut.RightOrDefault("three"));
      }

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

      [Test]
      public void Implicit_Right_Operator_Works()
      {
         const string value = "test";
         Either<Unit, string> sut = value;
         Assert.IsTrue(sut.IsRight);
      }

      [Test]
      public void Implicit_Right_Operator_Neithers_If_Null()
      {
         Either<Unit, string> sut = null;
         Assert.IsTrue(sut.IsNeither);
      }

      [Test]
      public void Implicit_Left_Operator_Works()
      {
         const int value = 5;
         Either<int, Unit> sut = value;
         Assert.IsTrue(sut.IsLeft);
      }

      [Test]
      public void Implicit_Left_Operator_Neithers_If_Null()
      {
         Either<string, Unit> sut = null;
         Assert.IsTrue(sut.IsNeither);
      }
   }
}
