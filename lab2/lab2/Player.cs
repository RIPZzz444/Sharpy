using System;

namespace CatAndMouseGame
{
    public class Player
    {
        public string name;
        public int location;
        public State state = State.NotInGame;
        public int distanceTraveled = 0;

        public Player(string name)
        {
            this.name = name;
            this.location = -1;
        }

        public void Move(int steps, int fieldSize)
        {
            if (state == State.NotInGame)
            {
                location = steps;
                state = State.Playing;
            }
            else if (state == State.Playing)
            {
                int zeroBasedIndex = location - 1 + steps;
                zeroBasedIndex = ((zeroBasedIndex % fieldSize) + fieldSize) % fieldSize;
                location = zeroBasedIndex + 1;

                distanceTraveled += Math.Abs(steps);
            }
        }
    }
}