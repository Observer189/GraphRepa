#include <iostream>
#include <fstream>
#include <string>

#include <GL/glew.h>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <random>
#include "obj_defs.h"

std::string LoadFromFile(const char* path) {
	std::ifstream ifs(path);
	const std::string vsh((std::istreambuf_iterator<char>(ifs)), (std::istreambuf_iterator<char>()));
	return vsh;
}

int main() {
	sf::Window window(sf::VideoMode(600, 600), "My OpenGL window", sf::Style::Default, sf::ContextSettings(24));
	auto t = std::string("");
	auto v = VertexShader(t);
	window.setVerticalSyncEnabled(true);
	window.setActive(true);
	glewInit();
	while (window.isOpen()) {
		sf::Event event;
		while (window.pollEvent(event)) {
			if (event.type == sf::Event::Closed) { window.close(); }
			else if (event.type == sf::Event::Resized) { glViewport(0, 0, event.size.width, event.size.height); }

			if (event.type == sf::Event::EventType::KeyPressed) {
				
			}
		}
		if (window.isOpen()) {
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			//Draw();
			window.display();
		}
	}
}