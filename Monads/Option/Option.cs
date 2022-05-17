using System;
using System.Threading.Tasks;

namespace Monads
{
   public readonly struct Option<TValue>
   {
      private readonly OptionState _state;
      private readonly TValue _value;

      private Option(TValue value)
      {
         _value = value;
         _state = value is null
            ? OptionState.None
            : OptionState.Some;
      }

      public static Option<TValue> From(TValue value)
      {
         return new Option<TValue>(value);
      }

      public static Task<Option<TValue>> FromAsync(Task<TValue> value)
      {
         async Task<Option<TValue>> unpack()
         {
            var result = await value;
            return From(result);
         }

         return unpack();
      }

      public bool IsSome
      { get { return _state == OptionState.Some; } }

      public bool IsNone
      { get { return _state == OptionState.None; } }

      public Unit IfSome(Action<TValue> some)
      {
         if (some is null)
         {
            throw new ArgumentNullException(nameof(some));
         }

         if (IsSome)
         {
            some(_value);
         }

         return default;
      }

      public async Task<Unit> IfSomeAsync(Func<TValue, Task> someAsync)
      {
         if (someAsync is null)
         {
            throw new ArgumentNullException(nameof(someAsync));
         }

         if (IsSome)
         {
            await someAsync(_value);
         }

         return default;
      }

      public Unit IfNone(Action none)
      {
         if (none is null)
         {
            throw new ArgumentNullException(nameof(none));
         }

         if (IsNone)
         {
            none();
         }

         return default;
      }

      public TValue SomeOrDefault(TValue defaultValue)
      {
         return IsNone
            ? defaultValue
            : _value;
      }

      public TResult Match<TResult>(Func<TResult> none, Func<TValue, TResult> some)
      {
         if (none is null)
         {
            throw new ArgumentNullException(nameof(none));
         }

         if (some is null)
         {
            throw new ArgumentNullException(nameof(some));
         }

         return IsSome
            ? some(_value)
            : none();
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TResult> none, Func<TValue, Task<TResult>> someAsync)
      {
         if (none is null)
         {
            throw new ArgumentNullException(nameof(none));
         }

         if (someAsync is null)
         {
            throw new ArgumentNullException(nameof(someAsync));
         }

         return IsSome
            ? await someAsync(_value)
            : none();
      }

      public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> noneAsync, Func<TValue, TResult> some)
      {
         if (noneAsync is null)
         {
            throw new ArgumentNullException(nameof(noneAsync));
         }

         if (some is null)
         {
            throw new ArgumentNullException(nameof(some));
         }

         return IsSome
            ? some(_value)
            : await noneAsync();
      }

      public Option<TResult> Map<TResult>(Func<TValue, TResult> map)
      {
         if (map is null)
         {
            throw new ArgumentNullException(nameof(map));
         }

         return IsSome
            ? map(_value)
            : Option<TResult>.None;
      }

      public async Task<Option<TResult>> MapAsync<TResult>(Func<TValue, Task<TResult>> mapAsync)
      {
         if (mapAsync is null)
         {
            throw new ArgumentNullException(nameof(mapAsync));
         }

         return IsSome
            ? await mapAsync(_value)
            : Option<TResult>.None;
      }

      public Option<TResult> Bind<TResult>(Func<TValue, Option<TResult>> bind)
      {
         if (bind is null)
         {
            throw new ArgumentNullException(nameof(bind));
         }

         return IsSome
            ? bind(_value)
            : Option<TResult>.None;
      }

      public async Task<Option<TResult>> BindAsync<TResult>(Func<TValue, Task<Option<TResult>>> bindAsync)
      {
         if (bindAsync is null)
         {
            throw new ArgumentNullException(nameof(bindAsync));
         }

         return IsSome
            ? await bindAsync(_value)
            : Option<TResult>.None;
      }

      public Either<TLeft, TValue> ToEither<TLeft>(TLeft left)
      {
         return IsSome
            ? Either<TLeft, TValue>.FromRight(_value)
            : Either<TLeft, TValue>.FromLeft(left);
      }

      public Either<TValue, Unit> ToLeftEither()
      {
         return IsSome
            ? Either<TValue, Unit>.FromLeft(_value)
            : Either<TValue, Unit>.FromRight(Unit.Default);
      }

      public Either<TValue, TRight> ToLeftEither<TRight>(TRight right)
      {
         return IsSome
            ? Either<TValue, TRight>.FromLeft(_value)
            : Either<TValue, TRight>.FromRight(right);
      }

      public Option<TResult> Select<TResult>(Func<TValue, TResult> map)
      {
         return IsSome
            ? Option<TResult>.From(map(_value))
            : default;
      }

      public Option<TResult> SelectMany<TIntermediate, TResult>(Func<TValue, Option<TIntermediate>> bind, Func<TValue, TIntermediate, TResult> project)
      {
         if (IsNone)
         {
            return default;
         }

         var bound = bind(_value);

         if (bound.IsNone)
         {
            return default;
         }

         var result = project(_value, bound._value);

         if (result is null)
         {
            throw new InvalidOperationException();
         }

         return result;
      }

      public Option<TValue> Where(Func<TValue, bool> predicate)
      {
         return IsSome && predicate(_value)
            ? this
            : default;
      }

      public static readonly Option<TValue> None = default;
      public static implicit operator Option<TValue>(TValue value) => Option<TValue>.From(value);
   }
}
