using System;
using System.Collections.Generic;
using System.Linq;

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
   }
}