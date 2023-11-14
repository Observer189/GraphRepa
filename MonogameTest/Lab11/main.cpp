//TODO: Готовая заготовка под лабараторную задачу

#include <iostream>
#include <fstream>
#include <string>

#include <GL/glew.h>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <random>

GLuint simpleShader, uniformShader, gradientShader;
GLuint Program;
// ID атрибута
GLint Attrib_vertex;
// ID Vertex Buffer Object
GLuint VBO,VBOQuad, VBOFan, VBOPentagon;
glm::vec4 color = glm::vec4(1.0f, 0.0f, 1.0f, 1.0f);

int shapeType = 0;
int shaderType = 0;

const char* attr_coord_name = "coord"; //имя в шейдере

struct Vertex {
	GLfloat x;
	GLfloat y;

	GLfloat r;
	GLfloat g;
	GLfloat b;
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
	ifs = std::ifstream("gradient.vert");
	const std::string vsh2((std::istreambuf_iterator<char>(ifs)),
		(std::istreambuf_iterator<char>()));
	auto GradVertShaderSource = vsh2.c_str();
	std::ifstream ifs2("shader.frag");
	const std::string fsh((std::istreambuf_iterator<char>(ifs2)),
		(std::istreambuf_iterator<char>()));
	auto FragShaderSource = fsh.c_str();

	ifs2 = std::ifstream("uniform.frag");
	const std::string fsh2((std::istreambuf_iterator<char>(ifs2)),
		(std::istreambuf_iterator<char>()));
	auto UniformFragShaderSource = fsh2.c_str();

	ifs2 = std::ifstream("gradient.frag");
	const std::string fsh3((std::istreambuf_iterator<char>(ifs2)),
		(std::istreambuf_iterator<char>()));
	auto GradientFragShaderSource = fsh3.c_str();


	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	GLuint gradVertShader = glCreateShader(GL_VERTEX_SHADER);
	// Передаем исходный код
	glShaderSource(vShader, 1, &VertexShaderSource, NULL);
	glShaderSource(gradVertShader, 1, &GradVertShaderSource, NULL);
	// Компилируем шейдер
	glCompileShader(vShader);
	glCompileShader(gradVertShader);
	std::cout << "vertex shader \n";
	// Функция печати лога шейдера
	ShaderLog(vShader);
	ShaderLog(gradVertShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	GLuint fUniformShader = glCreateShader(GL_FRAGMENT_SHADER);
	GLuint fGradientShader = glCreateShader(GL_FRAGMENT_SHADER);
	// Передаем исходный код
	glShaderSource(fShader, 1, &FragShaderSource, NULL);
	glShaderSource(fUniformShader, 1, &UniformFragShaderSource, NULL);
	glShaderSource(fGradientShader, 1, &GradientFragShaderSource, NULL);
	// Компилируем шейдер
	glCompileShader(fShader);
	glCompileShader(fUniformShader);
	glCompileShader(fGradientShader);
	std::cout << "fragment shader \n";
	// Функция печати лога шейдера
	ShaderLog(fShader);

	simpleShader = glCreateProgram();
	uniformShader = glCreateProgram();
	gradientShader = glCreateProgram();

	glAttachShader(simpleShader, vShader);
	glAttachShader(simpleShader, fShader);

	glAttachShader(uniformShader, vShader);
	glAttachShader(uniformShader, fUniformShader);

	glAttachShader(gradientShader, gradVertShader);
	glAttachShader(gradientShader, fGradientShader);
	// Линкуем шейдерную программу
	glLinkProgram(simpleShader);
	glLinkProgram(uniformShader);
	glLinkProgram(gradientShader);
	// Проверяем статус сборки
	int link_ok;
	glGetProgramiv(gradientShader, GL_LINK_STATUS, &link_ok);
	if (!link_ok) {
		std::cout << "error attach shaders \n";
		return;
	}
	// Вытягиваем ID атрибута из собранной программы
	Attrib_vertex = glGetAttribLocation(simpleShader, attr_coord_name);
	if (Attrib_vertex == -1) {
		std::cout << "could not bind attrib " << attr_coord_name << std::endl;
		return;
	}
	checkOpenGLerror();
}
void InitVBO() {
	glGenBuffers(1, &VBO);
	glGenBuffers(1, &VBOQuad);
	glGenBuffers(1, &VBOFan);
	glGenBuffers(1, &VBOPentagon);
	// Вершины нашего треугольника
	Vertex triangle[3] = {
	{ -0.5f, -0.5f, 1.0f, 0.0f, 0.0f },
	{ 0.5f, -0.5f, 0.0f, 1.0f, 0.0f },
	{ 0.5f, 0.5f, 0.0f, 0.0f , 1.0f },
	};

	Vertex quad[4] = {
	{ -0.5f, -0.5f , 1.0f, 0.0f, 0.0f},
	{ 0.5f, -0.5f , 0.0f, 1.0f, 0.0f},
	{ 0.5f, 0.5f, 0.0f, 0.0f , 1.0f},
	{-0.5f,0.5f, 1.0f, 1.0f, 0.0f},
	};

	Vertex fan[8] = {
	{ 0.0f, 0.0f, 1.0f, 1.0f, 1.0f },
	};

	Vertex pentagon[5] = { };

	std::random_device rd;
	std::mt19937 e2(rd());
	std::uniform_real_distribution<> dist(0, 1);

	glm::vec4 point = glm::vec4(0.6f,0.0f,0.0f,1.0f);
	glm::mat4 rotMat = glm::mat4(1.0f);
	rotMat = glm::rotate(rotMat, glm::radians(30.0f), glm::vec3(0.0, 0.0, 1.0));
	for (size_t i = 1; i < 8; i++)
	{
		Vertex vert;
		vert.x = point.x;
		vert.y = point.y;
		vert.r = dist(e2);
		vert.g = dist(e2);
		vert.b = dist(e2);
		fan[i] = vert;
		point = rotMat * point;
	}

	point = glm::vec4(0.3f, 0.5f, 0.0f, 1.0f);
	rotMat = glm::mat4(1.0f);
	rotMat = glm::rotate(rotMat, glm::radians(72.0f), glm::vec3(0.0, 0.0, 1.0));
	for (size_t i = 0; i < 5; i++)
	{
		Vertex vert;
		vert.x = point.x;
		vert.y = point.y;
		vert.r = dist(e2);
		vert.g = dist(e2);
		vert.b = dist(e2);
		pentagon[i] = vert;
		point = rotMat * point;
	}
	// Передаем вершины в буфер
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(triangle), triangle, GL_STATIC_DRAW);

