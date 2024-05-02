# https://gitlab.mpcdf.mpg.de/mtr/pocketfft

CPMAddPackage(
  NAME pocketfft
  VERSION 2024.03.22
  GIT_TAG 33ae5dc94c9cdc7f1c78346504a85de87cadaa12
  GIT_REPOSITORY https://gitlab.mpcdf.mpg.de/mtr/pocketfft
  DOWNLOAD_ONLY YES)

if(pocketfft_ADDED)

  add_library(pocketfft INTERFACE)

  target_include_directories(pocketfft
    INTERFACE "${pocketfft_SOURCE_DIR}")

  target_compile_definitions(pocketfft
    INTERFACE -DPOCKETFFT_NO_MULTITHREADING)

  target_compile_definitions(pocketfft
    INTERFACE -DPOCKETFFT_CACHE_SIZE=10)

  if(UNIX)
    target_link_libraries(pocketfft
      INTERFACE pthread)
  endif()

endif()
