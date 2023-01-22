# https://github.com/dmilos/color

CPMAddPackage(
  NAME color
  VERSION master
  GIT_TAG master
  GITHUB_REPOSITORY dmilos/color
  DOWNLOAD_ONLY YES)

if(color_ADDED)

  add_library(color INTERFACE)

  target_include_directories(color
    INTERFACE "${color_SOURCE_DIR}/src")

endif()
