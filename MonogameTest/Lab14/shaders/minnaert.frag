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

//Point light source
uniform float plsIntencity;


//Projector Light Source
uniform float pjlsIntencity;
uniform vec3 pjlsDir;
uniform float pjlsAngle;


//GlobalLightSource
uniform float glsIntencity;
uniform vec3 glsDir;


const float ATTENUATION_CONST = 0.1;
const float ATTENUATION_X1 = 0.5;
const float ATTENUATION_X2 = 0.5;
const vec4 MAT_EMISSION = vec4(0.05, 0.05, 0.05, 1);
const vec4 MAT_AMBIENT = vec4(0.1, 0.1, 0.1, 1);
const vec4 MAT_DIFFUSE = vec4(0.8, 0.8, 0.8, 1);
const vec4 MAT_SPECULAR = vec4(0.2, 0.2, 0.2, 1);
const float MAT_SHININESS = 1.0;
const float SPOT_EXPONENT = 80.0;

const float k = 0.8;

vec4 minnaert_model(vec3 faceNormal, vec3 LightDir, vec3 viewDir) 
{
    float d1 = pow(max(dot(faceNormal, LightDir), 0.0), 1.0 + k);
    float d2 = pow(1.0 - dot(faceNormal, viewDir), 1.0 - k);
    
    vec4 res_color = MAT_DIFFUSE * d1 * d2;
    if (dot(LightDir, faceNormal) > 0){
        float RdotVpow = max(pow(dot(reflect(-LightDir, faceNormal), viewDir), MAT_SHININESS), 0.0);
        res_color += MAT_SPECULAR * RdotVpow;
    }
    return res_color;
}

vec4 calculate_point_light(vec3 faceNormal, vec3 plsLightDir, vec3 viewDir) 
{
    float attenuation = plsIntencity / (ATTENUATION_CONST + ATTENUATION_X1 * Vert.plsDistance + ATTENUATION_X2 * Vert.plsDistance * Vert.plsDistance);
    vec4 res_color = MAT_AMBIENT * attenuation;
    res_color += minnaert_model(faceNormal, plsLightDir, viewDir) * attenuation;
    return res_color;
}
vec4 calculate_global_light(vec3 faceNormal, vec3 viewDir) 
{
    vec4 res_color = glsIntencity * minnaert_model(faceNormal, glsDir, viewDir);
    return res_color;
}
vec4 calculate_projector_light(vec3 faceNormal, vec3 pjlsLightDir, vec3 viewDir) 
{
    float spotEffect = dot(normalize(pjlsDir), pjlsLightDir);
    float spot = float(spotEffect > cos(pjlsAngle));
    float expSpotEffect = max(pow(spotEffect, SPOT_EXPONENT), 0.0);

    float attenuation = spot * expSpotEffect * pjlsIntencity / (ATTENUATION_CONST + ATTENUATION_X1 * Vert.pjlsDistance + ATTENUATION_X2 * Vert.pjlsDistance * Vert.pjlsDistance);
    vec4 res_color = attenuation * minnaert_model(faceNormal, pjlsLightDir, viewDir);
    return res_color;
}


void main()
{
    vec3 faceNormal = normalize(Vert.faceNormal);
    vec3 plsLightDir = normalize(Vert.plsLightDir);
    vec3 pjlsLightDir = normalize(Vert.pjlsLightDir);
    vec3 viewDir = normalize(Vert.viewDir);
    color = MAT_EMISSION;
    color += calculate_point_light(faceNormal, plsLightDir, viewDir);
    color += calculate_global_light(faceNormal, viewDir);
    color += calculate_projector_light(faceNormal, pjlsLightDir, viewDir);

    color *= texture(ourTexture, Vert.texCoord);
}