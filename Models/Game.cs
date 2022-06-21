namespace Minesweeper.Models
{
	public class Game
	{
		public enum Notation
        {
			UNVISITED = 0,
			CLEAR = 1,
			FLAGGED = 2,
			BOMB = 3,
		}

        #region CONSTANTS
        public static readonly string[] NotationsList = new string[4] { "⬛", "⬜", "🟧", "🟥" };
		public static readonly string ToggleHighlight = "🟦";
        #endregion

        #region GAME_ELEMENTS
        public readonly string[][] Matrix;
		public int[][] MinesMatrix;
		public List<Utils.Position> MinesIndices;
        #endregion

        #region GAME_STATE_VARS
        public Utils.Stats Statistics;
		public bool GameOver, isReady;
		#endregion

		/// <summary>
		/// Instantiates a new Game object.
		/// </summary>
		/// <param name="width">Game matrix width (> 3, <= 100)</param>
		/// <param name="height">Game matrix height (> 3, <= 100)</param>
		/// <exception cref="Exception">Thrown if the provided width or height is invalid.</exception>
		public Game(int width, int height)
		{
			if (width <= 3 || width > 100)
				throw new Exception("Game matrix width should be greater than 3 and at most 100");
			if (height <= 3 || height > 100)
				throw new Exception("Game matrix height should be greater than 3 and at most 100");

			Matrix = new string[height][];
			MinesMatrix = new int[height][];
			for (int i = 0; i < height; i++)
            {
				Matrix[i] = new string[width];
				for (int j = 0; j < width; j++)
					Matrix[i][j] = NotationsList[(int)Notation.UNVISITED];

				MinesMatrix[i] = new int[width];
			}

			MinesIndices = new List<Utils.Position>();
			Statistics = new Utils.Stats(GetTotalMineCount(), 0);
			isReady = false;
		}

		/// <summary>
        /// </summary>
        /// <returns>Game matrix width</returns>
		public int GetWidth()
        {
			return Matrix[0].Length;
        }

		/// <summary>
        /// </summary>
        /// <returns>Game matrix height</returns>
		public int GetHeight()
        {
			return Matrix.Length;
        }

		/// <summary>
        /// </summary>
        /// <returns>Total mine count</returns>
		public int GetTotalMineCount()
        {
			return GetWidth() + (int)(0.5 * GetWidth());
        }

		/// <summary>
        /// Prepares the game matrix by placing the mines and instantiates new Statistics.
        /// </summary>
		public void Ready()
        {
			Random random = new Random();
			for (int i = 0; i < GetTotalMineCount(); i++)
            {
				int x = random.Next(GetWidth()), y = random.Next(GetHeight());
				if (MinesMatrix[y][x] == -1)
                {
					i--;
					continue;
				}

				MinesIndices.Add(new Utils.Position(x, y));
				MinesMatrix[y][x] = -1;
				for (int _y = y - 1; _y <= y + 1; _y++)
					for (int _x = x - 1; _x <= x + 1; _x++)
						if (_x >= 0 && _x < GetWidth() && _y >= 0 && _y < GetHeight() && MinesMatrix[_y][_x] != -1 && !(_x == x && _y == y))
							MinesMatrix[_y][_x]++;
			}

			Statistics = new Utils.Stats(GetTotalMineCount(), 0);
			GameOver = false;
			isReady = true;
		}

		/// <summary>
        /// Prepares the game matrix by placing the mines such that the first reveal will always reveal a CLEAR block. Also instantiates new Statistics.
        /// </summary>
        /// <param name="pos">Position which will be set as CLEAR</param>
		public void Ready(Utils.Position pos)
        {
			Random random = new Random();
			for (int i = 0; i < GetTotalMineCount(); i++)
			{
				int x = random.Next(GetWidth()), y;
				if (Math.Abs(x - pos.x) <= 1)
					do
					{
						y = random.Next(GetHeight());
					} while (Math.Abs(y - pos.y) <= 1);
				else
				{
					y = random.Next(GetHeight());
					if (Math.Abs(y - pos.y) <= 1)
						do
						{
							x = random.Next(GetWidth());
						} while (Math.Abs(x - pos.x) <= 1);
				}

				if (MinesMatrix[y][x] == -1)
				{
					i--;
					continue;
				}

				MinesIndices.Add(new Utils.Position(x, y));
				MinesMatrix[y][x] = -1;
				for (int _y = y - 1; _y <= y + 1; _y++)
					for (int _x = x - 1; _x <= x + 1; _x++)
						if (_x >= 0 && _x < GetWidth() && _y >= 0 && _y < GetHeight() && MinesMatrix[_y][_x] != -1 && !(_x == x && _y == y))
							MinesMatrix[_y][_x]++;
			}

			Statistics = new Utils.Stats(GetTotalMineCount(), 0);
			GameOver = false;
			isReady = true;
		}

		/// <summary>
        /// Checks if the game can be continued, otherwise stops the game.
        /// </summary>
		private void CheckUpdateGameState()
        {
			if (GameOver)
				return;
			if (Statistics.MinesLive != 0)
				return;

			foreach (string[] row in Matrix)
				foreach (string block in row)
					if (block.Equals(NotationsList[(int)Notation.UNVISITED]))
						return;

			GameOver = true;
        }

		/// <summary>
        /// Reveals a chosen block. The game will stop if the chosen block is a mine, will automatically reveal adjacent blocks if the chosen block is CLEAR, or will reveal the chosen block and stop immediately.
        /// </summary>
        /// <param name="pos">Position to reveal</param>
        /// <param name="_ignore_pos">Exclusion to the function in case the next block is again a CLEAR block, to avoid endless looping.</param>
        /// <exception cref="Exception">Throws exception if the provided position(s) are invalid.</exception>
		public void Reveal(Utils.Position pos, Utils.Position? _ignore_pos = null)
        {
			if (GameOver)
				return;

			if (pos.x < 0 || pos.x >= GetWidth())
				throw new Exception("x must be non-negative and less than width.");
			if (pos.y < 0 || pos.y >= GetHeight())
				throw new Exception("y must be non-negative and less than height.");

			if (Matrix[pos.y][pos.x] != NotationsList[(int)Notation.UNVISITED])
				return;

			if (_ignore_pos == null)
				_ignore_pos = new Utils.Position(-1, -1);

			string[] NumberEmojis = new string[] { "0️⃣", "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣" };
			switch (MinesMatrix[pos.y][pos.x])
            {
				case -1:
                    {
						foreach (Utils.Position mineLocation in MinesIndices)
							Matrix[mineLocation.y][mineLocation.x] = NotationsList[(int)Notation.BOMB];
						this.GameOver = true;

						break;
                    }
				case 0:
                    {
						Matrix[pos.y][pos.x] = NotationsList[(int)Notation.CLEAR];

						for (int _x = pos.x - 1; _x <= pos.x + 1; _x++)
							for (int _y = pos.y - 1; _y <= pos.y + 1; _y++)
								if ((_x == pos.x && _y == pos.y) || (_x == _ignore_pos.x && _y == _ignore_pos.y))
									continue;
                                else if ((_x >= 0 && _x < GetWidth()) && (_y >= 0 && _y < GetHeight()) && Matrix[_y][_x] != NotationsList[(int)Notation.CLEAR])
									Reveal(new Utils.Position(_x, _y), new Utils.Position(pos.x, pos.y));

						break;
                    }
				default:
                    {
						Matrix[pos.y][pos.x] = NumberEmojis[MinesMatrix[pos.y][pos.x]];
						break;
                    }
            }

			CheckUpdateGameState();
		}

		/// <summary>
		/// Flags/Unflags a chosen UNVISITED block. Will not flag if the mines live value is `0`.
		/// </summary>
		/// <param name="pos">Position to flag</param>
		/// <exception cref="Exception">Throws exception if the provided position is invalid.</exception>
		public void FlagToggle(Utils.Position pos)
        {
			if (GameOver)
				return;

			if (pos.x < 0 || pos.x >= GetWidth())
				throw new Exception("x must be non-negative and less than width.");
			if (pos.y < 0 || pos.y >= GetHeight())
				throw new Exception("y must be non-negative and less than height.");

			if (Matrix[pos.y][pos.x] != NotationsList[(int)Notation.UNVISITED] && Matrix[pos.y][pos.x] != NotationsList[(int)Notation.FLAGGED])
				return;

			if (Matrix[pos.y][pos.x] == NotationsList[(int)Notation.FLAGGED])
            {
				Matrix[pos.y][pos.x] = NotationsList[(int)Notation.UNVISITED];
				Statistics.SetMinesLive(Statistics.MinesLive + 1);
			} else if (Statistics.MinesLive > 0)
            {
				Matrix[pos.y][pos.x] = NotationsList[(int)Notation.FLAGGED];
				Statistics.SetMinesLive(Statistics.MinesLive - 1);
			}

			CheckUpdateGameState();
		}

		/// <summary>
        /// Prints the game matrix to the console.
        /// </summary>
        /// <param name="togglePos">Position representing the toggle highlighter</param>
		public void PrintMatrix(Utils.Position? togglePos = null)
        {
			for (int i = 0; i < GetHeight(); i++)
			{
				for (int j = 0; j < GetWidth(); j++)
					Console.Write(togglePos != null && (togglePos.x == j && togglePos.y == i) ? ToggleHighlight : Matrix[i][j]);
				Console.WriteLine();
			}
			Console.WriteLine("Mines live: " + Statistics.MinesLive);
		}
	}
}