using System;
using System.Threading.Tasks;

namespace EasyMonads
{
   public readonly struct Either<TLeft, TRight>
   {
      private readonly TLeft _left;
      private readonly TRight _right;
      private readonly EitherState _state;

      private Either(TLeft left)
      {
         _right = default;

         if (left is null)
         {
            _state = EitherState.Neither;
            _left = default;
         }
         else
         {
            _state = EitherState.Left;
            _left = left;
         }
      }

      private Either(TRight right)
      {
         _left = default;

         if (right is null)
         {
            _state = EitherState.Neither;
            _right = default;
         }
         else
         {
            _state = EitherState.Right;
            _right = right;
         }
      }

      public static Either<TLeft, TRight> FromRight(TRight value)
      {
         return value is null
            ? Neither
            : new Either<TLeft, TRight>(value);
      }

      public static async Task<Either<TLeft, TRight>> FromRightAsync(Task<TRight> rightAsync)
      {
         var right = await rightAsync;
         return right is null
            ? Neither
            : right;
      }

      public static async Task<Either<TLeft, TRight>> FromRightAsync(Task<TRight> rightAsync, TLeft left)
      {
         var right = await rightAsync;
         return right is null
            ? FromLeft(left)
            : FromRight(right);
      }

      public static Either<TLeft, TRight> FromLeft(TLeft value)
      {
         return value is null
            ? Neither
            : new Either<TLeft, TRight>(value);
      }

      public static async Task<Either<TLeft, TRight>> FromLeftAsync(Task<TLeft> leftAsync)
      {
         var left = await leftAsync;
         return left is null
            ? Neither
            : left;
      }

      public bool IsLeft
      { get { return _state == EitherState.Left; } }

      public bool IsRight
      { get { return _state == EitherState.Right; } }

      public bool IsNeither
      { get { return _state == EitherState.Neither; } }

      public TRight RightOrDefault(TRight defaultValue)
      {
         return IsRight
            ? _right
            : defaultValue;
      }

      public TLeft LeftOrDefault(TLeft defaultValue)
      {
         return IsLeft
            ? _left
            : defaultValue;
      }

      private static Unit ValidateAction(Action action)
      {
         if (action is null)
         {
            throw new ArgumentNullException(nameof(action));
         }

         return Unit.Default;
      }

      private static Unit ValidateAction<T>(Action<T> action)
      {
         if (action is null)
         {
            throw new ArgumentNullException(nameof(action));
         }

         return Unit.Default;
      }

      private static Unit ValidateFunction<T1, T2>(Func<T1, T2> function)
      {
         if (function is null)
         {
            throw new ArgumentNullException(nameof(function));
         }

         return Unit.Default;
      }

      private static Unit ValidateMatch<TL, TR>(Func<TLeft, TL> left, Func<TRight, TR> right)
      {
         if (left is null)
         {
            throw new ArgumentNullException(nameof(left));
         }

         if (right is null)
         {
            throw new ArgumentNullException(nameof(right));
         }

         return Unit.Default;
      }

      public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right, TResult neither)
      {
         ValidateMatch(left, right);

#pragma warning disable CS8524
         return _state switch
         {
            EitherState.Neither => neither,
            EitherState.Left => left(_left),
            EitherState.Right => right(_right)
         };
#pragma warning restore CS8524
      }

      public TResult Match<TResult>(TResult leftOrNeither, Func<TRight, TResult> right)
      {
         ValidateFunction(right);

         return IsRight
            ? right(_right)
            : leftOrNeither;
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, Task<TResult>> leftAsync, Func<TRight, TResult> right, TResult neither)
      {
         ValidateMatch(leftAsync, right);

#pragma warning disable CS8524
         return _state switch
         {
            EitherState.Neither => neither,
            EitherState.Left => await leftAsync(_left),
            EitherState.Right => right(_right)
         };
#pragma warning restore CS8524
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, TResult> left, Func<TRight, Task<TResult>> rightAsync, TResult neither)
      {
         ValidateMatch(left, rightAsync);

#pragma warning disable CS8524
         return _state switch
         {
            EitherState.Neither => neither,
            EitherState.Left => left(_left),
            EitherState.Right => await rightAsync(_right)
         };
#pragma warning restore CS8524
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, Task<TResult>> leftAsync, Func<TRight, Task<TResult>> rightAsync, TResult neither)
      {
         ValidateMatch(leftAsync, rightAsync);

#pragma warning disable CS8524
         return _state switch
         {
            EitherState.Neither => neither,
            EitherState.Left => await leftAsync(_left),
            EitherState.Right => await rightAsync(_right)
         };
#pragma warning restore CS8524
      }

