cmake_minimum_required(VERSION 3.12...3.18)

project(spectroscope)

if(MSVC)
  # add_compile_options(/W3 /WX)
else()
  # add_compile_options(-Wall -Werror)
endif()

include("${CMAKE_CURRENT_LIST_DIR}/lib/cpm.cmake")



# https://github.com/bartjoyce/video-app/issues/4
# https://ffmpeg.org/doxygen/trunk/index.html

# find_package(FFmpeg REQUIRED)

find_package(PkgConfig REQUIRED)

pkg_check_modules(AVCODEC    REQUIRED IMPORTED_TARGET libavcodec)
pkg_check_modules(AVFORMAT   REQUIRED IMPORTED_TARGET libavformat)
pkg_check_modules(AVUTIL     REQUIRED IMPORTED_TARGET libavutil)
pkg_check_modules(SWRESAMPLE REQUIRED IMPORTED_TARGET libswresample)
pkg_check_modules(SWSCALE    REQUIRED IMPORTED_TARGET libswscale)

add_library(FFmpeg INTERFACE)

target_link_libraries(FFmpeg INTERFACE
  PkgConfig::AVCODEC
  PkgConfig::AVFORMAT
  PkgConfig::AVUTIL
  PkgConfig::SWRESAMPLE
  PkgConfig::SWSCALE)



include("${CMAKE_CURRENT_LIST_DIR}/src/spectroscope/spectroscope.cmake")
