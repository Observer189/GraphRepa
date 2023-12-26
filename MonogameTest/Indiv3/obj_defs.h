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
#include  "сamera.h"

void checkOpenGLerror();
void shaderLog(GLuint shader);
std::string stringFromFile(const char* path);

struct Vertex {
	//Пространственные координаты
	GLfloat x;
	GLfloat y;
	GLfloat z;
	//Значения нормали:
	GLfloat xn;
	GLfloat yn;
	GLfloat zn;
	//Текстурные координаты
	//x-координата текстуры
	GLfloat u;
	//y-координата текстуры
	GLfloat v;
};

class VertexShader {
	GLuint id = 0;
public:
	VertexShader(const VertexShader&) = delete; // конструктор копирования отключен
	VertexShader(VertexShader&& f) noexcept { // конструктор перемещения
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
	ProgramShader(ProgramShader&& f) noexcept { // Конструктор перемещения
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
	static std::shared_ptr<ProgramShader> make_from(std::string& vs_text, std::string& fs_text) {
		auto v = std::make_shared<VertexShader>(vs_text); // объект VertexShader будет уничтожен автоматически при выходе из области видимости указателя.
		auto f = std::make_shared<FragmentShader>(fs_text);
		return std::make_shared<ProgramShader>(v, f);
	}
	ProgramShader(std::shared_ptr<VertexShader> vs, std::shared_ptr<FragmentShader> fs) {
		vert_s = vs;
		frag_s = fs;
		id = glCreateProgram(); // Создание идентификатора программы OpenGL
		auto vs_id = vs.get()->Id();
		auto fs_id = fs.get()->Id();
		glAttachShader(id, vs_id);
		glAttachShader(id, fs_id);
		glLinkProgram(id); // объединяем вершинный и фрагментный шейдеры в программу
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
	void Bind() const { // позволяет устанавливать текущую шейдерную программу для последующих рендеринговых операций
		glUseProgram(id);
	}
	GLint GetUniformId(const char* name) { // Получает идентификатор uniform-переменной с заданным именем
		auto uloc = glGetUniformLocation(id, name);
		if (uloc == -1) {
			throw std::invalid_argument("This shader program doesn't have uniform with given name");
		}
		return uloc;
	}
	GLint GetAttribId(const char* name) {
		auto uloc = glGetAttribLocation(id, name); // Получает идентификатор attribute-переменной
		if (uloc == -1) {
			throw std::invalid_argument("This shader program doesn't have active attribute with given name. Maybe it doesn't used?");
		}
		return uloc;
	}
	//Chech for -1 for error
	GLint GetUniformIdFallable(const char* name) { // Если идентификатор равен -1, вместо исключения возвращается -1.
		auto uloc = glGetUniformLocation(id, name);
		return uloc;
	}
	//Chech for -1 for error
	GLint GetAttribIdFallable(const char* name) {
		auto uloc = glGetAttribLocation(id, name);
		return uloc;
	}
	static void Unbind() {
		glUseProgram(0); // отключить шейдеры 
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
		glBindBuffer(GL_ARRAY_BUFFER, id); //  привязывает буфер вершин к текущему контексту OpenGL
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
		glGenTextures(1, &id); // генерирует уникальный идентификатор текстуры
	}
	//позволяет загружать текстуры из файлов PNG в объекты Texture2D и настраивать параметры фильтрации для уменьшения и увеличения.
	void LoadFromPNG(const char* path) {
		std::vector<unsigned char> image;
		unsigned width, height;
		unsigned error = lodepng::decode(image, width, height, path);
		this->width = width;
		this->height = height;
		//Проверка на то, является ли размер картинки больше 0 и её размер - степень двойки.
		if (((width & (width - 1)) != 0) || (height & (height - 1)) != 0)
			//throw std::invalid_argument("Texture size must be power 2!");
		// If there's an error, display it.
		if (error != 0) {
			std::cout << "error " << error << ": " << lodepng_error_text(error) << std::endl;
			auto t = lodepng_error_text(error);
			throw std::invalid_argument(t);
		}
		
		Bind(); // Привязывает текущий объект текстуры к контексту OpenGL.
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST); // Устанавливает параметры фильтрации текстуры для уменьшения (минимизации) с использованием ближайшего соседа (без интерполяции).
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST); // для увеличения 
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
	Object3D(const std::string& inputfile) {
		tinyobj::ObjReaderConfig reader_config; // Path to material files
		reader_config.triangulate = true; // для обеспечения совместимости с OpenGL, который работает с треугольниками
		tinyobj::ObjReader reader;
		if (!reader.ParseFromFile(inputfile, reader_config)) {
			if (!reader.Error().empty()) {
				std::cerr << "TinyObjReader: " << reader.Error();
			}
			exit(1);
		}
		auto& attrib = reader.GetAttrib(); // cодержит атрибуты вершин
		auto& shapes = reader.GetShapes(); // используется для получения информации о формах в объекте
		auto& materials = reader.GetMaterials();
		//Индекс пространственной координаты вершины, индекс текстурной координаты вершины => индекс вершины в итоговом списке
		//Индекс пространственной координаты вершины, индекс нормали, индекс текстурной координаты вершины => индекс вершины в итоговом списке
		auto map = std::map<std::tuple<GLuint, GLuint, GLuint>, GLuint>();

		/*
		отвечает за обход данных о формах в объекте, извлечение вершин и индексов их положения,
		нормалей и текстурных координат из атрибутов и форм объекта, а также создание уникальных 
		вершин и индексов для построения трехмерной модели.
		*/

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
					auto index_tuple = std::tuple((GLuint)idx.vertex_index, (GLuint)idx.normal_index, (GLuint)idx.texcoord_index);
					if (map.contains(index_tuple)) {
						auto t = map[index_tuple];
						indices.push_back(t);
					}
					else {
						tinyobj::real_t vx = attrib.vertices[3 * size_t(idx.vertex_index) + 0];
						tinyobj::real_t vy = attrib.vertices[3 * size_t(idx.vertex_index) + 1];
						tinyobj::real_t vz = attrib.vertices[3 * size_t(idx.vertex_index) + 2];
						if (idx.normal_index < 0) {
							throw std::invalid_argument("File doesn't contain texture indices");
						}
						tinyobj::real_t nx = attrib.normals[3 * size_t(idx.normal_index) + 0];
						tinyobj::real_t ny = attrib.normals[3 * size_t(idx.normal_index) + 1];
						tinyobj::real_t nz = attrib.normals[3 * size_t(idx.normal_index) + 2];
						if (idx.texcoord_index < 0) {
							throw std::invalid_argument("File doesn't contain texture indices");
						}
						tinyobj::real_t tx = attrib.texcoords[2 * size_t(idx.texcoord_index) + 0];
						tinyobj::real_t ty = attrib.texcoords[2 * size_t(idx.texcoord_index) + 1];
						vertices.push_back(Vertex{ vx, vy, vz, nx, ny, nz, tx, ty });
						auto index = (GLuint)(vertices.size() - 1);
						map[index_tuple] = index;
						indices.push_back(index);
					}
				}
				index_offset += fv;
			}
		}
		std::cout << "Loaded: " << indices.size() << " index vertices\n";
		std::cout << "Loaded: " << vertices.size() << " unique vertices\n";		
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
	glm::mat3 getNormalTransformMatrix()
	{
		auto m = getTransformMatrix();

		//auto t = glm::mat3(m);
		glm::mat3 res = glm::transpose(glm::inverse(m));

		return res;
	}
	void Rotate(glm::vec3 delta)
	{
		rotation += delta;
	}
};


