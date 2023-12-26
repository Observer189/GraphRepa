#include <string>
#include "subtask1.h"
#include <random>
#include <glm/gtx/rotate_vector.hpp>

float mouseX;
float mouseY;

void Draw(Scene& scene) {
	for (auto& obj : scene.objects) {
		obj.update(scene.camera);
		obj.lightUpdate(scene);
		obj.vao.get()->Bind();
		obj.texture.get()->Bind(); //  Привязка текстуры объекта к текущему контексту OpenGL.
		obj.program.get()->Bind();
		glDrawElements(GL_TRIANGLES, obj.mesh.get()->indices.size(), GL_UNSIGNED_INT, 0); // Выполнение рендеринга объекта с использованием индексов
		obj.texture.get()->Unbind();
		obj.program.get()->Unbind();
		obj.vao.get()->Unbind();
	}
	checkOpenGLerror();
}

void subtask1(sf::Window& window) {
	using namespace std::string_literals;
	auto camera = Camera();
	window.setMouseCursorVisible(false);

	glEnable(GL_DEPTH_TEST);

	auto snow = GameObject::make_from_strings("./objects/grass.obj"s, "./textures/snow.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto tree = GameObject::make_from_strings("./objects/tree.obj"s, "./textures/tree.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto airship = GameObject::make_from_strings("./objects/airship.obj"s, "./textures/airship.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto sled = GameObject::make_from_strings("./objects/sled.obj"s, "./textures/sled.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);

	auto cloud1 = GameObject::make_from_strings("./objects/cloud1.obj"s, "./textures/cloud1.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto cloud2 = GameObject::make_from_strings("./objects/cloud2.obj"s, "./textures/cloud2.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);

	auto baloon1 = GameObject::make_from_strings("./objects/baloon.obj"s, "./textures/baloon.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto baloon2= GameObject::make_from_strings("./objects/baloon.obj"s, "./textures/baloon1.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto baloon3 = GameObject::make_from_strings("./objects/baloon.obj"s, "./textures/baloon2.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto baloon4 = GameObject::make_from_strings("./objects/baloon.obj"s, "./textures/baloon3.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto baloon5 = GameObject::make_from_strings("./objects/baloon.obj"s, "./textures/baloon4.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);

	snow.transform.scale = glm::vec3(0.8f);
	snow.transform.position = glm::vec3(0, -11.5, 0);
	snow.transform.Rotate(glm::vec3(-90.0f, 0.f, 90.0f));

	tree.transform.scale = glm::vec3(0.01f);
	tree.transform.position = glm::vec3(-5, -6.5f, 0);

	airship.transform.scale = glm::vec3(0.8f);
	airship.transform.position = glm::vec3(-5, 20.5, 0);

	sled.transform.scale = glm::vec3(4.0f);
	sled.transform.position = glm::vec3(-15, -5.0f, 0);

	cloud1.transform.scale = glm::vec3(3.5f);
	cloud1.transform.position = glm::vec3(0, 2500.0f, 0);
	cloud1.transform.Rotate(glm::vec3(0.0f, 0.0f, 90.0f));

	cloud2.transform.scale = glm::vec3(3.5f);
	cloud2.transform.position = glm::vec3(-5, 25.0f, 0);
	cloud2.transform.Rotate(glm::vec3(0.0f, 0.0f, 90.0f));

	std::srand(static_cast<unsigned int>(std::time(0)));

	baloon1.transform.position = glm::vec3(static_cast<float>(std::rand() % 41 - 20), 12.0f, static_cast<float>(std::rand() % 21 - 10));
	baloon2.transform.position = glm::vec3(static_cast<float>(std::rand() % 41 - 20), 12.0f, static_cast<float>(std::rand() % 21 - 10));
	baloon3.transform.position = glm::vec3(static_cast<float>(std::rand() % 41 - 20), 12.0f, static_cast<float>(std::rand() % 21 - 10));
	baloon4.transform.position = glm::vec3(static_cast<float>(std::rand() % 41 - 20), 12.0f, static_cast<float>(std::rand() % 21 - 10));
	baloon5.transform.position = glm::vec3(static_cast<float>(std::rand() % 41 - 20), 12.0f, static_cast<float>(std::rand() % 21 - 10));


	GlobalLightSource gls(0.3f, glm::vec3(1.0, 1.0, 1.0)); // (1.0, 1.0, 1.0) означает, что свет направлен вдоль диагонали в трехмерном пространстве. 
	PointLightSource pls(glm::vec4(-20.0, 6.0, 7.0, 1.0), 15.0f); // четвертая координата w обычно используется как вес для операций с матрицами

	//ProjectorLightSource pjls(glm::vec4(0.5, 2.0, 4.5, 1.0), 3000.0f, glm::normalize(glm::vec3(0, 0.2, -1)), 150.0f);
	ProjectorLightSource pjls(glm::vec4(5, 7.0, 4.5, 1.0), 3000.0f, glm::normalize(glm::vec3(1, 1.5, +0.5)), 150.0f);

	auto scene = Scene(std::vector({ snow, tree, airship, sled, baloon1, baloon2, baloon3, baloon4, baloon5}), camera, gls, pls, pjls);
	for (auto& obj : scene.objects) {
		obj.init();
	}

	checkOpenGLerror();

	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {

			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }


			if (event.type == sf::Event::EventType::KeyPressed) {

				if (event.key.code == sf::Keyboard::D) {
					scene.camera.ProcessKeyboard(RIGHT,0.25f);
				}
				else if (event.key.code == sf::Keyboard::A) {
					scene.camera.ProcessKeyboard(LEFT, 0.25f);
				}
				else if (event.key.code == sf::Keyboard::W) {
					scene.camera.ProcessKeyboard(FORWARD, 0.25f);
				}
				else if (event.key.code == sf::Keyboard::S) {
					scene.camera.ProcessKeyboard(BACKWARD, 0.25f);
				}
				else if (event.key.code == sf::Keyboard::O) {
					scene.pjls.intencity = 0.0f;
				}
				else if (event.key.code == sf::Keyboard::P) {
					scene.pjls.intencity = 3000.0f;
				}
				//Управление дирижаблем
				else if (event.key.code == sf::Keyboard::Up) {
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x, scene.objects[2].transform.position.y + 0.3f, scene.objects[2].transform.position.z);
				}
				else if (event.key.code == sf::Keyboard::Down) {
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x, scene.objects[2].transform.position.y-0.3f, scene.objects[2].transform.position.z);
				}
				else if (event.key.code == sf::Keyboard::Left) {
					scene.objects[2].transform.rotation = glm::vec3(0.0f, 90.f, 0.0f);
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x , scene.objects[2].transform.position.y, scene.objects[2].transform.position.z - 0.3f);
					scene.pjls.projectorPos = glm::vec4(scene.pjls.projectorPos.x, scene.pjls.projectorPos.y, scene.pjls.projectorPos.z - 0.3f, scene.pjls.projectorPos.w);
				}
				else if (event.key.code == sf::Keyboard::Right) {
					scene.objects[2].transform.rotation = glm::vec3(0.0f, -90, 0.0f);
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x , scene.objects[2].transform.position.y , scene.objects[2].transform.position.z + 0.3f);
					scene.pjls.projectorPos = glm::vec4(scene.pjls.projectorPos.x, scene.pjls.projectorPos.y, scene.pjls.projectorPos.z + 0.3f, scene.pjls.projectorPos.w);
				}
				else if (event.key.code == sf::Keyboard::LShift) {
					scene.objects[2].transform.rotation = glm::vec3(0.0f, 0.0f, 0.0f);
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x + 0.3f, scene.objects[2].transform.position.y, scene.objects[2].transform.position.z);
					scene.pjls.projectorPos = glm::vec4(scene.pjls.projectorPos.x + 0.3f, scene.pjls.projectorPos.y, scene.pjls.projectorPos.z, scene.pjls.projectorPos.w);
				}
				else if (event.key.code == sf::Keyboard::Space) {
					scene.objects[2].transform.rotation = glm::vec3(0.0f, 180.0f, 0.0f);
					scene.objects[2].transform.position = glm::vec3(scene.objects[2].transform.position.x - 0.3f, scene.objects[2].transform.position.y, scene.objects[2].transform.position.z);
					scene.pjls.projectorPos = glm::vec4(scene.pjls.projectorPos.x - 0.3f, scene.pjls.projectorPos.y, scene.pjls.projectorPos.z, scene.pjls.projectorPos.w);
				}							
			}
			else if (event.type == sf::Event::MouseMoved)
			{
				
				scene.camera.ProcessMouseMovement(-(mouseX - event.mouseMove.x), mouseY - event.mouseMove.y);
				
				mouseX = event.mouseMove.x;
				mouseY = event.mouseMove.y;
			}
		}
		sf::Clock clock;

		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			Draw(scene);
			window.display();
		}

	}
}