      public Either<TLeft, TResult> Map<TResult>(Func<TRight, TResult> map)
      {
         ValidateFunction(map);

         return IsRight
            ? map(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.Neither;
      }

      public Either<TResult, TRight> MapLeft<TResult>(Func<TLeft, TResult> map)
      {
         ValidateFunction(map);

         return IsLeft
            ? map(_left)
            : IsRight
               ? Either<TResult, TRight>.FromRight(_right)
               : Either<TResult, TRight>.Neither;
      }

      public async Task<Either<TLeft, TResult>> MapAsync<TResult>(Func<TRight, Task<TResult>> mapAsync)
      {
         ValidateFunction(mapAsync);

         return IsRight
            ? await mapAsync(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.Neither;
      }

      public Either<TLeft, TResult> Bind<TResult>(Func<TRight, Either<TLeft, TResult>> bind)
      {
         ValidateFunction(bind);

         return IsRight
            ? bind(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.Neither;
      }

      public Either<TResult, TRight> BindLeft<TResult>(Func<TLeft, Either<TResult, TRight>> bind)
      {
         ValidateFunction(bind);

         return IsLeft
            ? bind(_left)
            : IsRight
               ? Either<TResult, TRight>.FromRight(_right)
               : Either<TResult, TRight>.Neither;
      }

      public async Task<Either<TLeft, TResult>> BindAsync<TResult>(Func<TRight, Task<Either<TLeft, TResult>>> bindAsync)
      {
         ValidateFunction(bindAsync);

         return IsRight
            ? await bindAsync(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.Neither;
      }

      public Unit DoRight(Action<TRight> right)
      {
         ValidateAction(right);

         if (IsRight)
         {
            right(_right);
         }

         return Unit.Default;
      }

      public async Task<Unit> DoRightAsync(Func<TRight, Task> rightAsync)
      {
         ValidateFunction(rightAsync);

         if (IsRight)
         {
            await rightAsync(_right);
         }

         return Unit.Default;
      }

      public Unit DoLeftOrNeither(Action leftOrNeither)
      {
         ValidateAction(leftOrNeither);

         if (!IsRight)
         {
            leftOrNeither();
         }

         return Unit.Default;
      }

      public Unit DoLeftOrNeither(Action<TLeft> left, Action neither)
      {
         ValidateAction(left);
         ValidateAction(neither);

         if (IsLeft)
         {
            left(_left);
         }

         if (IsNeither)
         {
            neither();
         }

         return Unit.Default;
      }

      public async Task<Unit> DoLeftOrNeitherAsync(Func<TLeft, Task> leftAsync, Action neither)
      {
         ValidateFunction(leftAsync);
         ValidateAction(neither);

         if (IsLeft)
         {
            await leftAsync(_left);
         }

         if (IsNeither)
         {
            neither();
         }

         return Unit.Default;
      }

      public Maybe<TRight> ToMaybe()
      {
         return IsRight
            ? Maybe<TRight>.From(_right)
            : Maybe<TRight>.None;
      }

      public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> map)
      {
         return Match(
            left: left => left,
            right: right => map(right),
            neither: Either<TLeft, TResult>.Neither);
      }

      public Either<TLeft, TResult> SelectMany<TIntermediate, TResult>(Func<TRight, Either<TLeft, TIntermediate>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return Bind(x => bind(x).Bind(y => Either<TLeft, TResult>.FromRight(project(x, y))));
      }

      public Either<TLeft, TRight> Where(Func<TRight, bool> predicate)
      {
         if (!IsRight)
         {
            return Neither;
         }

         return predicate(_right)
            ? this
            : Neither;
      }

      public static Either<TLeft, TRight> Neither => new Either<TLeft, TRight>();

      public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);

      public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);
   }
}
