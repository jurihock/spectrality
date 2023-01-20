cmake_minimum_required(VERSION 3.12...3.18)

project(spectroscope)

if(MSVC)
  # add_compile_options(/W3 /WX)
else()
  # add_compile_options(-Wall -Werror)
endif()

include("${CMAKE_CURRENT_LIST_DIR}/lib/cpm.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/lib/dr.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/lib/ffmpeg.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/lib/opencv.cmake")

include("${CMAKE_CURRENT_LIST_DIR}/src/spectroscope/spectroscope.cmake")
