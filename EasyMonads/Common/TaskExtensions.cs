using System.Threading.Tasks;

namespace EasyMonads
{
   public static class TaskExtensions
   {
      public static Task<T> AsTask<T>(this T self)
      {
         return Task.FromResult(self);
      }
   }
}
