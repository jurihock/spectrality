using System.Runtime.InteropServices;

namespace Spectrality.Library;

public static class Audio
{
  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_audio_touch")]
  public static extern bool Touch(
    byte[] pathchars,
    int pathsize,
    out double samplerate,
    out uint channels,
    out ulong frames);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_audio_read")]
  public static extern bool Read(
    byte[] pathchars,
    int pathsize,
    float[] samples,
    ref ulong frames);
}
