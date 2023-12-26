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

std::string stringFromFile(const char* path) {
	std::ifstream ifs(path);
	const std::string vsh((std::istreambuf_iterator<char>(ifs)), (std::istreambuf_iterator<char>()));
	return vsh;
}

// предназначен для обновления параметров света в шейдерах объекта 
void GameObject::lightUpdate(Scene& scene)
{
	checkOpenGLerror();

	auto ps = this->program.get();
	ps->Bind();

	auto plsPos_id = ps->GetUniformIdFallable("plsPos");
	if (plsPos_id != -1) {
		glUniform4fv(plsPos_id, 1, glm::value_ptr(scene.pls.lightsourcePos)); // отправляем в шейдер
	}
	auto plsIntencity_id = ps->GetUniformIdFallable("plsIntencity");
	if (plsIntencity_id != -1) {
		glUniform1f(plsIntencity_id, scene.pls.intencity);
	}

	auto pjlsPos_id = ps->GetUniformIdFallable("pjlsPos");
	if (pjlsPos_id != -1) {
		glUniform4fv(pjlsPos_id, 1, &scene.pjls.projectorPos[0]);
	}
	auto pjlsIntencity_id = ps->GetUniformIdFallable("pjlsIntencity");
	if (pjlsIntencity_id != -1) {
		glUniform1f(pjlsIntencity_id, scene.pjls.intencity);
	}
	auto pjlsAngle_id = ps->GetUniformIdFallable("pjlsAngle");
	if (pjlsAngle_id != -1) {
		glUniform1f(pjlsAngle_id, scene.pjls.angle);
	}
	auto pjlsDir_id = ps->GetUniformIdFallable("pjlsDir");
	if (pjlsDir_id != -1) {
		glUniform3fv(pjlsDir_id, 1, &scene.pjls.dir[0]);
	}

	auto glsIntencity_id = ps->GetUniformIdFallable("glsIntencity");
	if (glsIntencity_id != -1) {
		glUniform1f(glsIntencity_id, scene.gls.intencity);
	}
	auto glsDir_id = ps->GetUniformIdFallable("glsDir");
	if (glsDir_id != -1) {
		glUniform3fv(glsDir_id, 1, &scene.gls.dir[0]);
	}

	ps->Unbind(); // отвязка шейдера
	checkOpenGLerror();
}