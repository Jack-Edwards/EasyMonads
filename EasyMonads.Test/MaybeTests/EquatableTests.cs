using System.Collections.Generic;
using NUnit.Framework;
using static EasyMonads.Core;

namespace EasyMonads.Test.MaybeTests
{
   [TestFixture]
   internal class EquatableTests
   {
      [Test]
      public void Maybe_Equality_Test()
      {
         var someA = Maybe(42);
         var someB = Some(42);
         var none = Maybe<int>.None;


         Assert.True(someA.Equals(someB));
         Assert.True(someA == someB);
         Assert.True(none == None);
      }

      [Test]
      public void Maybe_Equality_Test_When_Implicitly_Converted()
      {
         if (Some(42) == 42)
         {
            Assert.Pass();
         }
         else
         {
            Assert.Fail();
         }
      }

      [Test]
      public void Maybe_Inequality_Test()
      {
         var maybeA = Maybe(42);
         var maybeB = Some(12);
         Maybe<int> none = None;
         var someObj = Some(new object());

         Assert.False(maybeA.Equals(maybeB));
         Assert.True(maybeA != maybeB);
         Assert.True(maybeA != none);
         
         Assert.True(maybeA != someObj);
         Assert.False(maybeA == someObj);
      }

      [Test]
      public void Maybe_Inequality_Test_When_Implicitly_Converted()
      {
         if (Some(42) != 12)
         {
            Assert.Pass();
         }
         else
         {
            Assert.Fail();
         }
      }

      [Test]
      public void Maybe_is_Truthy_or_Falsy()
      {
         var someInt = Maybe<int?>.FromNullable(42);
         var noneInt = Maybe<int?>.FromNullable(null);
         bool some = someInt;
         bool none = noneInt;
         
         Assert.IsTrue(someInt);
         Assert.IsTrue(some);
         Assert.IsTrue(someInt == true);
         Assert.IsTrue(noneInt != true);
         
         Assert.IsFalse(noneInt);
         Assert.IsFalse(none);
         Assert.False(someInt == false);
         Assert.False(noneInt != false);
         
         Assert.IsTrue(Some(42));
         Assert.IsTrue(Some(new object()));

      }
      
      [Test]
      public void Maybe_Can_Be_Hash_Key()
      {
         object a = new object();
         object b = new object();
         
         var set = new HashSet<Maybe<object>>
         {
            Maybe(a),
            Maybe(b),
            Maybe(a)
         };

         Assert.IsTrue(2 == set.Count);
      }
  }
}