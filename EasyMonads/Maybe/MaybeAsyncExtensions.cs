using System;
using System.Threading.Tasks;

namespace EasyMonads
{
   public static class MaybeAsyncExtensions
   {
      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TResult> none, Func<TValue, TResult> some)
      {
         Maybe<TValue> maybeResult = await maybe;
         return maybeResult.Match(none, some);
      }

      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TResult> none, Func<TValue, Task<TResult>> someAsync)
      {
         Maybe<TValue> maybeResult = await maybe;
         return await maybeResult.MatchAsync(none, someAsync);
      }

      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<Task<TResult>> noneAsync, Func<TValue, TResult> some)
      {
         Maybe<TValue> maybeResult = await maybe;
         return await maybeResult.MatchAsync(noneAsync, some);
      }

      public static async Task<Unit> IfSomeAsync<TValue>(this Task<Maybe<TValue>> maybe, Func<TValue, Task> someAsync)
      {
         Maybe<TValue> maybeResult = await maybe;
         return await maybeResult.IfSomeAsync(someAsync);
      }

      public static Task<Maybe<TResult>> BindAsync<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> bind)
      {
         return maybe.MatchAsync(
            () => Maybe<TResult>.None,
            value => bind(value));
      }

      public static Task<Either<TLeft, TValue>> ToEitherAsync<TLeft, TValue>(this Task<Maybe<TValue>> maybe, TLeft left)
      {
         return maybe.MatchAsync(
            () => left,
            value => Either<TLeft, TValue>.FromRight(value));
      }

      public static Task<Either<TValue, TRight>> ToLeftEitherAsync<TValue, TRight>(this Task<Maybe<TValue>> maybe, TRight right)
      {
         return maybe.MatchAsync(
            () => right,
            value => Either<TValue, TRight>.FromLeft(value));
      }

      public static async Task<Maybe<TResult>> Select<TValue, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, TResult> map)
      {
         return await maybe.MatchAsync(
            () => Maybe<TResult>.None,
            value => map(value));
      }

      public static async Task<Maybe<TResult>> SelectMany<TValue, TIntermediate, TResult>(this Task<Maybe<TValue>> maybe, Func<TValue, Maybe<TIntermediate>> bind, Func<TValue, TIntermediate, TResult> project)
      {
         return await maybe.MatchAsync(
            () => Maybe<TResult>.None,
            value =>
            {
               return bind(value).Match(
                  () => default,
                  intermediate =>
                  {
                     var result = project(value, intermediate);
                     if (result is null)
                     {
                        throw new InvalidOperationException();
                     }

                     return result;
                  });
            });
      }

      public static async Task<Maybe<TValue>> Where<TValue>(this Task<Maybe<TValue>> maybe, Func<TValue, bool> predicate)
      {
         return await maybe.MatchAsync(
            () => Maybe<TValue>.None,
            value =>
            {
               return predicate(value)
                  ? value
                  : Maybe<TValue>.None;
            });
      }
   }
}
