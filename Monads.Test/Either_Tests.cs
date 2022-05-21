using NUnit.Framework;
using System.Threading.Tasks;

namespace Monads.Test
{
   [TestFixture]
   public class Either_Tests
   {
      [Test]
      public void Default_Constructor_Returns_Bottom()
      {
         var sut = new Either<Unit, string>();

         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(_ => Assert.Fail());

         bool isBottom = sut.Match(
            left: _ => false,
            right: _ => false,
            bottom: () => true);

         Assert.IsTrue(isBottom);

         Assert.IsTrue(sut.IsBottom);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
      }

      [Test]
      public void FromBottom_Works()
      {
         var sut = Either<Unit, string>.FromBottom();

         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(_ => Assert.Fail());

         bool isBottom = sut.Match(
            left: _ => false,
            right: _ => false,
            bottom: () => true);

         Assert.IsTrue(isBottom);

         Assert.IsTrue(sut.IsBottom);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
      }

      [Test]
      public void FromRight_Works()
      {
         string value = "test";
         bool doRightInvoked = false;

         var sut = Either<Unit, string>.FromRight(value);
         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(right =>
         {
            doRightInvoked = true;
            Assert.AreEqual(value, right);
         });

         Assert.IsTrue(doRightInvoked);

         Assert.IsTrue(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
         Assert.IsFalse(sut.IsBottom);

         Assert.AreEqual(value, sut.RightOrDefault("bar"));
         Assert.AreEqual(Unit.Default, sut.LeftOrDefault(Unit.Default));
      }

      [Test]
      public void FromRight_Returns_Bottom_If_Null_Provided()
      {
         var sut = Either<Unit, string>.FromRight(null);
         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(_ => Assert.Fail());

         bool isBottom = sut.Match(
            left: _ => false,
            right: _ => false,
            bottom: () => true);

         Assert.IsTrue(isBottom);

         Assert.IsTrue(sut.IsBottom);
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

         var sut = Either<int, Unit>.FromLeft(value);
         sut.DoLeft(left =>
         {
            doLeftInvoked = true;
            Assert.AreEqual(value, left);
         });
         sut.DoRight(_ => Assert.Fail());

         Assert.IsTrue(doLeftInvoked);

         Assert.IsTrue(sut.IsLeft);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsBottom);

         Assert.AreEqual(5, sut.LeftOrDefault(123));
         Assert.AreEqual(Unit.Default, sut.RightOrDefault(Unit.Default));
      }

      [Test]
      public void FromLeft_Returns_Bottom_If_Null_Provided()
      {
         var sut = Either<object, Unit>.FromLeft(null);
         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(_ => Assert.Fail());

         bool isBottom = sut.Match(
            left: _ => false,
            right: _ => false,
            bottom: () => true);

         Assert.IsTrue(isBottom);

         Assert.IsTrue(sut.IsBottom);
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

         var eitherTask = Either<Unit, string>.FromRightAsync(task);
         var sut = await eitherTask;

         bool doRightInvoked = false;
         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(right =>
         {
            doRightInvoked = true;
            Assert.AreEqual(value, right);
         });

         Assert.IsTrue(doRightInvoked);

         Assert.IsTrue(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
         Assert.IsFalse(sut.IsBottom);

         Assert.AreEqual(value, sut.RightOrDefault("bar"));
         Assert.AreEqual(Unit.Default, sut.LeftOrDefault(Unit.Default));
      }

      [Test]
      public async Task FromRightAsync_Bottoms_If_Null()
      {
         string value = null;
         Task<string> task = Task.FromResult(value);

         var eitherTask = Either<Unit, string>.FromRightAsync(task);
         var sut = await eitherTask;

         sut.DoLeft(_ => Assert.Fail());
         sut.DoRight(_ => Assert.Fail());

         bool isBottom = sut.Match(
           left: _ => false,
           right: _ => false,
           bottom: () => true);

         Assert.IsTrue(isBottom);

         Assert.IsTrue(sut.IsBottom);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsLeft);
      }

      [Test]
      public async Task FromRightAsync_Works_With_Default_Left()
      {
         string value = null;
         Task<string> task = Task.FromResult(value);

         var eitherTask = Either<int, string>.FromRightAsync(task, 3);
         var sut = await eitherTask;

         bool doLeftInvoked = true;
         sut.DoLeft(left =>
         {
            doLeftInvoked = true;
            Assert.AreEqual(3, left);
         });
         sut.DoRight(_ => Assert.Fail());

         Assert.IsTrue(doLeftInvoked);

         Assert.IsTrue(sut.IsLeft);
         Assert.IsFalse(sut.IsRight);
         Assert.IsFalse(sut.IsBottom);

         Assert.AreEqual(3, sut.LeftOrDefault(123));
         Assert.AreEqual("three", sut.RightOrDefault("three"));
      }

