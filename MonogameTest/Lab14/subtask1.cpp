#include <string>
#include "subtask1.h"


float mouseX;
float mouseY;


void Draw(Scene& scene) {
	for (auto& obj : scene.objects) {
		obj.update(scene.camera);
		obj.lightUpdate(scene);
		obj.vao.get()->Bind();
		obj.texture.get()->Bind();
		obj.program.get()->Bind();
		glDrawElements(GL_TRIANGLES, obj.mesh.get()->indices.size(), GL_UNSIGNED_INT, 0);
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
	//window.setMouseCursorGrabbed(true);


	glEnable(GL_DEPTH_TEST);
	/*auto s = std::string("./objects/12140_Skull_v3_L2.obj"s);
	std::string frag = stringFromFile("./shaders/subtask1.frag");
	std::string vert = stringFromFile("./shaders/subtask1.vert");
	auto prog = ProgramShader::make_from(vert, frag);
	auto tex = std::make_shared<Texture2D>();
	tex.get()->LoadFromPNG("./objects/Skull.png");
	auto skullMeshPointer = std::make_shared<Object3D>(s);
	auto skull = GameObject(skullMeshPointer, tex, prog);*/
	auto skull1 = GameObject::make_from_strings("./objects/12140_Skull_v3_L2.obj"s, "./objects/Skull.png"s, "./shaders/phong.vert"s, "./shaders/phong.frag"s);
	auto skull2 = GameObject::make_from_strings("./objects/12140_Skull_v3_L2.obj"s, "./objects/Skull.png"s, "./shaders/toon.vert"s, "./shaders/toon.frag"s);
	auto skull3 = GameObject::make_from_strings("./objects/12140_Skull_v3_L2.obj"s, "./objects/Skull.png"s, "./shaders/minnaert.vert"s, "./shaders/minnaert.frag"s);

	skull1.transform.scale = glm::vec3(0.2f);
	skull1.transform.position = glm::vec3(0, -1, 0);
	skull1.transform.Rotate(glm::vec3(-90.0f, 0.f, 0.f));
	skull2.transform.scale = glm::vec3(0.2f);
	skull2.transform.position = glm::vec3(0, 6, 0);
	skull2.transform.Rotate(glm::vec3(-90.0f, 0.f, 180.0f));
	skull3.transform.scale = glm::vec3(0.2f);
	skull3.transform.position = glm::vec3(0, -7, 0);
	skull3.transform.Rotate(glm::vec3(-90.0f, 0.f, 90.0f));
	GlobalLightSource gls(0.5f, glm::vec3(0.0, 1.0, 0.0));
	PointLightSource pls(glm::vec4(5.0, 0.0, 0.0, 1.0), 3.0f);
	ProjectorLightSource pjls(
		glm::vec4(-7.0, 1.0, 0.0, 1.0), 10.0f, glm::normalize(glm::vec3(-1, 0, 0)), 50.0f);
	auto scene = Scene(std::vector({ skull1, skull2, skull3 }), camera, gls, pls, pjls);
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
					//transform = glm::translate(transform,glm::vec3(0.05f,0.0f,0.0f));
					scene.camera.ProcessKeyboard(RIGHT,0.05f);

				}
				else if (event.key.code == sf::Keyboard::A) {
					scene.camera.ProcessKeyboard(LEFT, 0.05f);

				}
				else if (event.key.code == sf::Keyboard::W) {
					scene.camera.ProcessKeyboard(FORWARD, 0.05f);

				}
				else if (event.key.code == sf::Keyboard::S) {
					scene.camera.ProcessKeyboard(BACKWARD, 0.05f);

				}
				

			}
			else if (event.type == sf::Event::MouseMoved)
			{
				
				scene.camera.ProcessMouseMovement(-(mouseX - event.mouseMove.x), mouseY - event.mouseMove.y);
				
				mouseX = event.mouseMove.x;
				mouseY = event.mouseMove.y;
				
				//std::cout << "new mouse x: " << event.mouseMove.x<< std::endl;
				//std::cout << "new mouse y: " << event.mouseMove.y << std::endl;
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