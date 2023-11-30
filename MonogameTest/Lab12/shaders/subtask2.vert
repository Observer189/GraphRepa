#version 330 core
layout (location = 0) in vec3 coord;
layout (location = 1) in vec3 color;
layout (location = 2) in vec2 tex;

out vec4 FragColor;
out vec2 FragTex;

#define M_PI 3.1415926535897932384626433832795


float angle_x = M_PI / 4;
float angle_y = M_PI / 4;
//float angle_y = 0;



void main()
{
    mat4 rotY = mat4(cos(angle_x), 0.0, sin(angle_x), 0.0,
                       0.0, 1.0, 0.0, 0.0,
                      -sin(angle_x), 0.0, cos(angle_x), 0.0,
                       0.0, 0.0, 0.0, 1.0);
    mat4 rotX = mat4(1.0, 0.0, 0.0, 0.0,
                       0.0, cos(angle_y), -sin(angle_y), 0.0,
                       0.0, sin(angle_y), cos(angle_y), 0.0,
                       0.0, 0.0, 0.0, 1.0);
    gl_Position = rotY * rotX * vec4(coord, 1.0);
                  
    FragColor = vec4(color, 1.0f);
    FragTex = tex;
}