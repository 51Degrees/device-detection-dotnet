cmake_minimum_required(VERSION 3.18)
project(PerformanceTest)

include(ExternalProject)

set(EXTERNAL_INSTALL_LOCATION ${CMAKE_BINARY_DIR}/external)

ExternalProject_Add(ApacheBench
    GIT_REPOSITORY https://github.com/51degrees/apachebench
    CMAKE_ARGS -DCMAKE_INSTALL_PREFIX=${EXTERNAL_INSTALL_LOCATION}
    GIT_TAG origin/main
    STEP_TARGETS build
    EXCLUDE_FROM_ALL TRUE
)


function(download_file url filename)

if(NOT EXISTS ${filename})
    file(DOWNLOAD ${url} ${filename}
        TIMEOUT 60  # seconds
        TLS_VERIFY ON)
endif()

endfunction(download_file)


download_file(
    https://media.githubusercontent.com/media/51Degrees/device-detection-data/master/20000%20User%20Agents.csv
    ./uas.csv
)

add_custom_target(perf ALL
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_SOURCE_DIR}/runPerf.ps1 ./
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_SOURCE_DIR}/runPerf.sh ./
    DEPENDS ${CMAKE_SOURCE_DIR}/runPerf.ps1 ${CMAKE_SOURCE_DIR}/runPerf.sh

    VERBATIM
)

add_dependencies(perf ApacheBench-build)
