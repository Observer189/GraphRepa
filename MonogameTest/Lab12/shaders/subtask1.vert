#version 330 core
layout (location = 0) in vec3 coord;
layout (location = 1) in vec3 color;
layout (location = 2) in vec2 tex;

out vec4 FragColor;

uniform mat4 transformation;

void main()
{
    gl_Position = transformation * vec4(coord, 1.0);              
    FragColor = vec4(color, 1.0f);
}