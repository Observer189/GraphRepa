#version 330 core

in Vertex {
    vec2 texCoord;
    vec3 faceNormal;
    vec3 plsLightDir;
    vec3 pjlsLightDir;
    float plsDistance;
    float pjlsDistance;
    vec3 viewDir;
} Vert;


uniform sampler2D ourTexture;
out vec4 color;

uniform float plsIntencity;
uniform float pjlsIntencity;
uniform vec3 pjlsDir;
uniform float pjlsAngle;
uniform float glsIntencity;
uniform vec3 glsDir;


const float ATTENUATION_CONST = 0.1;
const float ATTENUATION_X1 = 0.5;
const float ATTENUATION_X2 = 0.5;
const vec4 MAT_EMISSION = vec4(0.05, 0.05, 0.05, 1);
const vec4 MAT_AMBIENT = vec4(0.1, 0.1, 0.1, 1);
const vec4 MAT_DIFFUSE = vec4(0.5, 0.5, 0.5, 1);
const vec4 MAT_SPECULAR = vec4(0.5, 0.5, 0.5, 1);
const float MAT_SHININESS = 1.0;
const float SPOT_EXPONENT = 80.0;


// Эта функция используется в шейдере для расчета цвета поверхности, основываясь на взаимодействии света и материала.
// вычисляет компоненты диффузного и зеркального отражения света на поверхности.
vec4 phong_light_model(vec3 faceNormal, vec3 LightDir, vec3 viewDir) 
{
    float Ndot = max(dot(faceNormal, LightDir), 0.0);
    vec4 res_color = MAT_DIFFUSE * Ndot;
    if (dot(LightDir, faceNormal) > 0){
        float RdotVpow = max(pow(dot(reflect(-LightDir, faceNormal), viewDir), MAT_SHININESS), 0.0);
        res_color += MAT_SPECULAR * RdotVpow;
    }
    return res_color;
}

// вычисляет освещение от точечного источника света, взаимодействующего с поверхностью
// Включает в себя учет диффузной, зеркальной компонент освещения, а также рассеивание.
vec4 calculate_point_light(vec3 faceNormal, vec3 plsLightDir, vec3 viewDir) 
{
    float attenuation = plsIntencity / (ATTENUATION_CONST + ATTENUATION_X1 * Vert.plsDistance + ATTENUATION_X2 * Vert.plsDistance * Vert.plsDistance);
    vec4 res_color = MAT_AMBIENT * attenuation;
    res_color += phong_light_model(faceNormal, plsLightDir, viewDir) * attenuation;
    return res_color;
}
// вычисляет освещение от глобального источника света, взаимодействующего с поверхностью.
// Включает в себя учет диффузной и зеркальной компонент освещения.
vec4 calculate_global_light(vec3 faceNormal, vec3 viewDir) 
{
    vec4 res_color = glsIntencity * phong_light_model(faceNormal, glsDir, viewDir);
    return res_color;
}
vec4 calculate_projector_light(vec3 faceNormal, vec3 pjlsLightDir, vec3 viewDir) 
{
    float spotEffect = dot(normalize(pjlsDir), pjlsLightDir);
    float spot = float(spotEffect > cos(pjlsAngle));
    float expSpotEffect = max(pow(spotEffect, SPOT_EXPONENT), 0.0);

    float attenuation = spot * expSpotEffect * pjlsIntencity / (ATTENUATION_CONST + ATTENUATION_X1 * Vert.pjlsDistance + ATTENUATION_X2 * Vert.pjlsDistance * Vert.pjlsDistance);
    vec4 res_color = attenuation * phong_light_model(faceNormal, pjlsLightDir, viewDir);
    return res_color;
}


void main()
{
    vec3 faceNormal = normalize(Vert.faceNormal);
    vec3 plsLightDir = normalize(Vert.plsLightDir);
    vec3 pjlsLightDir = normalize(Vert.pjlsLightDir);
    vec3 viewDir = normalize(Vert.viewDir);
    color = MAT_EMISSION; // цвет излучения материала
    color += calculate_point_light(faceNormal, plsLightDir, viewDir);
    color += calculate_global_light(faceNormal, viewDir);
    color += calculate_projector_light(faceNormal, pjlsLightDir, viewDir);

    color *= texture(ourTexture, Vert.texCoord);
}