class PointLightSource {
public:
	glm::vec4 lightsourcePos;
	GLfloat intencity;

	PointLightSource(const glm::vec4& lightsourcePos, const GLfloat& intencity)
		: lightsourcePos(lightsourcePos), intencity(intencity)
	{
	}
};
class ProjectorLightSource {
public:
	glm::vec4 projectorPos;
	GLfloat intencity;
	glm::vec3 dir;
	GLfloat angle;


	ProjectorLightSource(const glm::vec4& projectorPos, const GLfloat& intencity, const glm::vec3& dir, const GLfloat& angle)
		: projectorPos(projectorPos), intencity(intencity), dir(dir), angle(angle)
	{
	}
};
class GlobalLightSource {
public:
	GLfloat intencity;
	glm::vec3 dir;

	GlobalLightSource(const GLfloat& intencity, const glm::vec3& dir)
		: intencity(intencity), dir(dir)
	{
	}
};

class Scene;


class GameObject
{
public:
	std::shared_ptr<Texture2D> texture;
	std::shared_ptr<VAO> vao;
	std::shared_ptr<EBO> ebo;
	std::shared_ptr<VBO> vbo_mesh;
	std::shared_ptr<VBO> vbo_local_transform_matrix;
	std::shared_ptr<VBO> vbo_local_normal_transform_matrix;

