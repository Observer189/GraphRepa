#include "obj_defs.h";

void checkOpenGLerror() {
	GLenum err;
	int count = 0;
	while ((err = glGetError()) != GL_NO_ERROR)
	{
		auto err_text = glewGetErrorString(err);
		std::cout << "Error: " << err << ": " << err_text << "\n";
		// Process/log the error.
		if (count++ > 100) break;
	}
}
void shaderLog(GLuint shader)
{
	int infologLen = 0;
	glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);
	if (infologLen > 1)
	{
		int charsWritten = 0;
		std::vector<char> infoLog(infologLen);
		glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog.data());
		std::cout << "InfoLog: " << infoLog.data() << std::endl;
	}
}
