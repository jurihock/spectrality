using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spectrality.Extensions;

public static class ArrayExtensions
{
  public static void Fill<T>(this T[,] array, T value)
  {
    ref var reference = ref MemoryMarshal.GetArrayDataReference(array);

    var span = MemoryMarshal.CreateSpan(
      ref Unsafe.As<byte, T>(ref reference),
      array.Length);

    span.Fill(value);
  }
}
