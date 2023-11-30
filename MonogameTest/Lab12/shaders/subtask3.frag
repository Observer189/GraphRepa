#version 330 core
in vec2 FragTex;

out vec4 color;

uniform float portion;
uniform sampler2D ourTexture1;   
uniform sampler2D ourTexture2;   
void main()
{
    color = mix(texture(ourTexture1, FragTex), texture(ourTexture2, FragTex), portion);
}