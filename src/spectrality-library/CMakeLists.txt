cmake_minimum_required(VERSION 3.20)

project(spectrality)

include("${CMAKE_CURRENT_LIST_DIR}/cmake/CPM.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/cmake/DR.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/cmake/PocketFFT.cmake")
include("${CMAKE_CURRENT_LIST_DIR}/cmake/QDFT.cmake")

add_subdirectory("${CMAKE_CURRENT_LIST_DIR}/spectrality")