      [Test]
      public async Task MatchAsync_Works_For_Left_Async()
      {
         var leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            async left => await Task.FromResult("left"),
            right => "right",
            () => "bottom");

         Assert.AreEqual("left", leftSut);

         var rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            async left => await Task.FromResult("left"),
            right => "right",
            () => "bottom");

         Assert.AreEqual("right", rightSut);

         var bottomEither = Either<int, string>.FromBottom();
         string bottomSut = await bottomEither.MatchAsync(
            async left => await Task.FromResult("left"),
            right => "right",
            () => "bottom");

         Assert.AreEqual("bottom", bottomSut);
      }

      [Test]
      public async Task MatchAsync_Works_For_Right_Async()
      {
         var leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            left => "left",
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("left", leftSut);

         var rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            left => "left",
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("right", rightSut);

         var bottomEither = Either<int, string>.FromBottom();
         string bottomSut = await bottomEither.MatchAsync(
            left => "left",
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("bottom", bottomSut);
      }

      [Test]
      public async Task MatchAsync_Works_For_Left_And_Right_Async()
      {
         var leftEither = Either<int, string>.FromLeft(4);
         string leftSut = await leftEither.MatchAsync(
            async left => await Task.FromResult("left"),
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("left", leftSut);

         var rightEither = Either<int, string>.FromRight("foo");
         string rightSut = await rightEither.MatchAsync(
            async left => await Task.FromResult("left"),
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("right", rightSut);

         var bottomEither = Either<int, string>.FromBottom();
         string bottomSut = await bottomEither.MatchAsync(
            async left => await Task.FromResult("left"),
            async right => await Task.FromResult("right"),
            () => "bottom");

         Assert.AreEqual("bottom", bottomSut);
      }

      [Test]
      public void Select_Works_For_Right_Either()
      {
         string value = "test";
         Either<Unit, string> sut = value;

         var eitherRightUppercase = sut.Select(x => x.ToUpper());
         Assert.IsTrue(eitherRightUppercase.IsRight);
         eitherRightUppercase.DoRight(x => Assert.AreEqual(value.ToUpper(), x));
      }

      [Test]
      public void Select_Works_For_Left_Either()
      {
         string value = "test";
         Either<string, int> sut = value;

         var eitherLeft = sut.Select(x => x == 5);
         Assert.IsTrue(eitherLeft.IsLeft);
         eitherLeft.DoLeft(x => Assert.AreEqual(value, x));
      }

      [Test]
      public void Select_Works_For_Bottom_Either()
      {
         var sut = Either<Unit, string>.FromBottom();

         var eitherBottom = sut.Select(x => x == "foo");
         Assert.IsTrue(eitherBottom.IsBottom);
      }

      [Test]
      public void Where_Works_For_Right_Either()
      {
         string value = "test";
         Either<Unit, string> sut = value;

         var eitherRight = sut.Where(x => x == value);
         Assert.IsTrue(eitherRight.IsRight);
         eitherRight.DoRight(x => Assert.AreEqual(value, x));

         var eitherBottom = sut.Where(x => x == "foo");
         Assert.IsTrue(eitherBottom.IsBottom);
      }

      [Test]
      public void Where_Works_For_Left_Either()
      {
         string value = "test";
         Either<string, int> sut = value;

         var eitherBottom = sut.Where(x => x == 3);
         Assert.IsTrue(eitherBottom.IsBottom);
      }

      [Test]
      public void Where_Works_For_Bottom_Either()
      {
         var sut = Either<int, string>.FromBottom();

         var eitherBottom = sut.Where(x => x == "test");
         Assert.IsTrue(eitherBottom.IsBottom);
      }

      [Test]
      public void Implicit_Right_Operator_Works()
      {
         string value = "test";
         Either<Unit, string> sut = value;
         Assert.IsTrue(sut.IsRight);
      }

      [Test]
      public void Implicit_Right_Operator_Bottoms_If_Null()
      {
         string value = null;
         Either<Unit, string> sut = value;
         Assert.IsTrue(sut.IsBottom);
      }

      [Test]
      public void Implicit_Left_Operator_Works()
      {
         int value = 5;
         Either<int, Unit> sut = value;
         Assert.IsTrue(sut.IsLeft);
      }

      [Test]
      public void Implicit_Left_Operator_Bottoms_If_Null()
      {
         string value = null;
         Either<string, Unit> sut = value;
         Assert.IsTrue(sut.IsBottom);
      }
   }
}
