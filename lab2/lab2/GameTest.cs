using NUnit.Framework;
using CatAndMouseGame;

namespace CatAndMouseGame.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void Player_InitialMove_SetsPositionWithoutIncreasingDistance()
        {
            Player player = new Player("Mouse");

            player.Move(10, 20);

            Assert.That(player.location, Is.EqualTo(10));
            Assert.That(player.state, Is.EqualTo(State.Playing));
            Assert.That(player.distanceTraveled, Is.EqualTo(0));
        }

        [Test]
        public void Player_MoveForward_UpdatesLocationAndDistance()
        {
            Player player = new Player("Cat");
            player.Move(5, 20);

            player.Move(3, 20);

            Assert.That(player.location, Is.EqualTo(8));
            Assert.That(player.distanceTraveled, Is.EqualTo(3));
        }

        [Test]
        public void Player_MoveWrapAroundForward_LoopsCorrectly()
        {
            Player player = new Player("Mouse");
            player.Move(15, 16);

            player.Move(4, 16);

            Assert.That(player.location, Is.EqualTo(3));
            Assert.That(player.distanceTraveled, Is.EqualTo(4));
        }

        [Test]
        public void Player_MoveWrapAroundBackward_LoopsCorrectly()
        {
            Player player = new Player("Cat");
            player.Move(6, 26);

            player.Move(-7, 26);

            Assert.That(player.location, Is.EqualTo(25));
            Assert.That(player.distanceTraveled, Is.EqualTo(7));
        }

        [Test]
        public void Game_CatchMouse_EndsGameAndSetsWinnerLoser()
        {
            Game game = new Game(16);
            game.cat.Move(10, 16);
            game.mouse.Move(10, 16);

            if (game.cat.location == game.mouse.location)
            {
                game.cat.state = State.Winner;
                game.mouse.state = State.Loser;
                game.state = GameState.End;
            }

            Assert.That(game.state, Is.EqualTo(GameState.End));
            Assert.That(game.cat.state, Is.EqualTo(State.Winner));
            Assert.That(game.mouse.state, Is.EqualTo(State.Loser));
        }
    }
}