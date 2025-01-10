using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace EasyMonads.Test.EitherTests.QueryTests
{
   [TestFixture]
   internal class IEnumerableSelectTests
   {
      [Test]
      public void Select_Works_For_IEnumerable_Either()
      {
         Dictionary<int, string> resultDict = new Dictionary<int, string>
         {
            { 0, "0" },
            { 1, "1" },
            { 2, "2" }
         };
         
         IEnumerable<Either<Unit, string>> results = from number in GetRange()
            from text in ConvertToString(number)
            select text;

         foreach (var result in results.Select((x, i) => new { Value = x, Index = i }))
         {
            result.Value.DoRight(x => Assert.AreEqual(resultDict[result.Index], x));
            result.Value.DoLeftOrNeither(Assert.Fail);
         }

         return;
         
         IEnumerable<Either<Unit, int>> GetRange()
         {
            foreach (var entry in resultDict)
            {
               yield return entry.Key;
            }
         }
         
         Either<Unit, string> ConvertToString(int number)
         {
            return number.ToString();
         }
      }
   }
}