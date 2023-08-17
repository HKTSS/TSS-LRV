namespace Plugin.Data {
    internal class ReporterMessage {
        public int Duration;
        public int State;
        public int MaxY;
        public int IncrementY;
        public ReporterMessage() {
        }

        public ReporterMessage(int duration, int states, int maxY, int incrementY) {
            this.Duration = duration;
            this.State = states;
            this.MaxY = maxY;
            this.IncrementY = incrementY;
        }
    }
}
