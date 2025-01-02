# Support for just a few Monad types in C#

## Key differences from other libraries

### No exceptions and no 'unsafe' methods when working with `null` values

Some libraries go out of their way to stop you from providing `null` as a value when creating an instance of a monad or acting on an existing monad.
For example, trying to instantiate a monad with a null value will throw an exception.
So will returning `null` from a `Match` statement.
The "work-around" in these situations is using methods with "Unsafe" in the name, as if `null` is an implicitly hazardous thing.

Instead of throwing runtime exceptions or asking you to use methods with a different naming convention*, this library will try to let you do what you want and continue running.

_* There are some method names in this library that include the word "Nullable".
The behavior of these methods is the same as their standard counterparts.
The reason for these "Nullable" alternatives is to support projects with \<nullable\> enabled; you may provide nullable items to these methods without nullability warnings._

The monads supported by this library all have some default state:

* `Maybe<T> => None`
* `Either<TLeft, TRight> => Neither`

Rather than throw an exception when receiving null values, the monads you get back will just be in their default states.
These states should already be handled by the caller - there is nothing "unsafe" about it.
Especially when compared to throwing an exception which may terminate the program.

Similarly, if you want to return `null` in your `Match` statements, why shouldn't you?
This library is not going to get in the way of writing the code you want to write.

### Use the same types for async

While implementing the AsyncExtensions for Either and Maybe, I discovered `Task<Either<TLeft, TRight>>` and `Task<Maybe<T>>>` work perfectly fine.
Implementing distinct `EitherAsync` and `MaybeAsync` types is not necessary in order to add support for the Linq query syntax.

It is only when you add Linq query support for `Task<T>` that you begin to encounter ambiguity problems between `Task<Monad>` and `Task<T>`.
I do not intend to add Linq query support for `Task<T>`, so this should not become a problem.

## Monad Types

### Maybe\<T>

A simple wrapper around a value which may/not exist.
A good alternative to nullable types.

```cs
Maybe<int> daysSinceLastAccident = 10;

daysSinceLastAccident.IfNone(
   () => Console.WriteLine($"An accident has never been reported!"));

daysSinceLastAccident.IfSome(
   x => Console.WriteLine($"The last accident occurred {x} days ago!"));

int nonNullableDays = daysSinceLastAccident.GetSomeOrDefault(0);

double daysDouble = daysSinceLastAccident.Match(
   () => 0.0,
   x => double.Parse(x));
```

Convenience functions are available in `EasyMonads.Core` for simple `Maybe<T>` construction.

* `Some(value)` should be used when you expect the value to be non-null. It throws if null.
* `None` gets implicitly converted to `Maybe<T>`
* `Maybe(value)` constructs from a null or non-null value.

```cs
using static EasyMonads.Core;

...
    
var answer = Some(42);              // Maybe<int> answer = 42;
Maybe<int> nothing = None;          // var nothing = Maybe<int>.None;

Maybe<object> TryFoo()
{
    ...
    return None;                    // return Maybe<object>.None;
}
        
object foo = GetFoo();
var result = Maybe(foo)             // Maybe<object>.From(foo)

```

Maybe has equality checks, truthiness, and implicit conversions.

| LHS                      | True | RHS                   | 
|--------------------------|------|-----------------------|
| `Some(42)`               | `==` | `42`                  |
| `Some(42)`               | `!=` | `12`                  |
| `Some(42)`               | `==` | `Some(42)`            |
| `Some(42)`               | `!=` | `Some(12)`            |
| `Some(42)`               | `!=` | `Some(new object())`  |
| `Maybe<object> a = None` | `!=` | `Maybe<int> b = None` |
| `Maybe<int> a = None`    | `==` | `Maybe<int> b = None` |

```cs
Maybe<object> maybe = TryFoo();
    
bool success = maybe;               // bool success = maybe.IsSome;

if (maybe)                          // if (maybe.IsSome)
    ...

```

### Either\<TLeft, TRight>

A choice monad with left (bad) and right (good) states/values.
There is also a neither (empty) state which does not contain any value, sort of like an implicit `Maybe<Either<TLeft, TRight>>`.

A great example of when to use this is when deserializing a response from a web API.
Consider an API that may either return some good data with a 200 response or a standard error message with a 4xx or 5xx response.
You already know which data type to deserialize to by looking at the HTTP response, but what kind of object can you use to represent two different types of data?
One option could be `object` or `dynamic`, but those become difficult to work with almost immediately after you implement them.

A better option is deserializing to the correct data type, then storing the instance in an Either.

```cs

Either<int, MyDTO> apiResponse = httpStatus == 200
   ? JsonSerializer.Deserialize<MyDTO>(responseData)
   : JsonSerializer.Deserialize<int>(responseData);

eitherApiResponse.DoRight(dto =>
{
   // Do something with the dto data
});

eitherApiResponse.DoLeftOrNeither(
   left: errorCode =>
   {
      Console.WriteLine($"The API responded with an error code: {errorCode}");
   },
   neither: () => Console.WriteLine($"The API did not respond"))

bool wasTheRequestSuccessful = eitherApiResponse.IsRight;

Maybe<MyDTO> apiResponse = eitherApiResponse.ToMaybe();

```
