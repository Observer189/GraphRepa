#version 330 core

layout (location = 0) in vec3 coord;
layout (location = 1) in vec3 facenormal;
layout (location = 2) in vec2 tex;
layout (location = 3) in mat4 objectTransformation;
layout (location = 7) in mat3 normalTransformation;

uniform mat4 cameraTransformation;
uniform mat4 projection;
uniform vec4 cameraPos;
uniform vec4 plsPos;
uniform vec4 pjlsPos;

out Vertex {
    vec2 texCoord;
    vec3 faceNormal;
    vec3 plsLightDir;
    vec3 pjlsLightDir;
    float plsDistance;
    float pjlsDistance;
    vec3 viewDir;
} Vert;



void main()
{
    vec3 world_normal = normalTransformation * facenormal;
    vec4 vertex_pos = objectTransformation * vec4(coord, 1.0);

    mat4 resultmat = projection * cameraTransformation;
    gl_Position = resultmat * vertex_pos;              
    
    vec4 pls_light_dir = plsPos - vertex_pos;
    vec4 pjls_light_dir = pjlsPos - vertex_pos;

    Vert.faceNormal = world_normal;
    Vert.plsLightDir = vec3(pls_light_dir);
    Vert.plsDistance = length(pls_light_dir);
    Vert.pjlsLightDir = vec3(pjls_light_dir);
    Vert.pjlsDistance = length(pjls_light_dir);

    Vert.texCoord = vec2(tex.x, 1 - tex.y);
    Vert.viewDir = vec3(cameraPos - vertex_pos);
}