using System.Runtime.InteropServices;

namespace Spectrality.Library;

public static class Audio
{
  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_audio_touch")]
  public static extern void Touch(
    byte[] pathchars,
    int pathsize,
    out double samplerate,
    out int channels,
    out int frames);

  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_audio_read")]
  public static extern void Read(
    byte[] pathchars,
    int pathsize,
    float[] samples);
}
