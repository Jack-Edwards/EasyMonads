using System;
using System.Threading.Tasks;

namespace Monads
{
   public static class OptionAsyncExtensions
   {
      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Option<TValue>> option, Func<TResult> none, Func<TValue, TResult> some)
      {
         Option<TValue> optionResult = await option;
         return optionResult.Match(none, some);
      }

      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Option<TValue>> option, Func<TResult> none, Func<TValue, Task<TResult>> someAsync)
      {
         Option<TValue> optionResult = await option;
         return await optionResult.MatchAsync(none, someAsync);
      }

      public static async Task<TResult> MatchAsync<TValue, TResult>(this Task<Option<TValue>> option, Func<Task<TResult>> noneAsync, Func<TValue, TResult> some)
      {
         Option<TValue> optionResult = await option;
         return await optionResult.MatchAsync(noneAsync, some);
      }

      public static async Task<Unit> IfSomeAsync<TValue>(this Task<Option<TValue>> option, Func<TValue, Task> someAsync)
      {
         Option<TValue> optionResult = await option;
         return await optionResult.IfSomeAsync(someAsync);
      }

      public static Task<Option<TResult>> BindAsync<TValue, TResult>(this Task<Option<TValue>> option, Func<TValue, Task<Option<TResult>>> bindAsync)
      {
         return option.MatchAsync(
            () => Option<TResult>.None,
            value => bindAsync(value));
      }

      public static Task<Either<TLeft, TValue>> ToEitherAsync<TLeft, TValue>(this Task<Option<TValue>> option, TLeft left)
      {
         return option.MatchAsync(
            () => left,
            value => Either<TLeft, TValue>.FromRight(value));
      }

      public static async Task<Option<TResult>> Select<TValue, TResult>(this Task<Option<TValue>> option, Func<TValue, TResult> map)
      {
         return await option.MatchAsync(
            () => Option<TResult>.None,
            value => map(value));
      }

      public static async Task<Option<TResult>> SelectMany<TValue, TIntermediate, TResult>(this Task<Option<TValue>> option, Func<TValue, Option<TIntermediate>> bind, Func<TValue, TIntermediate, TResult> project)
      {
         return await option.MatchAsync(
            () => Option<TResult>.None,
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

      public static async Task<Option<TValue>> Where<TValue>(this Task<Option<TValue>> option, Func<TValue, bool> predicate)
      {
         return await option.MatchAsync(
            () => Option<TValue>.None,
            value =>
            {
               return predicate(value)
                  ? value
                  : Option<TValue>.None;
            });
      }
   }
}
