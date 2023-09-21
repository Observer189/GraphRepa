using MonogameTest;

using (var game = new Game1())
{
    game.Run();
}

using (var secondGame = new Game2())
{
    secondGame.Run();
}