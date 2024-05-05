using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spectrality.Extensions;

public static class AsRefExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this T[] array, int offset = 0)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(offset, nameof(offset));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, array.Length, nameof(offset));

    return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), offset);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this Span<T> span, int offset = 0)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(offset, nameof(offset));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, span.Length, nameof(offset));

    return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T AsRef<T>(this ReadOnlySpan<T> span, int offset = 0)
  {
    ArgumentOutOfRangeException.ThrowIfNegative(offset, nameof(offset));
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, span.Length, nameof(offset));

    return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
  }
}
