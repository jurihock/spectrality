using System.Runtime.InteropServices;

namespace Spectrality.Library;

public static class QDFT
{
  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_qdft_alloc")]
  public static extern nint Alloc(
    double samplerate,
    double bandwidth_min,
    double bandwidth_max,
    double resolution,
    double quality);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_qdft_free")]
  public static extern void Free(
    nint qdft);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_qdft_size")]
  public static extern int Size(
    nint qdft);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_qdft_frequencies")]
  public static extern void Frequencies(
    nint qdft,
    double[] frequencies);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_qdft_analyze_decibel")]
  public static extern void AnalyzeDecibel(
    nint qdft,
    float[] samples,
    float[] decibels,
    int offset,
    int size);
}