	std::shared_ptr<ProgramShader> program;
	ObjectTransform transform;
	std::shared_ptr<Object3D> mesh;

	GameObject(std::shared_ptr<Object3D> mesh, std::shared_ptr<Texture2D> tex, std::shared_ptr<ProgramShader> ps) 
	{
		auto vbo_mesh = std::make_shared<VBO>();
		auto vbo_local_transform_matrix = std::make_shared<VBO>();
		auto vbo_local_normal_transform_matrix = std::make_shared<VBO>();

		auto ebo = std::make_shared<EBO>();
		auto vao = std::make_shared<VAO>();
		transform.position = glm::vec3(0.0f);
		transform.rotation = glm::vec3(0.0f);
		transform.scale = glm::vec3(1.0f);
		this->texture = tex;
		this->vao = vao;
		this->ebo = ebo;
		this->vbo_mesh = vbo_mesh;
		this->vbo_local_transform_matrix = vbo_local_transform_matrix;
		this->vbo_local_normal_transform_matrix = vbo_local_normal_transform_matrix;
		this->program = ps;
		this->mesh = mesh;
	}
	static GameObject make_from_strings(const std::string& mesh_path, const std::string& texture_path, const std::string& vertex_shader_path, const std::string& fragment_shader_path) {
		auto mesh = std::make_shared<Object3D>(mesh_path);
		auto tex = std::make_shared<Texture2D>();
		tex.get()->LoadFromPNG(texture_path.c_str());
		std::string vert = stringFromFile(vertex_shader_path.c_str());
		std::string frag = stringFromFile(fragment_shader_path.c_str());
		auto prog = ProgramShader::make_from(vert, frag);
		auto obj = GameObject(mesh, tex, prog);
		return obj;
	};
	void init() {
		auto m = mesh.get();
		vao.get()->Bind();
		ebo.get()->Bind();
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(GLuint) * m->indices.size(), &m->indices[0], GL_STATIC_DRAW);

