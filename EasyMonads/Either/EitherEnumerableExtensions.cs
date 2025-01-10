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
      
      public static IEnumerable<Task<Either<TLeft, TResult>>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this IEnumerable<Task<Either<TLeft, TRight>>> enumerableAsyncEither,
         Func<TRight, Task<Either<TLeft, TIntermediate>>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return enumerableAsyncEither.Select(async either => await either.BindAsync<TLeft, TRight, TResult>(right =>
            bind(right).BindAsync<TLeft, TIntermediate, TResult>(intermediate => project(right, intermediate))));
      }
      
      /*
      public static async IAsyncEnumerable<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(
         this IAsyncEnumerable<Either<TLeft, TRight>> enumerableEither, 
         Func<Either<TLeft, TRight>, IAsyncEnumerable<Either<TLeft, TIntermediate>>> bindAsync,
         Func<Either<TLeft, TRight>, Either<TLeft, TIntermediate>, Either<TLeft, TResult>> project)
      {
         await foreach (Either<TLeft, TRight> either in enumerableEither)
         {
            await foreach (Either<TLeft, TIntermediate> intermediate in bindAsync(either))
            {
               yield return project(either, intermediate);
            }
         }
      }
 
      public static async IAsyncEnumerable<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(
         this IAsyncEnumerable<Either<TLeft, TRight>> enumerableEither, 
         Func<TRight, Either<TLeft, TIntermediate>> bind,
         Func<TRight, TIntermediate, TResult> project)
      {
         await foreach (var either in enumerableEither)
         {
            yield return await either.BindAsync<TResult>(async right => bind(right).Bind(delegate(TIntermediate intermediate)
            {
               Either<TLeft, TResult> projection = project(right, intermediate);
               return projection;
            }));
         }
      }
      
      public static async IAsyncEnumerable<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(
         this IAsyncEnumerable<Either<TLeft, TRight>> enumerableEither, 
         Func<TRight, Either<TLeft, TIntermediate>> bind,
         Func<TRight, TIntermediate, TResult> project)
      {
         await foreach (var either in enumerableEither)
         {
            yield return await either.BindAsync<TResult>(right => bind(right).BindAsync(delegate(TIntermediate intermediate)
            {
               Either<TLeft, TResult> projection = project(right, intermediate);
               return projection.AsTask();
            }));
         }
      }
      */
      
      public static async Task<IEnumerable<Either<TLeft, TResult>>> Select<TLeft, TRight, TResult>(this Task<IEnumerable<Either<TLeft, TRight>>> asyncEnumerableEither, Func<TRight, TResult> map)
      {
         return (await asyncEnumerableEither).Select(map);
      }
      
      public static async Task<IEnumerable<Either<TLeft, TResult>>> SelectMany<TLeft, TRight, TResult>(this Task<IEnumerable<Either<TLeft, TRight>>> asyncEnumerableEither,
         Func<TRight, Either<TLeft, Func<TRight, Either<TLeft, TResult>>>> bind,
         Func<TRight, Func<TRight, Either<TLeft, TResult>>, TResult> project)
      {
         return (await asyncEnumerableEither).Select(either =>
         {
            return either.Bind<TResult>(right =>
               bind(right).Bind(delegate(Func<TRight, Either<TLeft, TResult>> intermediate)
               {
                  Either<TLeft, TResult> projection = project(right, intermediate);
                  return projection;
               }));
         });
      }
      
      public static IEnumerable<Task<Either<TLeft, TResult>>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this IEnumerable<Task<Either<TLeft, TRight>>> asyncEnumerableEither,
         Func<TRight, Either<TLeft, TIntermediate>> bind,
         Func<TRight, TIntermediate, TResult> project)
      {
         return asyncEnumerableEither.Select(asyncEither => asyncEither.BindAsync(right => bind(right).Bind(delegate(TIntermediate intermediate)
         {
            Either<TLeft, TResult> projection = project(right, intermediate);
            return projection;
         })));
      }
   }
}