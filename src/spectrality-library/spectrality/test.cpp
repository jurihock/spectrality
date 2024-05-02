#include <spectrality/test.h>

bool spectrality_test(const char* data, const size_t size)
{
  return std::string(data, data + size) == "spectrality";
}
