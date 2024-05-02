# https://github.com/mackron/dr_libs

CPMAddPackage(
  NAME dr
  VERSION 2024.02.26
  GIT_TAG da35f9d6c7374a95353fd1df1d394d44ab66cf01
  GITHUB_REPOSITORY mackron/dr_libs
  DOWNLOAD_ONLY YES)

if(dr_ADDED)

  add_library(dr INTERFACE)

  target_include_directories(dr
    INTERFACE "${dr_SOURCE_DIR}")

  target_compile_definitions(dr
    INTERFACE -DDR_WAV_IMPLEMENTATION)

endif()
