# https://github.com/mackron/dr_libs

CPMAddPackage(
  NAME dr
  VERSION 2025.04.19
  GIT_TAG 9cb7092ac8c75a82b5c6ea72652ca8d0091d7ffa
  GITHUB_REPOSITORY mackron/dr_libs
  DOWNLOAD_ONLY YES)

if(dr_ADDED)

  add_library(dr INTERFACE)

  target_include_directories(dr
    INTERFACE "${dr_SOURCE_DIR}")

  target_compile_definitions(dr
    INTERFACE -DDR_MP3_IMPLEMENTATION)

  target_compile_definitions(dr
    INTERFACE -DDR_WAV_IMPLEMENTATION)

endif()
