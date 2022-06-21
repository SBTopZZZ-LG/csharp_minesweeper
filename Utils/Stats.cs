namespace Minesweeper.Utils
{
    /// <summary>
    /// Represents the Statistics for a Game object.
    /// </summary>
    public class Stats
    {
        private int minesLive;
        public int MinesLive
        {
            get { return minesLive; }
        }

        private int score;
        public int Score
        {
            get { return Score; }
        }

        public Stats(int minesLive, int score)
        {
            this.minesLive = minesLive;
            this.score = score;
        }

        /// <summary>
        /// Updates the MinesLive field value.
        /// </summary>
        /// <param name="minesLive">New value</param>
        public void SetMinesLive(int minesLive)
        {
            this.minesLive = minesLive;
        }

        /// <summary>
        /// Updates the Score field value.
        /// </summary>
        /// <param name="score">New value</param>
        public void SetScore(int score)
        {
            this.score = score;
        }

        /// <summary>
        /// Resets the MinesLive and Score fields.
        /// </summary>
        /// <param name="minesLive">Reset value for MinesLive field</param>
        /// <param name="score">Reset value for Score field</param>
        public void Reset(int minesLive, int score)
        {
            this.minesLive = minesLive;
            this.score = score;
        }
    }
}

