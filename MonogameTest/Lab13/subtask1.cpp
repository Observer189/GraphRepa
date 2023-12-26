#include "subtask1.h"



 glm::mat4 Matrices[] = {
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)), glm::vec3(0, 0, 0)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)), glm::vec3(-5, 0, 0)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)), glm::vec3(7, -5, 30)),
	glm::translate(glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)), glm::vec3(-4, 5, -5)),
	//glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)),
	//glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)),
	//glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01)),
	//glm::scale(glm::mat4(1.0f), glm::vec3(0.01, 0.01, 0.01))
};

 static Camera camera = Camera();
 static float totalTime;

static void Draw(Texture2D& tex, std::vector<GameObject> skulls,ProgramShader& ps,std::shared_ptr<Object3D>& skullMeshPointer, GLuint indices_count, sf::Window& window, float deltaTime) 
{
	totalTime += deltaTime;
	skulls[0].transform.rotation = glm::vec3(-90, 0, totalTime / 700);
	skulls[0].transform.position = glm::vec3(0, 0, 0);
	skulls[1].transform.position = glm::vec3(glm::cos(glm::radians(totalTime / 100)) * 0, glm::cos(glm::radians(totalTime / 100)) * 4, glm::sin(glm::radians(totalTime / 100)) * 3);
	skulls[1].transform.rotation = glm::vec3(-90, 0, totalTime / 100 );
	skulls[2].transform.position = glm::vec3(glm::cos(glm::radians(totalTime/500)) * 3, glm::cos(glm::radians(totalTime / 500))*0.3,glm::sin(glm::radians(totalTime / 500) )*3);
	skulls[2].transform.rotation = glm::vec3(-90,0, totalTime / 250 + 90);
	skulls[3].transform.position = glm::vec3(glm::cos(glm::radians(totalTime / 300)) * 2, glm::cos(glm::radians(totalTime / 300)) * 0.1, glm::sin(glm::radians(totalTime / 300)) * 2);
	skulls[3].transform.rotation = glm::vec3(-90, 0, totalTime / 150 + 180);
	skulls[4].transform.position = glm::vec3(glm::cos(glm::radians(totalTime / 600)) * 8, glm::sin(glm::radians(totalTime / 600)) * 0, glm::sin(glm::radians(totalTime / 600)) * 2);
	skulls[4].transform.rotation = glm::vec3(-90, 0, totalTime / 300 + 270);
	glm::mat4 skullLocalTranses[5];

	for (size_t i = 0; i < skulls.size(); i++)
	{
		skullLocalTranses[i] = skulls[i].transform.getTransformMatrix();
	}

	auto matrixVbo = VBO();
	auto vbo = VBO();
	auto vao = VAO();
	auto ebo = EBO();

	vao.Bind();
	tex.Bind();
	matrixVbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(skullLocalTranses), skullLocalTranses, GL_STATIC_DRAW);
	int pos = glGetAttribLocation(ps.Id(), "additionalTransformation");
	int pos1 = pos + 0;
	int pos2 = pos + 1;
	int pos3 = pos + 2;
	int pos4 = pos + 3;
	int pos5 = pos + 4;
	glEnableVertexAttribArray(pos1);
	glEnableVertexAttribArray(pos2);
	glEnableVertexAttribArray(pos3);
	glEnableVertexAttribArray(pos4);
	glEnableVertexAttribArray(pos5);
	glVertexAttribPointer(pos1, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(0));
	glVertexAttribPointer(pos2, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 4));
	glVertexAttribPointer(pos3, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 8));
	glVertexAttribPointer(pos4, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 12));
	glVertexAttribPointer(pos5, 4, GL_FLOAT, GL_FALSE, sizeof(GLfloat) * 4 * 4, (void*)(sizeof(float) * 16));
	glVertexAttribDivisor(pos1, 1);
	glVertexAttribDivisor(pos2, 1);
	glVertexAttribDivisor(pos3, 1);
	glVertexAttribDivisor(pos4, 1);
	glVertexAttribDivisor(pos5, 1);
	//matrixVbo.Unbind();
	checkOpenGLerror();
	vbo.Bind();
	glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * skullMeshPointer->vertices.size(), &skullMeshPointer->vertices[0], GL_STATIC_DRAW);

	// Атрибут с координатами
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// Атрибут с текстурными координатами
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);

	ebo.Bind();
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(GLuint) * skullMeshPointer->indices.size(), &skullMeshPointer->indices[0], GL_STATIC_DRAW);

	int uniformTransformLocation = glGetUniformLocation(ps.Id(), "cameraTransformation");

	ps.Bind(); // Устанавливаем шейдерную программу текущей
	glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
	uniformTransformLocation = glGetUniformLocation(ps.Id(), "projection");
	glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(glm::perspective(glm::radians(90.0f), 800.0f / 600.0f, 0.1f, 500.0f)));
	ps.Unbind();

	vao.Unbind();




	ps.Bind();
	vao.Bind();
	glDrawElementsInstanced(GL_TRIANGLES, indices_count, GL_UNSIGNED_INT, 0, skulls.size());
	vao.Unbind();
	ps.Unbind();
}
static float portion = 0.5f;

