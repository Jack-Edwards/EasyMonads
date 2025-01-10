using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyMonads
{
   public static class EitherEnumerableExtensions
   {
      public static IEnumerable<Either<TLeft, TResult>> Select<TLeft, TRight, TResult>(this IEnumerable<Either<TLeft, TRight>> enumerableEither, Func<TRight, TResult> map)
      {
         return enumerableEither.Select(either => either.Map(map));
      }
      
      public static IEnumerable<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this IEnumerable<Either<TLeft, TRight>> enumerableEither, Func<TRight, Either<TLeft, TIntermediate>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return enumerableEither.Select<Either<TLeft, TRight>, Either<TLeft, TResult>>(either => either.Bind<TResult>(right =>
            bind(right).Bind<TResult>(intermediate => project(right, intermediate))));
      }

      public static IEnumerable<Task<Either<TLeft, TResult>>> Select<TLeft, TRight, TResult>(this IEnumerable<Task<Either<TLeft, TRight>>> enumerableEither, Func<TRight, TResult> map)
      {
         return enumerableEither.Select(async eitherTask => await eitherTask.MatchAsync(
            left: Either<TLeft, TResult>.FromLeft,
            right: right => map(right),
            neither: Either<TLeft, TResult>.Neither));
      }
      
      public static IEnumerable<Task<Either<TLeft, TResult>>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this IEnumerable<Task<Either<TLeft, TRight>>> enumerableEitherTasks, Func<TRight, Task<Either<TLeft, TIntermediate>>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return enumerableEitherTasks.Select(async eitherTask => await eitherTask.BindAsync(async right =>
            await bind(right).BindAsync(delegate (TIntermediate intermediate)
            {
               Either<TLeft, TResult> projection = project(right, intermediate);
               return projection.AsTask();
            })));
      }
   }
}