		vbo_mesh.get()->Bind();
		auto ps = this->program.get();
		glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * m->vertices.size(), &m->vertices[0], GL_STATIC_DRAW);
		ps->Bind();
		// Атрибут с координатами
		auto coord = ps->GetAttribId("coord");
		glVertexAttribPointer(coord, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
		glEnableVertexAttribArray(coord);
		// Атрибут с нормалями
		auto facenormal = ps->GetAttribId("facenormal");
		glVertexAttribPointer(facenormal, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
		glEnableVertexAttribArray(facenormal);
		// Атрибут с текстурными координатами
		auto tex = ps->GetAttribId("tex");
		glVertexAttribPointer(tex, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
		glEnableVertexAttribArray(tex);
		vbo_local_transform_matrix.get()->Bind();
		auto matrix = transform.getTransformMatrix();
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat4), glm::value_ptr(matrix), GL_STATIC_DRAW);
		auto pos = glGetAttribLocation(ps->Id(), "objectTransformation");
		auto pos1 = pos + 0;
		auto pos2 = pos + 1;
		auto pos3 = pos + 2;
		auto pos4 = pos + 3;
		glEnableVertexAttribArray(pos1);
		glEnableVertexAttribArray(pos2);
		glEnableVertexAttribArray(pos3);
		glEnableVertexAttribArray(pos4);
		glVertexAttribPointer(pos1, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(0));
		glVertexAttribPointer(pos2, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 4));
		glVertexAttribPointer(pos3, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 8));
		glVertexAttribPointer(pos4, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 12));
		glVertexAttribDivisor(pos1, 1);
		glVertexAttribDivisor(pos2, 1);
		glVertexAttribDivisor(pos3, 1);
		glVertexAttribDivisor(pos4, 1);

		vbo_local_normal_transform_matrix.get()->Bind();
		auto normal_matrix = transform.getNormalTransformMatrix();

		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat3), glm::value_ptr(normal_matrix), GL_STATIC_DRAW);
		auto pos_mat3 = glGetAttribLocation(ps->Id(), "normalTransformation");
		if (pos_mat3 != -1) {
			auto pos1_mat3 = pos_mat3 + 0;
			auto pos2_mat3 = pos_mat3 + 1;
			auto pos3_mat3 = pos_mat3 + 2;
			glEnableVertexAttribArray(pos1_mat3);
			glEnableVertexAttribArray(pos2_mat3);
			glEnableVertexAttribArray(pos3_mat3);
			glVertexAttribPointer(pos1_mat3, 3, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 3 * 3, (void*)(0));
			glVertexAttribPointer(pos2_mat3, 3, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 3 * 3, (void*)(sizeof(float) * 3));
			glVertexAttribPointer(pos3_mat3, 3, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 3 * 3, (void*)(sizeof(float) * 6));
			glVertexAttribDivisor(pos1_mat3, 1);
			glVertexAttribDivisor(pos2_mat3, 1);
			glVertexAttribDivisor(pos3_mat3, 1);
		}
		else {
			std::cout << "Warning: This shader program doesn't have normalTransformation";
		}
		ps->Unbind();
		vao.get()->Unbind();
	}

	void update(Camera &camera) {
		checkOpenGLerror();

		// Привязка программы шейдеров
		auto ps = this->program.get();
		ps->Bind(); 

		// Установка матрицы преобразования камеры:
		auto cam_id = ps->GetUniformId("cameraTransformation");
		glUniformMatrix4fv(cam_id, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
		
		
		auto cam_dir_id = ps->GetUniformId("cameraPos");
		glUniform4fv(cam_dir_id, 1, glm::value_ptr(camera.GetPosition()));
		checkOpenGLerror();

		// Устанавливается uniform-переменная projection для передачи матрицы проекции в шейдер
		auto proj_id = ps->GetUniformId("projection");
		glUniformMatrix4fv(proj_id, 1, GL_FALSE, glm::value_ptr(camera.projection));

		vbo_local_transform_matrix.get()->Bind();
		auto matrix = transform.getTransformMatrix();
		// Обновление буфера вершинных атрибутов для матрицы преобразования
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat4), glm::value_ptr(matrix), GL_STATIC_DRAW);
		auto normal_transform = transform.getNormalTransformMatrix();
		vbo_local_normal_transform_matrix.get()->Bind();
		glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat3), glm::value_ptr(normal_transform), GL_STATIC_DRAW);
		vbo_local_transform_matrix.get()->Unbind();
		ps->Unbind();
		checkOpenGLerror();
	}
	void lightUpdate(Scene& scene);
private:

};

class Scene
{
public:

	std::vector<GameObject> objects;
	Camera camera;
	
	GlobalLightSource gls;
	PointLightSource pls;
	ProjectorLightSource pjls;


	Scene(const std::vector<GameObject>& objects, const Camera& camera, const GlobalLightSource& gls, const PointLightSource& pls, const ProjectorLightSource& pjls)
		: objects(objects), camera(camera), gls(gls), pls(pls), pjls(pjls)
	{
	}
};
