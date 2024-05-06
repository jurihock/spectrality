using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spectrality.Extensions;

public static class MemoryMarshalExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T Ref<T>(this T[] array) =>
    ref MemoryMarshal.GetArrayDataReference(array);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T Ref<T>(this Span<T> span) =>
    ref MemoryMarshal.GetReference(span);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref T Ref<T>(this ReadOnlySpan<T> span) =>
    ref MemoryMarshal.GetReference(span);
}
