using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class GameManager
    {
        public GameState State;

        public event Action<GameState> OnGameStateChanged;

        public GameManager()
        {
            State = GameState.Idle;
        }

        public void SetState(GameState newState)
        {
            if (State == newState) return;

            State = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
