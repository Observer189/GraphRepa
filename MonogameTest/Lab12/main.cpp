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
#include "subtask1.h"
#include "subtask2.h"
#include "subtask3.h"
#include "subtask4.h"





int main() {
	sf::Window window(sf::VideoMode(600, 600), "My OpenGL window", sf::Style::Default, sf::ContextSettings(24));
	window.setVerticalSyncEnabled(true);
	window.setActive(true);
	glewInit();
	glEnable(GL_DEPTH_TEST);
	//subtask1(window);
	//subtast2(window);
	//subtast3(window);
	subtask4(window);

}