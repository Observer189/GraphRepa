#include "subtask4.h"



static const size_t segments_count = 50;
static Vertex circle[segments_count+1];
static GLuint indices[segments_count*3];


static void Draw(ProgramShader& ps, VAO& vao) {
	ps.Bind();
	vao.Bind();
	glDrawElements(GL_TRIANGLES, sizeof(indices) / sizeof(GLuint), GL_UNSIGNED_INT, 0);
	vao.Unbind();
	ps.Unbind();
}
static float portion = 0.5f;

static glm::mat4 transform = glm::mat4(1.0f);
static glm::mat4 scaleMat = glm::mat4(1.0f);
static glm::vec3 scale = glm::vec3(1.0f, 1.0f, 1.0f);

void subtask4(sf::Window& window) {
	glEnable(GL_DEPTH_TEST);
	std::string frag = stringFromFile("./shaders/subtask1.frag");
	std::string vert = stringFromFile("./shaders/subtask1.vert");
	//std::cout << vert << '\n';
	auto vs = std::make_shared<VertexShader>(vert);
	auto fs = std::make_shared<FragmentShader>(frag);
	auto prog = ProgramShader(vs, fs);
	auto tex = Texture2D();
	tex.LoadFromPNG("./textures/subtask2.png");

	circle[0] = Vertex(0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f);

	auto rotMat = glm::rotate(glm::mat4(1.0f),glm::radians(360.0f/segments_count), 
		glm::vec3(0.0f,0.0f,1.0f));
	glm::vec4 offsetVec = glm::vec4(0.5f,0.0f,0.0f,1.0f);

	float segmentsPerStage = (float)segments_count / 3.0f;
	float diff = 1.0f / segmentsPerStage;
	glm::vec3 curColor = glm::vec3(1.0f, 0.0f, 0.0f);
	size_t stageNum = 0;
	size_t segmentsCurStage = 0;
	for (size_t i = 1; i < segments_count+1; i++)
	{
		circle[i] = Vertex(offsetVec.x,offsetVec.y,offsetVec.z,curColor.r,curColor.g,curColor.b,0.0f,0.0f);
		offsetVec = rotMat * offsetVec;
		if (i >= 2)
		{
			indices[(i-1) * 3 - 3] = 0;
			indices[(i-1) * 3 - 2] = i - 1;
			indices[(i-1) * 3 - 1] = i;
		}

		if (stageNum == 0)
		{
			curColor.r -= diff;
			curColor.g += diff;
		}
		else if (stageNum == 1)
		{
			curColor.g -= diff;
			curColor.b += diff;
		}
		else if (stageNum == 2)
		{
			curColor.b -= diff;
			curColor.r += diff;
		}
		segmentsCurStage++;
		if (segmentsCurStage > segmentsPerStage)
		{
			segmentsCurStage = 0;
			stageNum++;
		}
	}

	indices[segments_count * 3 - 3] = 0;
	indices[segments_count * 3 - 2] = segments_count;
	indices[segments_count * 3 - 1] = 1;

	auto vbo = VBO();
	auto vao = VAO();
	auto ebo = EBO();

	vao.Bind();

	tex.Bind();
	vbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(circle), circle, GL_STATIC_DRAW);

	// Атрибут с координатами
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// Атрибут с цветом
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);
	// Атрибут с текстурными координатами
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(GLfloat), (GLvoid*)(6 * sizeof(GLfloat)));
	glEnableVertexAttribArray(2);

	ebo.Bind();
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

	int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
	scaleMat = glm::scale(glm::mat4(1.0f), scale);
	prog.Bind(); // Устанавливаем шейдерную программу текущей
	glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * scaleMat));
	prog.Unbind();

	vao.Unbind();


	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }

			if (event.type == sf::Event::EventType::KeyPressed) {
				if (event.key.code == sf::Keyboard::Right) {
					transform = glm::translate(transform, glm::vec3(0.05f, 0.0f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Left) {
					transform = glm::translate(transform, glm::vec3(-0.05f, 0.0f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Up) {
					transform = glm::translate(transform, glm::vec3(0.0f, 0.05f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Down) {
					transform = glm::translate(transform, glm::vec3(0.0f, -0.05f, 0.0f));
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform* scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::Q) {
					scale.x += 0.1f;
					scaleMat = glm::scale(glm::mat4(1.0f), scale);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform* scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::W) {
					scale.x -= 0.1f;
					scaleMat = glm::scale(glm::mat4(1.0f), scale);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform* scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::E) {
					scale.y += 0.1f;
					scaleMat = glm::scale(glm::mat4(1.0f), scale);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform* scaleMat));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::R) {
					scale.y -= 0.1f;
					scaleMat = glm::scale(glm::mat4(1.0f), scale);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "transformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(transform * scaleMat));
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