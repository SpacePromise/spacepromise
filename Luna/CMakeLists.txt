cmake_minimum_required(VERSION 3.15)
project(Luna)

set(CMAKE_CXX_STANDARD 14)

add_executable(Luna main.cpp Vulkan/VulkanInstance.cpp Vulkan/VulkanInstance.h)

find_package(Vulkan REQUIRED)
target_include_directories(${PROJECT_NAME} PUBLIC ${Vulkan_INCLUDE_DIRS})
target_link_libraries(${PROJECT_NAME} Vulkan::Vulkan)

find_package(glfw3 REQUIRED)
include_directories(${GLFW_INCLUDE_DIRS})
target_link_libraries(${PROJECT_NAME} glfw)

find_package(glm REQUIRED)
target_link_libraries(${PROJECT_NAME} glm)