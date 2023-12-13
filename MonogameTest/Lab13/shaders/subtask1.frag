#version 330 core
in vec2 texCoord;

uniform sampler2D ourTexture;
out vec4 color;

void main()
{
    color = texture(ourTexture, vec2(texCoord.x, 1 - texCoord.y));
}