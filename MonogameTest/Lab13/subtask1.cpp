#include "subtask1.h"



static glm::mat4 Matrices[] = {
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.05, 0.05, 0.05)), glm::vec3(5, 5, 0)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.05, 0.05, 0.05)), glm::vec3(-5, 5, 0)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.05, 0.05, 0.05)), glm::vec3(5, -5, 0)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.05, 0.05, 0.05)), glm::vec3(-5, -5, 0)),
};

static void Draw(ProgramShader& ps, VAO& vao, GLuint indices_count) {
	ps.Bind();
	vao.Bind();
	glDrawElementsInstanced(GL_TRIANGLES, indices_count, GL_UNSIGNED_INT, 0, 4);
	vao.Unbind();
	ps.Unbind();
}
static float portion = 0.5f;

static glm::mat4 transform = glm::mat4(1.0f);
static glm::mat4 rotation= glm::mat4(1.0f);

void subtask1(sf::Window& window) {
	//auto s = std::string("./objects/cube.obj");
	auto s = std::string("./objects/12140_Skull_v3_L2.obj");

	auto o = Object3D(s);
	glEnable(GL_DEPTH_TEST);
	std::string frag = stringFromFile("./shaders/subtask1.frag");
	std::string vert = stringFromFile("./shaders/subtask1.vert");
	//std::cout << vert << '\n';
	auto vs = std::make_shared<VertexShader>(vert);
	auto fs = std::make_shared<FragmentShader>(frag);
	auto prog = ProgramShader(vs, fs);
	auto tex = Texture2D();
	//tex.LoadFromPNG("./objects/cube_texture.png");
	tex.LoadFromPNG("./objects/Skull.png");

	auto matrixVbo = VBO();
	auto vbo = VBO();
	auto vao = VAO();
	auto ebo = EBO();

	vao.Bind();
	tex.Bind();
	matrixVbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(Matrices), Matrices, GL_STATIC_DRAW);
	int pos = glGetAttribLocation(prog.Id(), "additionalTransformation");
	int pos1 = pos + 0;
	int pos2 = pos + 1;
	int pos3 = pos + 2;
	int pos4 = pos + 3;
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
	//matrixVbo.Unbind();
	checkOpenGLerror();
	vbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * o.verteces.size(), &o.verteces[0], GL_STATIC_DRAW);

	// Атрибут с координатами
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// Атрибут с текстурными координатами
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);

	ebo.Bind();
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(GLuint) * o.indices.size(), &o.indices[0], GL_STATIC_DRAW);

	int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");

	prog.Bind(); // Устанавливаем шейдерную программу текущей
	glUniformMatrix4fv(uniformTransformLocation,1,GL_FALSE, glm::value_ptr(transform*rotation));
	prog.Unbind();

	vao.Unbind();

	checkOpenGLerror();

	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }

			if (event.type == sf::Event::EventType::KeyPressed) {
				if (event.key.code == sf::Keyboard::Right) {
					transform = glm::translate(transform,glm::vec3(0.05f,0.0f,0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Left) {
					transform = glm::translate(transform, glm::vec3(-0.05f, 0.0f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Up) {
					transform = glm::translate(transform, glm::vec3(0.0f, 0.05f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Down) {
					transform = glm::translate(transform, glm::vec3(0.0f, -0.05f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Q) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(1.0f, 0.0f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::W) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(0.0f, 1.0f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::E) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(0.0f, 0.0f, 1.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}

			}
		}
		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			Draw(prog, vao, o.indices.size());
			window.display();
		}
	}
}