# https://ffmpeg.org/doxygen/trunk

find_package(PkgConfig REQUIRED)

pkg_check_modules(AVCODEC    REQUIRED IMPORTED_TARGET libavcodec)
pkg_check_modules(AVFORMAT   REQUIRED IMPORTED_TARGET libavformat)
pkg_check_modules(AVUTIL     REQUIRED IMPORTED_TARGET libavutil)
pkg_check_modules(SWRESAMPLE REQUIRED IMPORTED_TARGET libswresample)
pkg_check_modules(SWSCALE    REQUIRED IMPORTED_TARGET libswscale)

add_library(ffmpeg INTERFACE)

target_link_libraries(ffmpeg INTERFACE
  PkgConfig::AVCODEC
  PkgConfig::AVFORMAT
  PkgConfig::AVUTIL
  PkgConfig::SWRESAMPLE
  PkgConfig::SWSCALE)
