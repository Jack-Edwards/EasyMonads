using NUnit.Framework;
using System.Threading.Tasks;

namespace Monads.Test
{
   [TestFixture]
   internal class MaybeAsyncExtensions_Tests
   {
      [Test]
      public async Task BindAsync_Matches_None()
      {
         Task<Maybe<int>> isNone = Maybe<int>.None.AsTask();
         Maybe<string> unwrapped = await isNone.BindAsync(x => x.ToString());

         Assert.IsTrue(unwrapped.IsNone);
         unwrapped.IfSome(_ => Assert.Fail());
      }

      [Test]
      public async Task BindAsync_Matches_Some()
      {
         Task<Maybe<int>> isSome = Maybe<int>.From(5).AsTask();
         Maybe<string> unwrapped = await isSome.BindAsync(x => x.ToString());

         Assert.IsTrue(unwrapped.IsSome);
         unwrapped.IfSome(x => Assert.AreEqual("5", x));
         unwrapped.IfNone(() => Assert.Fail());
      }
   }
}
