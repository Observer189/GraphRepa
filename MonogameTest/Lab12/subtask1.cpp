#include "subtask1.h"




static Vertex tetrahedron[] = {
	Vertex{ 0.0f, -0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f}, // 0 верхн€€ права€ передн€€
	Vertex{0.6f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f}, // 1 нижн€€ права€ передн€€
	Vertex{-0.6f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f}, //2 нижн€€ лева€ передн€€
	Vertex{ 0.0f, 0.7f, 0.0f, 0.0f, 1.0f, 0.0f,1.0f, 1.0f}, //8 нижн€€ права€ задн€€

};
static GLuint indices[] = {
	0, 1, 2, //нижн€€ грань
	1, 2, 3, //лицева€ грань
	0, 1, 3, //лева€ грань
	0, 2, 3, //права€ грань
};

static void Draw(ProgramShader& ps, VAO& vao) {
	ps.Bind();
	vao.Bind();
	glDrawElements(GL_TRIANGLES, sizeof(indices) / sizeof(GLuint), GL_UNSIGNED_INT, 0);
	vao.Unbind();
	ps.Unbind();
}
static float portion = 0.5f;

static glm::mat4 transform = glm::mat4(1.0f);
static glm::mat4 rotation= glm::mat4(1.0f);

void subtask1(sf::Window& window) {
	glEnable(GL_DEPTH_TEST);
	std::string frag = stringFromFile("./shaders/subtask1.frag");
	std::string vert = stringFromFile("./shaders/subtask1.vert");
	//std::cout << vert << '\n';
	auto vs = std::make_shared<VertexShader>(vert);
	auto fs = std::make_shared<FragmentShader>(frag);
	auto prog = ProgramShader(vs, fs);
	auto tex = Texture2D();
	tex.LoadFromPNG("./textures/subtask2.png");

	auto vbo = VBO();
	auto vao = VAO();
	auto ebo = EBO();

	vao.Bind();

	tex.Bind();
	vbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(tetrahedron), tetrahedron, GL_STATIC_DRAW);

	// јтрибут с координатами
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// јтрибут с цветом
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);
	// јтрибут с текстурными координатами
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
	glEnableVertexAttribArray(2);

	ebo.Bind();
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

	int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");

	prog.Bind(); // ”станавливаем шейдерную программу текущей
	glUniformMatrix4fv(uniformTransformLocation,1,GL_FALSE, glm::value_ptr(transform*rotation));
	prog.Unbind();

	vao.Unbind();


	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }

			if (event.type == sf::Event::EventType::KeyPressed) {
				if (event.key.code == sf::Keyboard::Right) {
					transform = glm::translate(transform,glm::vec3(0.05f,0.0f,0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Left) {
					transform = glm::translate(transform, glm::vec3(-0.05f, 0.0f, 0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Up) {
					transform = glm::translate(transform, glm::vec3(0.0f, 0.05f, 0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Down) {
					transform = glm::translate(transform, glm::vec3(0.0f, -0.05f, 0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Q) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(1.0f, 0.0f, 0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::W) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(0.0f, 1.0f, 0.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::E) {
					rotation = glm::rotate(rotation, 0.1f, glm::vec3(0.0f, 0.0f, 1.0f));
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * rotation));
					prog.Unbind();

				}

			}
		}
		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			Draw(prog, vao);
			window.display();
		}
	}
}