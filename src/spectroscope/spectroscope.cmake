add_executable(${PROJECT_NAME})

file(GLOB_RECURSE
  HDR "${CMAKE_CURRENT_LIST_DIR}/*.h")

file(GLOB_RECURSE
  SRC "${CMAKE_CURRENT_LIST_DIR}/*.cpp")

target_sources(${PROJECT_NAME}
  PRIVATE "${HDR}" "${SRC}")

target_include_directories(${PROJECT_NAME}
  PRIVATE "${CMAKE_CURRENT_LIST_DIR}/..")

target_link_libraries(${PROJECT_NAME}
  PRIVATE ffmpeg)

target_compile_features(${PROJECT_NAME}
  PRIVATE cxx_std_20)

if (MSVC)
  target_compile_options(${PROJECT_NAME}
    PRIVATE /fp:fast)
else()
  target_compile_options(${PROJECT_NAME}
    PRIVATE -ffast-math)
endif()
