using System.Runtime.InteropServices;

namespace Spectrality.Library;

public static partial class Audio
{
  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_audio_touch")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static partial bool Touch(
    [In] byte[] pathchars,
    int pathsize,
    out double samplerate,
    out int channels,
    out int frames);

  [LibraryImport("Spectrality.Library.dll", EntryPoint = "spectrality_audio_read")]
  [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static partial bool Read(
    [In] byte[] pathchars,
    int pathsize,
    out float samples,
    ref int frames);
}
