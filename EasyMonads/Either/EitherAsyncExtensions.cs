using System;
using System.Threading.Tasks;

namespace Monads
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
         var eitherResult = await either;
         return eitherResult.DoRight(right);
      }

      public async static Task<Unit> DoRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task> rightAsync)
      {
         var eitherResult = await either;
         return await eitherResult.DoRightAsync(rightAsync);
      }

      public static Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TResult>>> map)
      {
         return either.MatchAsync(
            left: left => Either<TLeft, TResult>.FromLeft(left),
            rightAsync: right => map(right),
            neither: Either<TLeft, TResult>.Neither);
      }

      public static Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TResult>>> bind)
      {
         return either.MapAsync(
                async right =>
                    await Either<TLeft, TRight>.FromRight(right).MatchAsync(
                       left => Either<TLeft, TResult>.FromLeft(left),
                       async right2 => await bind(right2),
                       Either<TLeft, TResult>.Neither));
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
            Either<TLeft, TResult>.FromLeft,
            right => map(right),
            Either<TLeft, TResult>.Neither);
      }

      public static async Task<Either<TLeft, TResult>> SelectMany<TLeft, TRight, TIntermediate, TResult>(this Task<Either<TLeft, TRight>> either, Func<TRight, Task<Either<TLeft, TIntermediate>>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return await either.BindAsync(async (TRight right) =>
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
            right =>
            {
               return predicate(right)
                  ? right
                  : Either<TLeft, TRight>.FromRight(right);
            },
            Either<TLeft, TRight>.Neither);
      }
   }
}
