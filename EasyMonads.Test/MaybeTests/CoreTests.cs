using System;
using NUnit.Framework;
using static EasyMonads.Core;

namespace EasyMonads.Test.MaybeTests
{
   [TestFixture]
   internal class CoreTests
   {
      [Test]
      public void Can_Implicitly_Convert_None_To_Maybe()
      {
         Maybe<int> maybe = None;
         Assert.IsTrue(maybe.IsNone);
      }

      [Test]
      public void Maybe_Constructs_Valid_Maybe()
      {
         var someInt = Maybe(5);

         object? obj = null;
         var noneObj = Maybe(obj);
         var someObj = Maybe(new object());

         Assert.IsTrue(someInt.IsSome);
         Assert.IsTrue(noneObj.IsNone);
         Assert.IsTrue(someObj.IsSome);
      }

      [Test]
      public void Some_Constructs_Valid_Maybe_When_Value_Is_Not_Null()
      {
         var maybeValType = Some(1);
         var maybeRefType = Some(new object());
         
         Assert.IsTrue(maybeValType.IsSome);
         Assert.IsTrue(maybeRefType.IsSome);
      }
      
      [Test]
      public void Some_Throws_When_Value_Is_Null()
      {
         object? obj = null;

         Assert.Throws<ArgumentNullException>(() => Some(obj));
      }
  }
}