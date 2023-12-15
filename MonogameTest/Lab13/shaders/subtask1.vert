#version 330 core
layout (location = 0) in vec3 coord;
layout (location = 1) in vec2 tex;

in mat4 additionalTransformation;
out vec2 texCoord;

uniform mat4 cameraTransformation;
uniform mat4 projection;

void main()
{
    mat4 resultmat = projection * cameraTransformation * additionalTransformation;
    gl_Position = resultmat * vec4(coord, 1.0);              
    texCoord = tex;
}