#pragma once
#include <iostream>
#include <fstream>
#include <string>

#include <GL/glew.h>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <lodepng.h>
#include <random>
#include <stdexcept>
#include "tiny_obj_loader.h"
#include <unordered_set>
#include <unordered_map>
#include <map>

#include <tuple>

void checkOpenGLerror();
void shaderLog(GLuint shader);
std::string stringFromFile(const char* path);

struct Vertex {
	//���������������� ����������
	GLfloat x;
	GLfloat y;
	GLfloat z;
	//���������� ����������
	//x-���������� ��������
	GLfloat u;
	//y-���������� ��������
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
		// ��������� ������ ������
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
//Element Buffer Object ��� Index Buffer Object
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
	//���������� ������� ��������
	void LoadFromPNG(const char* path) {
		std::vector<unsigned char> image;
		unsigned width, height;
		unsigned error = lodepng::decode(image, width, height, path);
		this->width = width;
		this->height = height;
		//�������� �� ��, �������� �� ������ �������� ������ 0 � � ������ - ������� ������.
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

class Object3D {
public:
	std::vector<Vertex> vertices = std::vector<Vertex>();
	std::vector<GLuint> indices = std::vector<GLuint>();
public:
	Object3D(std::string& inputfile) {
		tinyobj::ObjReaderConfig reader_config; // Path to material files
		reader_config.triangulate = true;
		tinyobj::ObjReader reader;
		if (!reader.ParseFromFile(inputfile, reader_config)) {
			if (!reader.Error().empty()) {
				std::cerr << "TinyObjReader: " << reader.Error();
			}
			exit(1);
		}
		auto& attrib = reader.GetAttrib();
		auto& shapes = reader.GetShapes();
		auto& materials = reader.GetMaterials();
		//������ ���������������� ���������� �������, ������ ���������� ���������� ������� => ������ ������� � �������� ������
		auto map = std::map<std::tuple<GLuint, GLuint>, GLuint>();

		// Loop over shapes
		for (size_t s = 0; s < shapes.size(); s++) {
			// Loop over faces(polygon)
			size_t index_offset = 0;
			for (size_t f = 0; f < shapes[s].mesh.num_face_vertices.size(); f++) {
				size_t fv = size_t(shapes[s].mesh.num_face_vertices[f]);
				//std::cout << "Face: " << f << "\n";
				// Loop over vertices in the face.
				for (size_t v = 0; v < fv; v++) {
					// access to vertex
					tinyobj::index_t idx = shapes[s].mesh.indices[index_offset + v];
					auto index_tuple = std::tuple((GLuint)idx.vertex_index, (GLuint)idx.texcoord_index);
					if (map.contains(index_tuple)) {
						auto t = map[index_tuple];
						indices.push_back(t);
					}
					else {
						tinyobj::real_t vx = attrib.vertices[3 * size_t(idx.vertex_index) + 0];
						tinyobj::real_t vy = attrib.vertices[3 * size_t(idx.vertex_index) + 1];
						tinyobj::real_t vz = attrib.vertices[3 * size_t(idx.vertex_index) + 2];
						if (idx.texcoord_index < 0) {
							throw std::invalid_argument("File doesn't contain texture indices");
						}
						tinyobj::real_t tx = attrib.texcoords[2 * size_t(idx.texcoord_index) + 0];
						tinyobj::real_t ty = attrib.texcoords[2 * size_t(idx.texcoord_index) + 1];
						vertices.push_back(Vertex{vx, vy, vz, tx, ty});
						auto index = (GLuint)(vertices.size() - 1);
						map[index_tuple] = index;
						indices.push_back(index);
					}
				}
				index_offset += fv;

				// per-face material
				//shapes[s].mesh.material_ids[f];
			}
		}
		std::cout << "Loaded: " << indices.size() << " index vertices\n";
		std::cout << "Loaded: " << vertices.size() << " unique vertices\n";
		//for (auto& i : indices) {
		//	auto v = vertices[i];
		//	std::cout << "Index: " << i << ": " << v.x << " " << v.y << " " << v.z << " " << v.u << " " << v.v << " " <<  "\n";
		//}
		
	}
};

struct ObjectTransform
{
	glm::vec3 position;
	glm::vec3 rotation;
	glm::vec3 scale;

	glm::mat4 getTransformMatrix()
	{
		glm::mat4 transform = glm::mat4(1.0f);
		transform = glm::translate(transform,position);
		transform = glm::rotate(transform,glm::radians(rotation.x), glm::vec3(1.0f,0.0f,0.0f));
		transform = glm::rotate(transform, glm::radians(rotation.y), glm::vec3(0.0f, 1.0f, 0.0f));
		transform = glm::rotate(transform, glm::radians(rotation.z), glm::vec3(0.0f, 0.0f, 1.0f));
		transform = glm::scale(transform,scale);

		return transform;
	}

	void Rotate(glm::vec3 delta)
	{
		rotation += delta;
	}
};

class GameObject
{
public:
	ObjectTransform transform;
	std::shared_ptr<Object3D> mesh;

	GameObject(std::shared_ptr<Object3D> mesh) 
	{
		transform.position = glm::vec3(0.0f);
		transform.rotation = glm::vec3(0.0f);
		transform.scale = glm::vec3(1.0f);
		this->mesh = mesh;
	}

	

private:

};
