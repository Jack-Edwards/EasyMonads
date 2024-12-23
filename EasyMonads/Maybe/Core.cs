using System;

namespace EasyMonads
{
   public static partial class Core
   {
      public static Maybe.None None => EasyMonads.Maybe.None.Default;

      /// <summary>
      /// Simple factory for `Maybe` type.
      /// </summary>
      /// <param name="value">non-null for `Some` or null for `None`</param>
      /// <typeparam name="T">Type held by Maybe</typeparam>
      /// <returns>Maybe</returns>
      public static Maybe<T> Maybe<T>(T value) => new(value);

      /// <summary>
      /// Factory for `Maybe` type when value is expected to be non-null.
      /// </summary>
      /// <param name="value">non-null value</param>
      /// <typeparam name="T">Type held by Maybe</typeparam>
      /// <returns>Maybe</returns>
      /// <exception cref="ArgumentNullException">Thrown if value is null</exception>
      public static Maybe<T> Some<T>(T value)
      {
         if (value is null)
         {
            throw new ArgumentNullException(nameof(value));
         }

         return new Maybe<T>(value);
      }
   }
   
   public struct Maybe
   {
      /// <summary>
      /// Simple none-type for implicit conversion to typed `Maybe`.
      /// </summary>
      public struct None
      {
         internal static readonly None Default = default;
      }
      
      public static Maybe<T> FromNullable<T>(T? value)
         => new Maybe<T>(value);
   }
}