static glm::mat4 transform = glm::mat4(1.0f);
static glm::mat4 rotation= glm::mat4(1.0f);

static std::vector<GameObject> skulls = std::vector<GameObject>();
float mouseX;
float mouseY;

void subtask1(sf::Window& window) {
	window.setMouseCursorVisible(false);
	//window.setMouseCursorGrabbed(true);
	//auto s = std::string("./objects/cube.obj");
	auto s = std::string("./objects/12140_Skull_v3_L2.obj");


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

	auto skullMeshPointer = std::make_shared<Object3D>(Object3D(s));

	skulls.push_back(GameObject(skullMeshPointer));
	skulls.push_back(GameObject(skullMeshPointer));
	skulls.push_back(GameObject(skullMeshPointer));
	skulls.push_back(GameObject(skullMeshPointer));
	skulls.push_back(GameObject(skullMeshPointer));

	//for (size_t i = 0; i < skulls.size(); i++)
	//{
	//	skulls[i].transform.scale = glm::vec3(0.01f * glm::sqrt(i+2));
	//	skulls[i].transform.Rotate(glm::vec3(-90.0f,0.f,0.f));
	//}

	const float rotationSpeed = 20.0f; 
	for (size_t i = 0; i < skulls.size(); i++)
	{
		skulls[i].transform.scale = glm::vec3(0.01f * glm::sqrt(i + 2));
		skulls[i].transform.Rotate(glm::vec3(-90.0f * rotationSpeed, 0.0f, 0.0f));
	}

	checkOpenGLerror();
	
	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }

			if (event.type == sf::Event::EventType::KeyPressed) {
				if (event.key.code == sf::Keyboard::D) {
					//transform = glm::translate(transform,glm::vec3(0.05f,0.0f,0.0f));
					camera.ProcessKeyboard(RIGHT,0.05f);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::A) {
					camera.ProcessKeyboard(LEFT, 0.05f);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::W) {
					camera.ProcessKeyboard(FORWARD, 0.05f);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
					prog.Unbind();

				}
				else if (event.key.code == sf::Keyboard::S) {
					camera.ProcessKeyboard(BACKWARD, 0.05f);
					prog.Bind(); // Устанавливаем шейдерную программу текущей
					int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
					glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
					prog.Unbind();

				}
				

			}
			else if (event.type == sf::Event::MouseMoved)
			{
				
				camera.ProcessMouseMovement(-(mouseX - event.mouseMove.x), mouseY - event.mouseMove.y);
				prog.Bind(); // Устанавливаем шейдерную программу текущей
				int uniformTransformLocation = glGetUniformLocation(prog.Id(), "cameraTransformation");
				glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(camera.GetViewMatrix()));
				//uniformTransformLocation = glGetUniformLocation(prog.Id(), "projection");
				//glUniformMatrix4fv(uniformTransformLocation, 1, GL_FALSE, glm::value_ptr(glm::perspective(glm::radians(90.0f),800.0f/600.0f,0.1f,500.0f)));
				prog.Unbind();

				mouseX = event.mouseMove.x;
				mouseY = event.mouseMove.y;
				//std::cout << "new mouse x: " << event.mouseMove.x<< std::endl;
				//std::cout << "new mouse y: " << event.mouseMove.y << std::endl;
			}
		}
		sf::Clock clock;
		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			Draw(tex,skulls,prog,skullMeshPointer, skullMeshPointer->indices.size(),window,clock.restart().asMicroseconds());
			window.display();
		}
	}
}