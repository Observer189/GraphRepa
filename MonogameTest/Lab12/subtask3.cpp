#include "subtask3.h"




static Vertex cube[] = {
	Vertex{ 0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f}, // 0 верхн€€ права€ передн€€
	Vertex{0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f}, // 1 нижн€€ права€ передн€€
	Vertex{-0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f}, //2 нижн€€ лева€ передн€€
	Vertex{-0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f,  0.0f, 1.0f}, //3 верхн€€ лева€ передн€€
	Vertex{ 0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f}, //4 верхн€€ права€ задн€€
	Vertex{ 0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f,0.0f, 0.0f}, //5 нижн€€ права€ задн€€
	Vertex{-0.5f, -0.5f,-0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f}, //6 нижн€€ лева€ задн€€
	Vertex{-0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f}, //7 верхн€€ лева€ задн€€
	Vertex{ 0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f,1.0f, 1.0f}, //8 нижн€€ права€ задн€€

};
static GLuint indices[] = {
	0, 1, 2,
	0, 3, 2,
	0, 1, 4,
	4, 5, 1,
	1, 2, 8,
	6, 2, 8,
};

static void Draw(ProgramShader& ps, VAO& vao) {
	ps.Bind();
	vao.Bind();
	glDrawElements(GL_TRIANGLES, sizeof(indices) / sizeof(GLuint), GL_UNSIGNED_INT, 0);
	vao.Unbind();
	ps.Unbind();
}
static float portion = 0.5f;

void subtast3(sf::Window& window) {
	std::string frag = stringFromFile("./shaders/subtask3.frag");
	std::string vert = stringFromFile("./shaders/subtask3.vert");
	//std::cout << vert << '\n';
	auto vs = std::make_shared<VertexShader>(vert);
	auto fs = std::make_shared<FragmentShader>(frag);
	auto prog = ProgramShader(vs, fs);
	auto tex1 = Texture2D();
	checkOpenGLerror();


	tex1.LoadFromPNG("./textures/subtask3.png");

	auto tex2 = Texture2D();
	tex2.LoadFromPNG("./textures/subtask2.png");
	//tex2.Unbind();
	tex1.Unbind();

	auto vbo = VBO();
	auto vao = VAO();
	auto ebo = EBO();

	vao.Bind();

	vbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(cube), cube, GL_STATIC_DRAW);

	// јтрибут с координатами
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// јтрибут с цветом
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);
	// јтрибут с текстурными координатами
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
	glEnableVertexAttribArray(2);
	tex1.Unbind();
	checkOpenGLerror();
	glActiveTexture(GL_TEXTURE0);
	checkOpenGLerror();
	tex1.Bind();
	checkOpenGLerror();
	prog.Bind();
	auto a_id1 = glGetUniformLocation(prog.Id(), "ourTexture1");
	checkOpenGLerror();
	glUniform1i(a_id1, 0);
	checkOpenGLerror();
	glActiveTexture(GL_TEXTURE1);
	checkOpenGLerror();
	tex2.Bind();
	checkOpenGLerror();
	auto a_id2 = glGetUniformLocation(prog.Id(), "ourTexture2");
	checkOpenGLerror();
	glUniform1i(a_id2, 1);
	prog.Unbind();
	checkOpenGLerror();
	ebo.Bind();
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

	int uniformColorLocation = glGetUniformLocation(prog.Id(), "portion");

	prog.Bind(); // ”станавливаем шейдерную программу текущей
	glUniform1f(uniformColorLocation, portion);
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
					portion = std::clamp(portion + 0.05f, 0.0f, 1.0f);
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					glUniform1f(uniformColorLocation, portion);
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Left) {
					portion = std::clamp(portion - 0.05f, 0.0f, 1.0f);
					prog.Bind(); // ”станавливаем шейдерную программу текущей
					glUniform1f(uniformColorLocation, portion);
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