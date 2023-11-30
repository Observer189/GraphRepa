#pragma once
#include <iostream>
#include <fstream>
#include <string>

#include <GL/glew.h>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
//#include <glm/glm.hpp>
//#include <glm/gtc/matrix_transform.hpp>
//#include <glm/gtc/type_ptr.hpp>
#include <lodepng.h>
#include <random>
#include <stdexcept>

void checkOpenGLerror();
void shaderLog(GLuint shader);
std::string stringFromFile(const char* path);

struct Vertex {
	//Пространственные координаты
	GLfloat x;
	GLfloat y;
	GLfloat z;
	//Цветовые координаты
	GLfloat r;
	GLfloat g;
	GLfloat b;
	//Текстурные координаты

	//x-координата текстуры
	GLfloat u;
	//y-координата текстуры
	GLfloat v;
};

class VertexShader {
	GLuint id = 0;
public:
	VertexShader(const VertexShader&) = delete;
	VertexShader(VertexShader&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	VertexShader& operator=(VertexShader&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	VertexShader(std::string& vertex_shader) {
		auto v = vertex_shader.c_str();
		//std::cout << v << '\n';
		GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vShader, 1, &v, 0);
		glCompileShader(vShader);
		shaderLog(vShader);
		id = vShader;
		checkOpenGLerror();
	}
	VertexShader() {}
	~VertexShader()
	{
		glDeleteShader(id);
	}
};
class FragmentShader {
	GLuint id = 0;
public:
	FragmentShader(const FragmentShader&) = delete;
	FragmentShader(FragmentShader&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	FragmentShader& operator=(FragmentShader&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	FragmentShader(std::string& fragment_shader) {
		auto v = fragment_shader.c_str();
		GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fShader, 1, &v, 0);
		glCompileShader(fShader);
		shaderLog(fShader);

		id = fShader;
		checkOpenGLerror();
	}
	FragmentShader() {}
	~FragmentShader()
	{
		glDeleteShader(id);
	}
};

class ProgramShader {
	std::shared_ptr<VertexShader> vert_s;
	std::shared_ptr<FragmentShader> frag_s;
	GLuint id = 0;
public: 
	ProgramShader(const ProgramShader&) = delete;
	ProgramShader(ProgramShader&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	ProgramShader& operator=(ProgramShader&& other) noexcept {
		id = other.id;
		vert_s = other.vert_s;
		frag_s = other.frag_s;
		other.id = 0;

		return *this;
	}
	GLuint Id() const {
		return id;
	}
	ProgramShader(std::shared_ptr<VertexShader> vs, std::shared_ptr<FragmentShader> fs) {
		vert_s = vs;
		frag_s = fs;
		id = glCreateProgram();
		auto vs_id = vs.get()->Id();
		auto fs_id = fs.get()->Id();
		glAttachShader(id, vs_id);
		glAttachShader(id, fs_id);
		glLinkProgram(id);
		// Проверяем статус сборки
		int link_ok;
		glGetProgramiv(id, GL_LINK_STATUS, &link_ok);
		if (!link_ok) {
			checkOpenGLerror();
			std::cout << "error attach shaders \n";
			throw std::invalid_argument("Program not linked properly!");
		}
	}
	ProgramShader(){}
	void Bind() const {
		glUseProgram(id);
	}
	static void Unbind() {
		glUseProgram(0);
	}
	~ProgramShader()
	{
		glDeleteProgram(id);
	}
};


class VBO {
	GLuint id = 0;
public:
	VBO(const VBO&) = delete;
	VBO(VBO&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	VBO& operator=(VBO&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	VBO() {
		glGenBuffers(1, &id);
	}
	~VBO()
	{
		glDeleteBuffers(1, &id);
	}
	void Bind() const {
		glBindBuffer(GL_ARRAY_BUFFER, id);
	}
	static void Unbind() {
		glBindBuffer(GL_ARRAY_BUFFER, 0);
	}
};
class VAO {
	GLuint id = 0;
public:
	VAO(const VAO&) = delete;
	VAO(VAO&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	VAO& operator=(VAO&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	VAO() {
		glGenVertexArrays(1, &id);

	}
	~VAO()
	{
		glDeleteVertexArrays(1, &id);
	}
	void Bind() const {
		glBindVertexArray(id);
	}
	static void Unbind() {
		glBindVertexArray(0);
	}
};
//Element Buffer Object или Index Buffer Object
class EBO {
	GLuint id = 0;
public:
	EBO(const EBO&) = delete;
	EBO(EBO&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	EBO& operator=(EBO&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	EBO() {
		glGenBuffers(1, &id);

	}
	~EBO()
	{
		glDeleteBuffers(1, &id);
	}
	void Bind() const {
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, id);
	}
	static void Unbind() {
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
	}
};

class Texture2D {
	size_t width = 0;
	size_t height = 0;

	GLuint id = 0;
public:
public:
	Texture2D(const Texture2D&) = delete;
	Texture2D(Texture2D&& f) noexcept {
		id = f.id;
		f.id = 0;
	}
	Texture2D& operator=(Texture2D&& other) noexcept {
		id = other.id;
		other.id = 0;
		return *this;
	}
	GLuint Id() const {
		return id;
	}
	Texture2D() {
		id = 0;
		glGenTextures(1, &id);
	}
	//Отвязывает текущую текстуру
	void LoadFromPNG(const char* path) {
		std::vector<unsigned char> image;
		unsigned width, height;
		unsigned error = lodepng::decode(image, width, height, path);
		this->width = width;
		this->height = height;
		//Проверка на то, является ли размер картинки больше 0 и её размер - степень двойки.
		if (((width & (width - 1)) != 0) || (height & (height - 1)) != 0)
			throw std::invalid_argument("Texture size must be power 2!");
		// If there's an error, display it.
		if (error != 0) {
			//std::cout << "error " << error << ": " << lodepng_error_text(error) << std::endl;
			auto t = lodepng_error_text(error);
			throw std::invalid_argument(t);
		}
		
		Bind();
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
		glTexImage2D(GL_TEXTURE_2D, 0, 4, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, &image[0]);
		checkOpenGLerror();
		Unbind();
	}
	~Texture2D()
	{
		glDeleteTextures(1, &id);
	}
	void Bind() const {
		glBindTexture(GL_TEXTURE_2D, id);
	}
	static void Unbind() {
		glBindTexture(GL_TEXTURE_2D, 0);
	}
};