	glBindBuffer(GL_ARRAY_BUFFER, VBOQuad);
	glBufferData(GL_ARRAY_BUFFER, sizeof(quad), quad, GL_STATIC_DRAW);

	glBindBuffer(GL_ARRAY_BUFFER, VBOFan);
	glBufferData(GL_ARRAY_BUFFER, sizeof(fan), fan, GL_STATIC_DRAW);

	glBindBuffer(GL_ARRAY_BUFFER, VBOPentagon);
	glBufferData(GL_ARRAY_BUFFER, sizeof(pentagon), pentagon, GL_STATIC_DRAW);
	checkOpenGLerror(); //Пример функции есть в лабораторной
	// Проверка ошибок OpenGL, если есть, то вывод в консоль тип ошибки
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
	switch (shaderType)
	{
	case 0: Program = simpleShader; 
		break;
	case 1: Program = uniformShader;
		break;
	case 2: Program = gradientShader;
		break;
	default:
		break;
	}
	int uniformColorLocation = glGetUniformLocation(Program, "uColor");
	glUseProgram(Program); // Устанавливаем шейдерную программу текущей
	if (shaderType == 1)
		glUniform4f(uniformColorLocation, color.r,color.g,color.b,color.a);
	glEnableVertexAttribArray(Attrib_vertex); // Включаем массив атрибутов
	if (shapeType == 0)
	{
		glBindBuffer(GL_ARRAY_BUFFER, VBO); // Подключаем VBO
		// Указывая pointer 0 при подключенном буфере, мы указываем что данные в VBO
		glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), 0);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));
		glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO
		glDrawArrays(GL_TRIANGLES, 0, 3); // Передаем данные на видеокарту(рисуем)
	}
	else if (shapeType == 1)
	{
		glBindBuffer(GL_ARRAY_BUFFER, VBOQuad); // Подключаем VBO
		// Указывая pointer 0 при подключенном буфере, мы указываем что данные в VBO
		glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 5*sizeof(float), 0);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));
		glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO
		glDrawArrays(GL_QUADS, 0, 4); // Передаем данные на видеокарту(рисуем)
	}
	else if(shapeType == 2)
	{
		glBindBuffer(GL_ARRAY_BUFFER, VBOFan); // Подключаем VBO
		// Указывая pointer 0 при подключенном буфере, мы указываем что данные в VBO
		glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), 0);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));
		glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO
		glDrawArrays(GL_TRIANGLE_FAN, 0, 8); // Передаем данные на видеокарту(рисуем)
	}
	else
	{
		glBindBuffer(GL_ARRAY_BUFFER, VBOPentagon); // Подключаем VBO
		// Указывая pointer 0 при подключенном буфере, мы указываем что данные в VBO
		glVertexAttribPointer(Attrib_vertex, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), 0);
		glEnableVertexAttribArray(1);
		glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));
		glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO
		glDrawArrays(GL_TRIANGLE_FAN, 0, 5); // Передаем данные на видеокарту(рисуем)
	}

	glDisableVertexAttribArray(Attrib_vertex);
	glDisableVertexAttribArray(1);// Отключаем массив атрибутов
	glUseProgram(0); // Отключаем шейдерную программу
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

			if (event.type == sf::Event::EventType::KeyPressed) {
				if (event.key.code == sf::Keyboard::Right) {
					shapeType++;
					if (shapeType > 3) shapeType = 0;
				}
				else if (event.key.code == sf::Keyboard::Left) {
					shapeType--;
					if (shapeType < 0) shapeType = 3;
				}
				else if (event.key.code == sf::Keyboard::Up) {
					shaderType++;
					if (shaderType > 2) shaderType = 0;
				}
				else if (event.key.code == sf::Keyboard::Down) {
					shaderType--;
					if (shaderType < 0) shaderType = 2;
				}
			}
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