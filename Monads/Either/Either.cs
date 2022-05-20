using System;
using System.Threading.Tasks;

namespace Monads
{
   public struct Either<TLeft, TRight>
   {
      private readonly TLeft _left;
      private readonly TRight _right;
      private readonly EitherState _state;

      private Either(TLeft left)
      {
         _right = default;

         if (left is null)
         {
            _state = EitherState.Bottom;
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
            _state = EitherState.Bottom;
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
            ? FromBottom()
            : new Either<TLeft, TRight>(value);
      }

      public static async Task<Either<TLeft, TRight>> FromRightAsync(Task<TRight> rightAsync)
      {
         var right = await rightAsync;
         return right is null
            ? FromBottom()
            : FromRight(right);
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
            ? FromBottom()
            : new Either<TLeft, TRight>(value);
      }

      public static Either<TLeft, TRight> FromBottom()
      {
         return new Either<TLeft, TRight>();
      }

      public bool IsLeft
      { get { return _state == EitherState.Left; } }

      public bool IsRight
      { get { return _state == EitherState.Right; } }

      public bool IsBottom
      { get { return _state == EitherState.Bottom; } }

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

         return default;
      }

      private static TResult MatchBottom<TResult>(Func<TResult> bottom = null)
      {
         return bottom is null
               ? default
               : bottom();
      }

      public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right, Func<TResult> bottom = null)
      {
         ValidateMatch(left, right);

         return _state switch
         {
            EitherState.Bottom => MatchBottom(bottom),
            EitherState.Left => left(_left),
            EitherState.Right => right(_right),
            _ => throw new NotImplementedException()
         };
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, Task<TResult>> leftAsync, Func<TRight, TResult> right, Func<TResult> bottom = null)
      {
         ValidateMatch(leftAsync, right);

         return _state switch
         {
            EitherState.Bottom => MatchBottom(bottom),
            EitherState.Left => await leftAsync(_left),
            EitherState.Right => right(_right),
            _ => throw new NotImplementedException()
         };
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, TResult> left, Func<TRight, Task<TResult>> rightAsync, Func<TResult> bottom = null)
      {
         ValidateMatch(left, rightAsync);

         return _state switch
         {
            EitherState.Bottom => MatchBottom(bottom),
            EitherState.Left => left(_left),
            EitherState.Right => await rightAsync(_right),
            _ => throw new NotImplementedException()
         };
      }

      public async Task<TResult> MatchAsync<TResult>(Func<TLeft, Task<TResult>> leftAsync, Func<TRight, Task<TResult>> rightAsync, Func<TResult> bottom = null)
      {
         ValidateMatch(leftAsync, rightAsync);

         return _state switch
         {
            EitherState.Bottom => MatchBottom(bottom),
            EitherState.Left => await leftAsync(_left),
            EitherState.Right => await rightAsync(_right),
            _ => throw new NotImplementedException()
         };
      }

      public Either<TLeft, TResult> Map<TResult>(Func<TRight, TResult> map)
      {
         if (map is null)
         {
            throw new ArgumentNullException(nameof(map));
         }

         return IsRight
            ? map(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.FromBottom();
      }

      public Either<TResult, TRight> MapLeft<TResult>(Func<TLeft, TResult> map)
      {
         if (map is null)
         {
            throw new ArgumentNullException(nameof(map));
         }

         return IsLeft
            ? map(_left)
            : IsRight
               ? Either<TResult, TRight>.FromRight(_right)
               : Either<TResult, TRight>.FromBottom();
      }

      public async Task<Either<TLeft, TResult>> MapAsync<TResult>(Func<TRight, Task<TResult>> mapAsync)
      {
         if (mapAsync is null)
         {
            throw new ArgumentNullException(nameof(mapAsync));
         }

         return IsRight
            ? await mapAsync(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.FromBottom();
      }

      public Either<TLeft, TResult> Bind<TResult>(Func<TRight, Either<TLeft, TResult>> bind)
      {
         if (bind is null)
         {
            throw new ArgumentNullException(nameof(bind));
         }

         return IsRight
            ? bind(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.FromBottom();
      }

      public async Task<Either<TLeft, TResult>> BindAsync<TResult>(Func<TRight, Task<Either<TLeft, TResult>>> bindAsync)
      {
         if (bindAsync is null)
         {
            throw new ArgumentNullException(nameof(bindAsync));
         }

         return IsRight
            ? await bindAsync(_right)
            : IsLeft
               ? Either<TLeft, TResult>.FromLeft(_left)
               : Either<TLeft, TResult>.FromBottom();
      }

      public Unit DoRight(Action<TRight> right)
      {
         if (right is null)
         {
            throw new ArgumentNullException(nameof(right));
         }

         if (IsRight)
         {
            right(_right);
         }

         return Unit.Default;
      }

      public async Task<Unit> DoRightAsync(Func<TRight, Task> rightAsync)
      {
         if (rightAsync is null)
         {
            throw new ArgumentNullException(nameof(rightAsync));
         }

         if (IsRight)
         {
            await rightAsync(_right);
         }

         return Unit.Default;
      }

      public Unit DoLeft(Action<TLeft> left)
      {
         if (left is null)
         {
            throw new ArgumentNullException(nameof(left));
         }

         if (IsLeft)
         {
            left(_left);
         }

         return Unit.Default;
      }

      public async Task<Unit> DoLeftAsync(Func<TLeft, Task> leftAsync)
      {
         if (leftAsync is null)
         {
            throw new ArgumentNullException(nameof(leftAsync));
         }

         if (IsLeft)
         {
            await leftAsync(_left);
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
            bottom: () => Either<TLeft, TResult>.FromBottom());
      }

      public Either<TLeft, TResult> SelectMany<TIntermediate, TResult>(Func<TRight, Either<TLeft, TIntermediate>> bind, Func<TRight, TIntermediate, TResult> project)
      {
         return Bind(x => bind(x).Bind(y => Either<TLeft, TResult>.FromRight(project(x, y))));
      }

      public Either<TLeft, TRight> Where(Func<TRight, bool> predicate)
      {
         if (!IsRight)
         {
            return FromBottom();
         }

         return predicate(_right)
            ? this
            : FromBottom();
      }

      public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);

      public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);
   }
}
