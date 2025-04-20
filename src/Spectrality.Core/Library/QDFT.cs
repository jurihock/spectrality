using System.Runtime.InteropServices;

namespace Spectrality.Library;

public static partial class QDFT
{
  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_qdft_alloc")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  public static partial nint Alloc(
    double samplerate,
    double bandwidth_min,
    double bandwidth_max,
    double resolution,
    double quality);

  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_qdft_free")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  public static partial void Free(
    nint qdft);

  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_qdft_size")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  public static partial int Size(
    nint qdft);

  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_qdft_frequencies")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  public static partial void Frequencies(
    nint qdft,
    out double frequencies);

  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_qdft_analyze_decibel")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  public static partial void AnalyzeDecibel(
    nint qdft,
    in float samples,
    out float decibels,
    int count);
}
