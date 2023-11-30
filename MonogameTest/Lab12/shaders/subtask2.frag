#version 330 core
in vec4 FragColor;
in vec2 FragTex;

out vec4 color;

uniform float portion;
uniform sampler2D ourTexture;   

void main()
{
    color = mix(texture(ourTexture, FragTex), FragColor, portion);
    //color = texture(ourTexture, FragTex);
    //color = vec4(FragTex, 0.0, 1.0);
}