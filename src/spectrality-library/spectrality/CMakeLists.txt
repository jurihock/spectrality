cmake_minimum_required(VERSION 3.20)

project(spectrality)

file(GLOB_RECURSE HEADERS
  "${CMAKE_CURRENT_LIST_DIR}/*.h")

file(GLOB_RECURSE SOURCES
  "${CMAKE_CURRENT_LIST_DIR}/*.cpp")

add_library(${PROJECT_NAME} SHARED)

target_sources(${PROJECT_NAME}
  PRIVATE ${HEADERS} ${SOURCES})

target_include_directories(${PROJECT_NAME}
  PRIVATE "${CMAKE_CURRENT_LIST_DIR}/..")

target_compile_features(${PROJECT_NAME}
  PRIVATE cxx_std_20)

if(MSVC)
  target_compile_options(${PROJECT_NAME}
    PRIVATE /fp:fast)
else()
  target_compile_options(${PROJECT_NAME}
    PRIVATE -ffast-math)
endif()

target_link_libraries(${PROJECT_NAME}
  PRIVATE qdft)

set_target_properties(${PROJECT_NAME}
  PROPERTIES OUTPUT_NAME "Spectrality.Library")

set_target_properties(${PROJECT_NAME}
  PROPERTIES PREFIX "" SUFFIX ".dll")
