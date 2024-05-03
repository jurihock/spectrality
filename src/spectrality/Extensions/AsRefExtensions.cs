using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spectrality.Extensions;

public static class AsRefExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this T[] array, int offset = 0)
  {
    if (offset < 0 || offset >= array.Length)
    {
      throw new ArgumentOutOfRangeException(nameof(offset));
    }

    return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), offset);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this Span<T> span, int offset = 0)
  {
    if (offset < 0 || offset >= span.Length)
    {
      throw new ArgumentOutOfRangeException(nameof(offset));
    }

    return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this ReadOnlySpan<T> span, int offset = 0)
  {
    if (offset < 0 || offset >= span.Length)
    {
      throw new ArgumentOutOfRangeException(nameof(offset));
    }

    return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
  }
}
