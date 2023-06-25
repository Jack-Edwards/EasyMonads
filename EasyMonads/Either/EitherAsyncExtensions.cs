using System;
using System.Threading.Tasks;

namespace EasyMonads
{
   public static class EitherAsyncExtensions
   {
      public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, TResult leftOrNeither, Func<TRight, TResult> right)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return eitherResult.Match(leftOrNeither, right);
      }

      public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TLeft, TResult> left, Func<TRight, TResult> right, TResult neither)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return eitherResult.Match(left, right, neither);
      }

      public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TLeft, TResult> left, Func<TRight, Task<TResult>> rightAsync, TResult neither)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return await eitherResult.MatchAsync(left, rightAsync, neither);
      }

      public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TLeft, Task<TResult>> leftAsync, Func<TRight, TResult> right, TResult neither)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return await eitherResult.MatchAsync(leftAsync, right, neither);
      }

      public static async Task<Unit> DoRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> either, Action<TRight> right)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return eitherResult.DoRight(right);
      }

      public static async Task<Unit> DoRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task> rightAsync)
      {
         Either<TLeft, TRight> eitherResult = await either;
         return await eitherResult.DoRightAsync(rightAsync);
      }

      public static Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Either<TLeft, TResult>> map)
      {
         return either.MatchAsync(
            left: Either<TLeft, TResult>.FromLeft,
            right: map,
            neither: Either<TLeft, TResult>.Neither);
      }

      public static Task<Either<TResult, TRight>> MapLeftAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TLeft, Either<TResult, TRight>> map)
      {
         return either.MatchAsync(
            left: map,
            right: Either<TResult, TRight>.FromRight,
            neither: Either<TResult, TRight>.Neither);
      }

      public static Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TResult>>> map)
      {
         return either.MatchAsync(
            left: Either<TLeft, TResult>.FromLeft,
            rightAsync: map,
            neither: Either<TLeft, TResult>.Neither);
      }

      public static Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TResult>>> bindAsync)
      {
         return either.MapAsync(
               async right =>
                  await Either<TLeft, TRight>.FromRight(right).MatchAsync(
                     left => Either<TLeft, TResult>.FromLeft(left),
                     async right2 => await bindAsync(right2),
                     Either<TLeft, TResult>.Neither));
      }

      public static Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Either<TLeft, TResult>> bind)
      {
         return either.MapAsync(
               right =>
                  Either<TLeft, TRight>.FromRight(right).Match(
                     Either<TLeft, TResult>.FromLeft,
                     bind,
                     Either<TLeft, TResult>.Neither));
      }

      public static Task<Either<TResult, TRight>> BindLeftAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TLeft, Either<TResult, TRight>> bind)
      {
         return either.MapLeftAsync(
               left =>
                  Either<TLeft, TRight>.FromLeft(left).Match(
                     bind,
                     Either<TResult, TRight>.FromRight,
                     Either<TResult, TRight>.Neither));
      }

      public static Task<Maybe<TRight>> ToMaybeTask<TLeft, TRight>(this Task<Either<TLeft, TRight>> either)
      {
         return either.MatchAsync(
            left => Maybe<TRight>.None,
            right => right,
            Maybe<TRight>.None);
      }

      public static async Task<Either<TLeft, TResult>> Select<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, TResult> map)
      {
         return await either.MatchAsync(
            left: Either<TLeft, TResult>.FromLeft,
            right: right => map(right),
            neither: Either<TLeft, TResult>.Neither);
      }

      public static async Task<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TIntermediate>>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return await either.BindAsync(async right =>
            await bind(right).BindAsync(delegate (TIntermediate intermediate)
            {
               Either<TLeft, TResult> projection = project(right, intermediate);
               return projection.AsTask();
            }));
      }

      public static async Task<Either<TLeft, TRight>> Where<TLeft, TRight>(this Task<Either<TLeft, TRight>> either, Func<TRight, bool> predicate)
      {
         return await either.MatchAsync(
            left => Either<TLeft, TRight>.Neither,
            right => predicate(right)
               ? right
               : Either<TLeft, TRight>.FromRight(right),
            Either<TLeft, TRight>.Neither);
      }
   }
}
