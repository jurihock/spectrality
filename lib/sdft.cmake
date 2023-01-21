# https://github.com/jurihock/sdft

CPMAddPackage(
  NAME sdft
  VERSION main
  GIT_TAG main
  GITHUB_REPOSITORY jurihock/sdft
  DOWNLOAD_ONLY YES)

if(sdft_ADDED)

  add_library(sdft INTERFACE)

  target_include_directories(sdft
    INTERFACE "${sdft_SOURCE_DIR}/src/cpp")

endif()
