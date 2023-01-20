# https://docs.opencv.org/4.x

find_package(PkgConfig REQUIRED)

pkg_check_modules(OPENCV4 REQUIRED IMPORTED_TARGET opencv4)

add_library(opencv INTERFACE)

target_link_libraries(opencv INTERFACE
  PkgConfig::OPENCV4)

