using System.Runtime.InteropServices;
using System.Text;

namespace Spectrality.Library;

public static class Test
{
  [DllImport(
    Import.FileName,
    CharSet = Import.DefaultCharSet,
    CallingConvention = Import.DefaultCallingConvention,
    EntryPoint = "spectrality_test")]
  public static extern bool SpectralityTest(
    byte[] data, nuint size);

  public static bool SpectralityTest()
  {
    var bytes = Encoding.UTF8.GetBytes("spectrality");
    var size = (nuint)bytes.Length;

    return SpectralityTest(bytes, size);
  }
}
