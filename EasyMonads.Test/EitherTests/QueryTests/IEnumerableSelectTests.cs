using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
      
      [Test]
      public async Task Select_Works_For_Async_IEnumerable_Either()
      {
         Dictionary<int, string> resultDict = new Dictionary<int, string>
         {
            { 0, "0" },
            { 1, "1" },
            { 2, "2" }
         };
         
         IEnumerable<Either<Unit, string>> results = await
            from number in GetRangeAsync()
            from text in ConvertToString(number)
            select text;
         
         foreach (var result in results.Select((x, i) => new { Value = x, Index = i }))
         {
            result.Value.DoRight(x => Assert.AreEqual(resultDict[result.Index], x));
            result.Value.DoLeftOrNeither(Assert.Fail);
         }

         return;
         
         Task<IEnumerable<Either<Unit, int>>> GetRangeAsync()
         {
            return resultDict.Select(x => Either<Unit, int>.FromRight(x.Key)).AsTask();
         }
         
         Either<Unit, string> ConvertToString(int number)
         {
            return Either<Unit, string>.From(number.ToString());
         }
      }
   }
}