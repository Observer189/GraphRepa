//TODO: ������� ��������� ��� ������������ ������

#include <iostream>
#include <fstream>
#include <string>

#include <GL/glew.h>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>


GLuint Program;
// ID ��������
GLint Attrib_vertex;
// ID Vertex Buffer Object
GLuint VBO;


struct Vertex {
	GLfloat x;
	GLfloat y;
};

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

void ShaderLog(unsigned int shader)
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

void InitShader() {
	std::ifstream ifs("shader.vert");
	const std::string vsh((std::istreambuf_iterator<char>(ifs)),
		(std::istreambuf_iterator<char>()));
	auto VertexShaderSource = vsh.c_str();
	std::ifstream ifs2("shader.frag");
	const std::string fsh((std::istreambuf_iterator<char>(ifs2)),
		(std::istreambuf_iterator<char>()));
	auto FragShaderSource = fsh.c_str();


	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	// �������� �������� ���
	glShaderSource(vShader, 1, &VertexShaderSource, NULL);
	// ����������� ������
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	// ������� ������ ���� �������
	ShaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	// �������� �������� ���
	glShaderSource(fShader, 1, &FragShaderSource, NULL);
	// ����������� ������
	glCompileShader(fShader);
	std::cout << "fragment shader \n";
	// ������� ������ ���� �������
	ShaderLog(fShader);

	Program = glCreateProgram();
	glAttachShader(Program, vShader);
	glAttachShader(Program, fShader);
	// ������� ��������� ���������
	glLinkProgram(Program);
	// ��������� ������ ������
	int link_ok;
	glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);
	if (!link_ok) {
		std::cout << "error attach shaders \n";
		return;
	}
	// ���������� ID �������� �� ��������� ���������
	const char* attr_name = "coord"; //��� � �������
	Attrib_vertex = glGetAttribLocation(Program, attr_name);
	if (Attrib_vertex == -1) {
		std::cout << "could not bind attrib " << attr_name << std::endl;
		return;
	}
	checkOpenGLerror();
}
void InitVBO() {
	glGenBuffers(1, &VBO);
	// ������� ������ ������������
	Vertex triangle[3] = {
	{ -1.0f, -1.0f },
	{ 0.0f, 1.0f },
	{ 1.0f, -1.0f }
	};
	// �������� ������� � �����
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(triangle), triangle, GL_STATIC_DRAW);
	checkOpenGLerror(); //������ ������� ���� � ������������
	// �������� ������ OpenGL, ���� ����, �� ����� � ������� ��� ������
}

void Init() {
	glEnable(GL_DEBUG_OUTPUT);
	glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	InitShader();
	InitVBO();
}
void Release() {
	glUseProgram(0);
	glDeleteProgram(Program);

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &VBO);
}

void Draw() {
	glUseProgram(Program); // ������������� ��������� ��������� �������
	glEnableVertexAttribArray(Attrib_vertex); // �������� ������ ���������
	glBindBuffer(GL_ARRAY_BUFFER, VBO); // ���������� VBO
	// �������� pointer 0 ��� ������������ ������, �� ��������� ��� ������ � VBO
	glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 0, 0);
	glBindBuffer(GL_ARRAY_BUFFER, 0); // ��������� VBO
	glDrawArrays(GL_TRIANGLES, 0, 3); // �������� ������ �� ����������(������)
	glDisableVertexAttribArray(Attrib_vertex); // ��������� ������ ���������
	glUseProgram(0); // ��������� ��������� ���������
	checkOpenGLerror();
}
int main() {
	sf::Window window(sf::VideoMode(600, 600), "My OpenGL window", sf::Style::Default, sf::ContextSettings(24));
	window.setVerticalSyncEnabled(true);
	window.setActive(true);
	glewInit();
	Init();
	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }
		}
		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			Draw();
			window.display();
		}
	}
	Release();
	return 0;
}